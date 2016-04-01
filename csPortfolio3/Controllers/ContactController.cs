using csPortfolio3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SendGrid;
using System.Configuration;
using System.Net.Mail;

namespace csPortfolio3.Controllers
{
    public class ContactController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contact
        public ActionResult Index()
        {
            return View(db.Contact);
        }
        // GET: Contact/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contact.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View();
        }
        // GET: Contact/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Contact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Message,Phone,Sendtime")] Contact contact)
        {
            contact.Sendtime = DateTime.Now;

            //create the email object first, then add the properties
            SendGrid.SendGridMessage myMessage = new SendGridMessage();
            myMessage.AddTo(ConfigurationManager.AppSettings["ContactEmail"]);
            myMessage.From = new MailAddress(contact.Email, contact.Name);
            myMessage.Subject = "New Personal Contact Email";
            myMessage.Text = contact.Message;

            //create a Web transport using Api key
            var transportWeb = new Web(ConfigurationManager.AppSettings["APIKey"]);
            //send the email
            transportWeb.DeliverAsync(myMessage);

            if (ModelState.IsValid)
            {
                //db = database ContactMessage = table this will add the message to the table save and redirect to "action" 
                contact.Success = true;
                db.Contact.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index", "Home", routeValues: new { cont = contact });
            }

            return View();
        }
        // GET: Contact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contact.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View();
        }
        // POST: Contact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Message,Phone,Sendtime")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: Contact/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contact.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = db.Contact.Find(id);
            db.Contact.Remove(contact);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}