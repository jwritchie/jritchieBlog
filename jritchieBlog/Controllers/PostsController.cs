// Using Directives point to Name spaces
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jritchieBlog.Models;
using jritchieBlog.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;

namespace jritchieBlog.Controllers
{
    [RequireHttps]
    public class PostsController : UniversalController
    {
        // This instance isn't needed when using the 'UniversalController' which has already instantiated an ApplicationDbContext. 
        // Instantiate db instance of ApplicationDbContext. Gives access all tables w/in database & dbSet (CRUD operations).
        //private ApplicationDbContext db = new ApplicationDbContext();


        //// GET: Posts
        //public ActionResult Index()
        //{
        //    // Returns all Posts as a List to the Index view.

        //    // Show newest post first (inverse order by Id).
        //    return View(db.Posts.OrderByDescending(p => p.Id));

        //    //return View(db.Posts.ToList());
        //}

        // GET: Posts
        public ActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);                                   // If null, defaults to number 1.

            if (Request.IsAuthenticated && User.IsInRole("Admin"))          // Confirm user is logged-in AND 'Admin'.
            {
                return View(db.Posts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
            }
            //                 '.Where(p => p.Published == true)' Restricts to Published Posts.
            return View(db.Posts.Where(p => p.Published == true).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Index(string searchStr, int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);                                   // Confirm user is logged-in AND 'Admin'.

            ViewBag.Search = searchStr;                                     // Use to hold searchTerm over multiple pages with pagination.
            SearchHelper search = new SearchHelper();
            var blogList = search.IndexSearch(searchStr);

            if (Request.IsAuthenticated && User.IsInRole("Admin"))          // Confirm user is logged-in AND 'Admin'.
            {
                // Return all posts matching searchTerm.
                return View(blogList.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));                     
            }
            // Return Published posts matching searchTerm.
            return View(blogList.Where(p => p.Published == true).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }


        // GET: Posts
        //public ActionResult Index(string searchTerm)
        //{
        //    if (searchTerm == null)
        //    {
        //        ViewBag.SearchTerm = "";
        //        ViewBag.Search = true;

        //        List<Post> defaultList = new List<Post>();
        //        foreach (Post post in db.Posts)
        //        {
        //            if (User.IsInRole("Admin"))
        //            {
        //                defaultList.Add(post);
        //            }
        //            else
        //            {
        //                if (post.Published)
        //                {
        //                    defaultList.Add(post);
        //                }
        //            }
        //        }
        //        //return View(db.Posts.OrderByDescending(p => p.Id).ToList());
        //        return View(defaultList.OrderByDescending(p => p.Id));
        //    }

        //    List<Post> searchList = new List<Post>();
        //    foreach (Post post in db.Posts)
        //    {
        //        if (post.Title.Contains(searchTerm) || post.Body.Contains(searchTerm))
        //        {
        //            if (User.IsInRole("Admin"))
        //            {
        //                searchList.Add(post);
        //            }
        //            else
        //            {
        //                if (post.Published)
        //                {
        //                    searchList.Add(post);
        //                }
        //            }
        //        }
        //    }

        //    //List<Post> searchList = new List<Post>();
        //    //foreach (Post post in db.Posts)
        //    //{
        //    //    if (post.Title.Contains(searchTerm) || post.Body.Contains(searchTerm))
        //    //    {
        //    //        searchList.Add(post);
        //    //    }
        //    //}

        //    if (searchList.Count > 0)
        //    {
        //        ViewBag.SearchTerm = searchTerm;
        //        //ViewBag.SearchTerm = "";
        //        ViewBag.Search = true;
        //        return View(searchList.OrderByDescending(p => p.Id));
        //    }
        //    else
        //    {
        //        ViewBag.SearchTerm = searchTerm;
        //        ViewBag.Search = false;
        //        //return View(db.Posts.ToList());
        //        return View(searchList.OrderByDescending(p => p.Id).ToList());
        //    }
        //}

        // GET: Posts/Details/5
        //public ActionResult Details(int? id)    // ? allows passing null values.
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = db.Posts.Find(id);      // Explicitly declare variable type is Post object.
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(post);
        //}

        public ActionResult Details(string slug)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.UserId = user;

            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.FirstOrDefault(p => p.Slug == slug);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Admin")]     // Requires Admin user be logged in; if not, redirects to log-in page.
        public ActionResult Create()
        {
            // Returns empty form to user for completion.
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]     // Requires Admin user be logged in; if not, redirects to log-in page.
        [HttpPost]                       // Attribute required for Post action to distinguish from Get action.
        [ValidateAntiForgeryToken]       // Creates token to confirm incoming data comes from the user.
        // Bind attribute requires the specified properties be completed before Post action.
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Body,MediaURL,Published,Slug")] Post post, HttpPostedFileBase image)
        {
            // Validate image.
            if (image != null && image.ContentLength > 0)                       // Confirm file has data
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                {
                    ModelState.AddModelError("image", "Invalid Format.");       // Validation message
                }
            }

            // Test whether all properties were received.
            if (ModelState.IsValid)
            {
                var filePath = "/Assets/Images/Uploads/";                       // MediaURL
                var absPath = Server.MapPath("~" + filePath);                   // Physical file

                if (image == null)
                {
                    post.MediaUrl = "/Assets/Images/DefaultImage.png";          // Set to default image if no image is specified
                }
                else
                {
                    post.MediaUrl = filePath + image.FileName;                  // Sets path of file in database
                    image.SaveAs(Path.Combine(absPath, image.FileName));        // Saves (adds) the physical file to the application
                }

                var Slug = StringUtilities.URLFriendly(post.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(post);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(post);
                }
                post.Slug = Slug;

                post.Created = DateTime.Now;

                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)       // Use id of specific Post to edit.
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
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,Body,MediaUrl,Published,Slug")] Post post, string mediaUrl, HttpPostedFileBase image)
        {
            // Validate image.
            if (image != null && image.ContentLength > 0)                       // Confirm file has data
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                {
                    ModelState.AddModelError("image", "Invalid Format.");       // Validation message
                }
            }

            // Test whether all properties were received.
            if (ModelState.IsValid)
            {
                // Generate new slug.
                var Slug = StringUtilities.URLFriendly(post.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(post);
                }

                // Compare existing slug with new slug (Has title/slug changed?).
                if (Slug != post.Slug)
                {
                    // If title/slug has changed, determine whether new title/slug is unique.
                    if (db.Posts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(post);
                    }
                }

                // Set new slug.
                post.Slug = Slug;

                // Set Updated date.
                post.Updated = DateTime.Now;

                if (image != null)
                {
                    var filePath = "/Assets/Images/Uploads/";                   // MediaURL
                    var absPath = Server.MapPath("~" + filePath);               // Physical file
                    post.MediaUrl = filePath + image.FileName;                  // Sets path of file in database
                    image.SaveAs(Path.Combine(absPath, image.FileName));        // Saves (adds) the physical file to the application
                }
                else
                {
                    post.MediaUrl = mediaUrl;                                   // Sets image to prior image. 
                }

                db.Entry(post).State = EntityState.Modified;                    // Sets State to 'Modified'.
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", new { slug = post.Slug });
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
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

        // POST: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);

            // Don't delete the DefaultImage.jpg!
            if (post.MediaUrl != "/Assets/Images/DefaultImage.png")
            {
                var absPath = Server.MapPath("~" + post.MediaUrl);          // Identify the Physical file.
                System.IO.File.Delete(absPath);                             // Delete the physical file.
                // File.Delete(absPath);                                    // This doesn't work, attempts to access 'File' within the Controller class.
            }

            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Called automatically after an Action is completed to delete temporary variables used/created by the Action.
        protected override void Dispose(bool disposing)     // Overrides virtual method in the base class.
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

