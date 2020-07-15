using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cafe24App.CriteoCatalog.Controllers
{
    public class OauthController : Controller
    {
        // GET: Oauth
        //public ActionResult Index(string code)
        public string Index(string code)
        {
            return code;
        }
    }
}