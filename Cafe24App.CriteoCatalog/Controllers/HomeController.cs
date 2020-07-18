using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cafe24App.CriteoCatalog.Controllers
{
    public class HomeController : Controller
    {
        private Random random = new Random();

        // GET: Home
        public ActionResult Index()
        {
            //string mall_id = "";
            //string client_id = "jfteFzqAlURv2XMrCUuFFL";
            //string encode_csrf_token = RandomString(12);
            //string scope = "mall.read_product";
            //string encode_redirect_uri = "https://criteo-catalog.azurewebsites.net/oauth";
            //return Redirect($"https://{mall_id}.cafe24api.com/api/v2/oauth/authorize?response_type=code&client_id={client_id}&state={encode_csrf_token}&redirect_uri={encode_redirect_uri}&scope={scope}");

            return View();
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}