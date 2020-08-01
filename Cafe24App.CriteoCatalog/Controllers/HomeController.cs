using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

            List<dynamic> objList = new List<dynamic>();
            foreach (string mallId in ReadInstalledMalls())
            {
                dynamic obj = new ExpandoObject();
                obj.id = mallId;
                obj.lastUpdated = GetLastSuccessfulGeneration(mallId);

                objList.Add(obj);
            }

            return View(objList);
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string[] ReadInstalledMalls()
        {
            string path = Path.Combine(Server.MapPath("~/Catalogs"));
            string filename = $"installed-apps.txt";
            string fullFilename = Path.Combine(path, filename);

            string installedMallString = string.Empty;
            //throw new Exception(fullFilename);
            if (System.IO.File.Exists(fullFilename))
            {
                using (StreamReader sr = new StreamReader(fullFilename))
                {
                    installedMallString = sr.ReadToEnd();
                }
            }
            else
                return new List<string>().ToArray();

            return installedMallString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Where(mallId => !string.IsNullOrEmpty(mallId)).ToArray();
        }
        private DateTime GetLastSuccessfulGeneration(string mallId)
        {
            string path = Path.Combine(Server.MapPath("~/Catalogs"));
            string filename = $"{mallId}.csv";

            try
            {
                return System.IO.File.GetLastWriteTime(Path.Combine(path, filename));
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }
    }
}