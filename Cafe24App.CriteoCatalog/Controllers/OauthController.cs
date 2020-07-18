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
        public ActionResult Index(string code, string state)
        {
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state))
            {
                return Content("Redirecting...<script>alert('앱이 성공적으로 설치되었습니다');window.location.href='/';</script>");
            }
            else
            {
                return Content("Redirecting...<script>alert('앱을 설치하는데 에러가 발생했습니다.관리자에게 문의 바랍니다');window.location.href='/';</script>");
            }

            
        }
    }
}