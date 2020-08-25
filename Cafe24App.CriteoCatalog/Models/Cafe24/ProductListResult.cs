using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cafe24App.CriteoCatalog.Models.Cafe24
{
    public class ProductListResult
    {
        public List<Product> products { get; set; }
    }

    [Serializable]
    public class Product
    {
        public int? product_no { get; set; }
        public string product_name { get; set; }
        public string price { get; set; }
        public string retail_price { get; set; }
        //public string summary_description { get; set; }
        //public string link { get; set; }
        //public Additional_Image[] additional_image { get; set; }
        //public string adult_certification { get; set; }
        public string detail_image { get; set; }
        public string sold_out { get; set; }
        //public string buy_limit_by_product { get; set; }
    }

    //public class CategoryInProduct
    //{
    //    public int category_no { get; set; }
    //    public string recommend { get; set; }
    //    public string _new { get; set; }
    //}

    //public class Additional_Image
    //{
    //    public string big { get; set; }
    //    public string medium { get; set; }
    //    public string small { get; set; }
    //}

    //public class Product
    //{
    //    public int product_no { get; set; }
    //    public string product_name { get; set; }
    //    public string retail_price { get; set; }
    //    public string price { get; set; }
    //    public string brand_name { get; set; }
    //    public string simple_description { get; set; }
    //    public string summary_description { get; set; }
    //    public int review_count { get; set; }
    //}

    //public class Expiration_Date
    //{
    //    public string start_date { get; set; }
    //    public string end_date { get; set; }
    //}

    //public class Promotion_Period
    //{
    //    public DateTime start_date { get; set; }
    //    public DateTime end_date { get; set; }
    //    public string dc_price { get; set; }
    //}

    //public class Point_Amount
    //{
    //    public string payment_method { get; set; }
    //    public string type { get; set; }
    //    public string value { get; set; }
    //    public string rate { get; set; }
    //}

}