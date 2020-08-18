using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cafe24App.CriteoCatalog.Controllers
{
    public class OauthController : Controller
    {
        // GET: Oauth
        //public ActionResult Index(string code)
        public ActionResult Index(string code, string state, string mallId)
        {
            if (!string.IsNullOrEmpty(mallId))
            {
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state))
                {
                    //Uri referrerUri = Request.UrlReferrer;
                    //string mallId = GetCurrentMallId();
                    //if (!string.IsNullOrEmpty(mallId))
                    //{
                        AddInstalledMall(mallId);
                        return Content("Redirecting...<script>alert('앱이 성공적으로 설치되었습니다');window.location.href='/';</script>");
                    //}
                    //else
                    //    return Content("Redirecting...<script>alert('mallId를 확인할 수 없습니다. 관리자에게 문의 바랍니다');window.location.href='/';</script>");
                }
                else
                    return Content("Redirecting...<script>alert('앱을 설치하는데 에러가 발생했습니다.관리자에게 문의 바랍니다');window.location.href='/';</script>");
            }
            else
            {
                return Content("Redirecting...<script>window.location.href = window.location + '&mallId=' + window.localStorage.getItem('mallId');</script>");
            }
        }

        private static string GetCurrentMallId()

        {
            string mallId = string.Empty;
            


            return mallId;
        }

        public void AddInstalledMall(string mallId)
        {
            string path = Path.Combine(Server.MapPath("~/Catalogs"));
            string filename = $"installed-apps.txt";
            string fullFilename = Path.Combine(path, filename);

            Directory.CreateDirectory(path);

            string installedAppsString = string.Empty;
            if (System.IO.File.Exists(fullFilename))
            {
                using (StreamReader sr = new StreamReader(fullFilename))
                {
                    installedAppsString = sr.ReadToEnd();
                }
            }

            if (!installedAppsString.Contains(mallId))
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, filename), true))
                {
                    outputFile.WriteLine(mallId);
                }
            }
        }
    }
}