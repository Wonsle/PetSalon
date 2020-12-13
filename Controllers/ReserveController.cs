using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetSalon.Controllers
{
    public class ReserveController : _BaseController
    {
        // GET: Reserve
        public ActionResult Index()
        {
            return View();
        }
    }
}