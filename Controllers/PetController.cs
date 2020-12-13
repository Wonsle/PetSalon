using PetSalon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetSalon.Controllers
{
    public class PetController : _BaseController
    {
        
        public ActionResult Index()
        {
            List<Pet> pet = new List<Pet>();

            return View(pet);
        }

        public ActionResult Create()
        {

            return View();
        }
    }
}