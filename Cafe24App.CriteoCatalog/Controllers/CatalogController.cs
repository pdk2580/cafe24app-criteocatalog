using Cafe24App.CriteoCatalog.Models;
using Cafe24App.CriteoCatalog.Models.Cafe24;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cafe24App.CriteoCatalog.Controllers
{
    [RoutePrefix("catalogs")]
    public class CatalogController : Controller
    {
        static string clientId = "jfteFzqAlURv2XMrCUuFFL";
        static readonly HttpClient client = new HttpClient();
        
        public CatalogController()
        {
            client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        }

        // GET: Catalog
        [Route("{mallId}")]
        public async Task<ActionResult> Index(string mallId)
        {
            if (!string.IsNullOrEmpty(mallId))
            {
                //var categoryListResult = await GetAllCategories(mallId);
                //var productListResult = await GetAllProducts(mallId);
                //var categoriesCount = await CountCategories(mallId);
                //var productsCount = await CountProducts(mallId);
                var topCategories = await GetTopCategories(mallId);
                var allProducts = await GetAllProducts(mallId, topCategories.Where(x => x.category_no.Equals(5121)));


                return Json(topCategories.Count(), JsonRequestBehavior.AllowGet);
                //return Json(productListResult, JsonRequestBehavior.AllowGet);
                //return Content(result.products.Count().ToString());
                //return Content(productsCount.ToString() + " | " + productListResult.products.Count());
            }

            return Content("MallId cannot be nulll or empty");
        }

        private async Task<IEnumerable<Product>> GetAllProducts(string mallId, IEnumerable<Category> categories)
        {
            IEnumerable<Product> products = new List<Product>();

            int limit = 100;
            int lastProductNo = 0;

            foreach (Category category in categories)
            {
                int categoryNo = category.category_no;

                int count = 0;
                while (count > 0 || lastProductNo.Equals(0))
                {
                    //string url = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit}&offset={offset}&fields=product_no,product_name,price,retail_price,display,selling,summary_description,adult_certification,detail_image,small_image,sold_out,category";
                    string url = lastProductNo.Equals(0) ?
                        $"https://{mallId}.cafe24api.com/api/v2/products?order=asc&sort=created_date&limit={limit}&category={categoryNo}&display=T&selling=T&fields=product_no,product_name,price,retail_price,summary_description,adult_certification,detail_image,sold_out" :
                        $"https://{mallId}.cafe24api.com/api/v2/products?since_product_no={lastProductNo}&limit={limit}&category={categoryNo}&display=T&selling=T&fields=product_no,product_name,price,retail_price,summary_description,adult_certification,detail_image,sold_out";

                    var response = await client.GetAsync(url);
                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        ProductListResult result = JsonConvert.DeserializeObject<ProductListResult>(response.Content.ReadAsStringAsync().Result);
                        products = products.Union(result.products);

                        count = result.products.Count();
                        if (!count.Equals(0))
                            lastProductNo = result.products.OrderBy(product => product.product_no).LastOrDefault().product_no;
                    }
                    else
                        lastProductNo = 9999;  // to exit loop
                }
            }

            return products;
        }

        //private async Task<ProductListResult> GetAllProducts(string mallId)
        //{
        //    HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        //    var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/products?limit=100&fields=product_no");
        //    //var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/products?fields=product_no,product_name,price,retail_price,display,selling,summary_description,adult_certification,detail_image,small_image,sold_out,category");
        //    //var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/mains/2/products");
        //    if (response.StatusCode.Equals(HttpStatusCode.OK))
        //    {
        //        ProductListResult products = JsonConvert.DeserializeObject<ProductListResult>(response.Content.ReadAsStringAsync().Result);
        //        return products;
        //    }

        //    return null;
        //}

        //private async Task<int> CountProducts(string mallId)
        //{
        //    //int count = 0;

        //    HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        //    var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/products/count?display=T&selling=T&approve_status=C");
        //    if (response.StatusCode.Equals(HttpStatusCode.OK))
        //    {
        //        dynamic jsonResult = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
        //        return jsonResult.count;
        //    }

        //    return 0;
        //}

        private async Task<IEnumerable<Category>> GetTopCategories(string mallId)
        {
            List<Category> categories = new List<Category>();

            int limit = 100;
            int offset = 0;

            int count = 0;
            while (count > 0 || offset.Equals(0))
            {
                string url = $"https://{mallId}.cafe24api.com/api/v2/categories?limit={limit}&offset={offset}&category_depth=1&fields=category_no,category_name";

                var response = await client.GetAsync(url);
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    CategoryListResult result = JsonConvert.DeserializeObject<CategoryListResult>(response.Content.ReadAsStringAsync().Result);
                    categories.AddRange(result.categories);

                    count = result.categories.Count();
                    offset += limit;
                }
                else
                {
                    offset = 8000;  // to exit loop

                    throw new Exception(response.Content.ReadAsStringAsync().Result + " | " + url);
                }
                    
            }

            return categories;
        }

        private async Task<CategoryListResult> GetAllCategories(string mallId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
            var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/categories?limit=100&fields=category_no,category_name,full_category_no,full_category_name");
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                CategoryListResult categories = JsonConvert.DeserializeObject<CategoryListResult>(response.Content.ReadAsStringAsync().Result);
                return categories;
            }

            return null;
        }

        private async Task<int> CountCategories(string mallId)
        {
            //int count = 0;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
            var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/categories/count");
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                dynamic jsonResult = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                return jsonResult.count;
            }

            return 0;
        }
    }
}