using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cafe24App.CriteoCatalog.Models.Criteo
{
    public class Catalog
    {
        public string id { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string image_link { get; set; }
        public string price { get; set; }
        public string sale_price { get; set; }
        public string categoryid1 { get; set; }
        public string categoryid2 { get; set; }
        public string categoryid3 { get; set; }
    }
}