using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using csPortfolio3.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
//using PagedList;
//using PagedList.Mvc;

namespace csPortfolio3.Controllers
{
    [RequireHttps]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize (Roles="Admin")]
        // GET: Posts
        public ActionResult Index()
        {
           
            return View(db.Posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug)) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.FirstOrDefault(p=>p.Slug == slug);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }
        [Authorize(Roles = "Admin")]
        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaUrl,Slug")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtlities.URLFriendly(post.Title);
                if(String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(post);
                }
                if(db.Posts.Any(p=>p.Slug ==Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(post);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/assets/Uploads/"), fileName));
                    post.MediaUrl = "~/assets/Uploads/" + fileName;
                }
                post.Slug = Slug;
                post.Created = System.DateTimeOffset.Now;
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }
        public static class ImageUploadValidator
        {
            public static bool IsWebFriendlyImage(HttpPostedFileBase file)
            {
                if (file == null)
                    return false;
                if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                    return false;
                try
                {
                    using (var img = Image.FromStream(file.InputStream))
                    {
                        return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                               ImageFormat.Png.Equals(img.RawFormat) ||
                               ImageFormat.Gif.Equals(img.RawFormat);
                    }

                }
                catch
                {
                    return false;
                }
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include = "PostId,Body")] Comment comment)
        {
            var slug = db.Posts.Find(comment.PostId).Slug;
            if (ModelState.IsValid)
            {
                comment.AuthorId = User.Identity.GetUserId();
                comment.Created = System.DateTimeOffset.Now;

                db.Comments.Add(comment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { Slug = slug });
        }


        //get editcomment
        [Authorize(Roles = "Admin, Moderator")]//if admin and moderator then put this (Roles = "Admin, Moderator")
        public ActionResult EditComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "PostId,Body,Id")] Comment comment)
        {
            // var post = db.Posts.Find(comment.PostId).Id;
            if (ModelState.IsValid)
            {
                //comment.AuthorId = User.Identity.GetUserId();
                var slug = db.Posts.Find(comment.PostId).Slug;
                db.Comments.Attach(comment);
                db.Entry(comment).Property("Body").IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Details", new { Slug = slug });
            }

            return View(comment);
        }
        //get deletecomment
        [Authorize(Roles = "Admin, Moderator")]//if admin and moderator then put this (Roles = "Admin, Moderator")
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }


        //post deletecomment
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommentConfirmed(int id)
        {

            Comment comment = db.Comments.Find(id);
            var slug = db.Posts.Find(comment.PostId).Slug;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", new { Slug = slug });
        }






        [Authorize (Roles ="Admin")]
        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Body,MediaUrl,Published")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                db.Posts.Attach(post);

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/assets/Uploads/"), fileName));
                    post.MediaUrl = "~/assets/Uploads/" + fileName;
                    db.Entry(post).Property("MediaUrl").IsModified = true;

                }
                post.Updated = System.DateTimeOffset.Now;
                /*db.Entry(post).Property("Title").IsModified = true;*/ //do this for every column you want to change
                db.Entry(post).Property("Body").IsModified = true;
                //db.Entry(post).Property("Slug").IsModified = true;
                //db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(post);
        }
        [Authorize(Roles = "Admin")]
        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
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
