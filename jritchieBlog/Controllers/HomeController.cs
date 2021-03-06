﻿using jritchieBlog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace jritchieBlog.Controllers
{
    [RequireHttps]
    public class HomeController : UniversalController
    {
        public ActionResult Index()
        {
            return Redirect("https://jritchie-blog.azurewebsites.net/");
            //return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return Redirect("http://jritchie-portfolio.azurewebsites.net/");
            //return View();
        }

        public ActionResult Contact()
        {
            return Redirect("http://jritchie-portfolio.azurewebsites.net/Home/Contact");
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "MyPortfolio<jritchie.nc@gmail.com>";
                    //model.Body = "This is a message from your portfolio site.  The name and the email of the contacting person is above.";
                    string subject = null;
                    if (model.Subject != null)
                    {
                        subject = "Portfolio Contact Email: " + model.Subject;
                    }
                    else
                    {
                        subject = "Portfolio Contact Email";
                    }


                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                        //Subject = "Portfolio Contact Email" + model.Subject,
                        Subject = subject,
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);             // Sends email.

                    return View(new EmailModel());          // Return to View (need to send empty Email model)
                    //return RedirectToAction("Sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

    }
}