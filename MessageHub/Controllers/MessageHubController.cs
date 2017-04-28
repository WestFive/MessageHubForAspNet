using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageHub.Controllers
{
    public class MessageHubController : Controller
    {
        // GET: MessageHub
        public ActionResult connection()
        {
            return View();
        }

        public ActionResult server()
        {
            //ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ActionResult directive()
        {
            //ViewData["Message"] = "Your application description page.";

            return View();
        }


        public ActionResult lane()
        {

            return View();
        }

        public ActionResult queue()
        {

            return View();
        }
        public ActionResult Error()
        {

            return View();
        }
    }
}