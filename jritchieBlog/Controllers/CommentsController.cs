﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jritchieBlog.Models;
using jritchieBlog.Controllers;
using Microsoft.AspNet.Identity;


namespace jritchieBlog.Models
{
    [RequireHttps]
    public class CommentsController : UniversalController
    {   
        // This instance isn't needed when using the 'UniversalController' which has already instantiated an ApplicationDbContext. 
        // Instantiate db instance of ApplicationDbContext. Gives access all tables w/in database & dbSet (CRUD operations).
        //private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Post);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        [Authorize(Roles ="Admin")]
        public ActionResult Details(int? id)
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

        // GET: Comments/Create
        //[Authorize]
        //public ActionResult Create()
        //{
            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.PostId = new SelectList(db.Posts, "Id", "Title");
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PostId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment, String postToReturnTo)
        {

            if (ModelState.IsValid)
            {
                //string postToReturnTo = comment.Post.Slug;

                comment.Created = DateTime.Now;
                comment.AuthorId = User.Identity.GetUserId();

                //comment.AuthorId = db.Users.Find(User.Identity.Name).Id;

                db.Comments.Add(comment);
                db.SaveChanges();
                
                //return RedirectToAction("Index");
                return RedirectToAction("Details", "Posts", new { slug = postToReturnTo });
            }

            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            //ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            //return View(comment);
            return RedirectToAction("Details", "Posts", new { slug = postToReturnTo });
        }

        // GET: Comments/Edit/5
        [Authorize]
        //[Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
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
            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            //ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        //[Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,AuthorId,Body,Created,Updated,UpdateReason")] Comment comment, String postToReturnTo )
        {
            if (ModelState.IsValid)
            {
                //string postToReturnTo =         //comment.Post.Slug;

                comment.Updated = DateTime.Now;

                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Details", "Posts", new { slug = postToReturnTo });
            }
            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            //ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize]
        //[Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
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

        // POST: Comments/Delete/5
        [Authorize]
        //[Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);

            string postToReturnTo = comment.Post.Slug;

            db.Comments.Remove(comment);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return RedirectToAction("Details", "Posts", new { slug = postToReturnTo });
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
