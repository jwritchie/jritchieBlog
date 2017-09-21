using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jritchieBlog.Models
{

    public class Post
    {
        // Constructor.
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        // Properties.
        public int Id { get; set; }                 // Primary key.
        public DateTime Created { get; set; }   
        public DateTime? Updated { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [AllowHtml]
        public string Body { get; set; }
        public string MediaUrl { get; set; }
        public bool Published { get; set; }         // Visible (or not) to the public
        public string Slug { get; set; }

        public string Excerpt
        {
            get
            {
                //return Body.Substring(0, 30);
                if (Body.Length < 250)
                {
                    return Body;
                }
                else
                {
                    return Body.Substring(0, 250) + " ...";
                }
            }
        }

        // Collection of Comments.
        public virtual ICollection<Comment> Comments { get; set; }


    }
}