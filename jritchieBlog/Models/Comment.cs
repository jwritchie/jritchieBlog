using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jritchieBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }                 // Primary key.
        public int PostId { get; set; }             // Foreign key.
        public string AuthorId { get; set; }        // Foreign key.
        [Required]
        [AllowHtml]
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdateReason { get; set; }

        // Virtual properties (Foreign key relationships).
        public virtual Post Post { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}