using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cafe24App.CriteoCatalog.Models.Cafe24
{
    public class CategoryListResult
    {
        public Category[] categories { get; set; }
    }

    public class Category
    {
        public int category_no { get; set; }
        public string category_name { get; set; }
        //public Full_Category_No full_category_no { get; set; }
        //public Full_Category_Name full_category_name { get; set; }
    }

    //public class Full_Category_Name
    //{
    //    public string _1 { get; set; }
    //    public string _2 { get; set; }
    //    public string _3 { get; set; }
    //    public string _4 { get; set; }
    //}

    //public class Full_Category_No
    //{
    //    public string _1 { get; set; }
    //    public string _2 { get; set; }
    //    public string _3 { get; set; }
    //    public string _4 { get; set; }
    //}
}