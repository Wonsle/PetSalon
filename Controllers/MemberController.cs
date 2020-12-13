using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetSalon.Models;
namespace PetSalon.Controllers
{
    public class MemberController : _BaseController
    {
        MemberRepository memberrepo;
        public MemberController()
        {
            memberrepo = RepositoryHelper.GetMemberRepository();
        }
        
        // GET: Member
        public ActionResult Index()
        {
            return View(memberrepo.All());
        }
    }
}