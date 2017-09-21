using jritchieBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jritchieBlog.Helpers
{
    public class SearchHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IQueryable<Post> IndexSearch(string searchStr)
        {
            IQueryable<Post> result = null;
            if(searchStr != null)
            {
                result = db.Posts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                p.Body.Contains(searchStr) ||
                p.Comments.Any(c => c.Body.Contains(searchStr) ||
                c.Author.FirstName.Contains(searchStr) ||
                c.Author.LastName.Contains(searchStr) ||
                c.Author.Email.Contains(searchStr)
                ));
            }
            else
            {
                result = db.Posts.AsQueryable();
            }

            return result;
        }
    }
}