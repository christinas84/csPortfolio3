using csPortfolio3.Models;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace csPortfolio3.Controllers
{
    //use authorize you must be logged in to do anything in this controller
    //[Authorize]
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContact([Bind(Include = "Id,Name,Email,Message,Phone,Sendtime")] Contact contact)
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
                //return RedirectToAction("Index", "Home"/*, routeValues: new { cont = contact }*/);
                TempData["Message"] = "Success! Your email has been sent!";
                return Redirect(Url.RouteUrl(new { controller = "Home", action = "Index" }) + "#contact");

            }

            return View();
        }
        public ActionResult Blog(int? page, string query)
        {
            ////return View();
            //return View(db.Posts.ToList());
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.Query = query;
            var qposts = db.Posts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            {
                qposts = qposts.Where(p => p.Title.Contains(query) || p.Body.Contains(query) || p.Comments.Any(c => c.Body.Contains(query)));
            }
            var posts = qposts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize);
            return View(posts);

        }
    }
}