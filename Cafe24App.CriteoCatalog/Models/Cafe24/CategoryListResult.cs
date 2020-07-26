using Newtonsoft.Json;
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
        public int parent_category_no { get; set; }
        public Full_Category_No full_category_no { get; set; }
        public Full_Category_Name full_category_name { get; set; }

        public string GetCategory1()
        {
            //return $"{this.full_category_no?._1} / {this.full_category_name?._1}";

            if (!string.IsNullOrEmpty(this.full_category_name._1))
                return $"{this.full_category_no?._1} / {this.full_category_name?._1}";
            else
                return "";
        }

        public string GetCategory2()
        {
            if (!string.IsNullOrEmpty(this.full_category_name._2))
                return $"{this.full_category_no?._2} / {this.full_category_name?._2}";
            else
                return "";
        }

        public string GetCategory3()
        {
            if (!string.IsNullOrEmpty(this.full_category_name._3))
                return $"{this.full_category_no?._3} / {this.full_category_name?._3}";
            else
                return "";
        }

    }

    public class Full_Category_Name
    {
        [JsonProperty("1")]
        public string _1 { get; set; }
        [JsonProperty("2")]
        public string _2 { get; set; }
        [JsonProperty("3")]
        public string _3 { get; set; }
        [JsonProperty("4")]
        public string _4 { get; set; }
    }

    public class Full_Category_No
    {
        [JsonProperty("1")]
        public string _1 { get; set; }
        [JsonProperty("2")]
        public string _2 { get; set; }
        [JsonProperty("3")]
        public string _3 { get; set; }
        [JsonProperty("4")]
        public string _4 { get; set; }
    }
}