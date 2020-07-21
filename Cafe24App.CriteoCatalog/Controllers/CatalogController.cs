using Cafe24App.CriteoCatalog.Models;
using Cafe24App.CriteoCatalog.Models.Cafe24;
using Cafe24App.CriteoCatalog.Models.Criteo;
using MoreLinq;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;

namespace Cafe24App.CriteoCatalog.Controllers
{
    [RoutePrefix("catalogs")]
    public class CatalogController : Controller
    {
        string clientId = "jfteFzqAlURv2XMrCUuFFL";
        HttpClient client = new HttpClient();
        List<Product> products = new List<Product>();

        public CatalogController()
        {
            client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        }

        //[Route("test")]
        //public async Task<ActionResult> Test()
        //{
        //    var topCategories = await GetTopCategories("oneeight");
        //    //var count = await CountCategories("oneeight");

        //    return Content(topCategories.Count().ToString());
        //    //return Content(count.ToString());
        //}

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


                //var topCategories = await GetTopCategories(mallId);
                //var allProducts = await GetAllProducts(mallId, topCategories.Where(x => x.category_no.Equals(3839)));
                //var allProducts = await GetAllProducts(mallId, topCategories);
                var allProducts = await GetAllProducts(mallId);

                //List<Catalog> criteoCatalogs = new List<Catalog>();
                string baseURL = string.Format("{0}://{1}", new Uri(allProducts.FirstOrDefault().detail_image).Scheme, new Uri(allProducts.FirstOrDefault().detail_image).Host);
                var criteoCatalogs = allProducts.Select(product => new Catalog
                {
                    id = product.product_no.ToString(),
                    title = product.product_name,
                    price = product.price,
                    sale_price = product.retail_price,
                    image_link = product.detail_image,
                    link = string.Format("{0}/product/detail.html?product_no={1}", baseURL, product.product_no)
                });
                

                string csvString = CsvSerializer.SerializeToString<List<Catalog>>(criteoCatalogs.ToList());

                return File(new System.Text.UTF8Encoding().GetBytes(csvString), "text/csv", "oneeight.csv");
                //return Content(csvString, "text/csv", new System.Text.UTF8Encoding());




                //if (products.Count().Equals(0))
                //{
                //var tempProducts = await GetAllProducts(mallId, topCategories);
                //products = tempProducts.Where(product => product.sold_out.Equals("F")).ToList();
                //}

                //Cache[mallId] = JsonConvert.SerializeObject(tempProducts.Where(product => product.sold_out.Equals("F")));




                //XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(allProducts));


                //var binFormatter = new BinaryFormatter();
                //var mStream = new MemoryStream();
                //binFormatter.Serialize(mStream, products);
                //return File(mStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "test.json");


                //var jsonResult = Json(allProducts, JsonRequestBehavior.AllowGet);
                //jsonResult.MaxJsonLength = int.MaxValue;
                //return jsonResult;

                //return Content(allProducts.Count().ToString());

                //return Content(doc.OuterXml, "text/xml", System.Text.Encoding.UTF8);


                //return Json(productListResult, JsonRequestBehavior.AllowGet);
                //return Content(result.products.Count().ToString());
                //return Content(productsCount.ToString() + " | " + productListResult.products.Count());
            }

            return Content("MallId cannot be nulll or empty");
        }

        private async Task<IEnumerable<Product>> GetAllProducts(string mallId, IEnumerable<Category> categories)
        {
            var cache = MemoryCache.Default;

            //IEnumerable<Product> products = new List<Product>();
            var products = cache.Get(mallId);
            if (products == null)
            {
                IEnumerable<Product> tempProducts = new List<Product>();

                int limit = 100;


                int test1 = 0;
                int test2 = 0;
                string urlcheck = "";
                string categoryCountString = "";
                foreach (Category category in categories)
                {
                    int offset = 0;
                    //int lastProductNo = 0;
                    int categoryNo = category.category_no;

                    int categoryCount = 0;
                    string error = "";


                    int productCount = await CountProducts(mallId, categoryNo);
                    if (productCount.Equals(0))
                        continue;

                    int count = 0;
                    //while (count > 0 || lastProductNo.Equals(0))
                    while (count > 0 || offset.Equals(0))
                    {


                        if (offset.Equals(5000))
                            break;

                        //string url = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit}&offset={offset}&fields=product_no,product_name,price,retail_price,summary_description,detail_imagesold_out";
                        string url = $"https://{mallId}.cafe24api.com/api/v2/products?limit={limit}&offset={offset}&category={categoryNo}&display=T&selling=T&fields=product_no,product_name,price,retail_price,detail_image,sold_out";
                        //string url = lastProductNo.Equals(0) ?
                        //    $"https://{mallId}.cafe24api.com/api/v2/products?order=asc&sort=created_date&limit={limit}&category={categoryNo}&display=T&selling=T&adult_certification=F&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out" :
                        //    $"https://{mallId}.cafe24api.com/api/v2/products?since_product_no={lastProductNo}&limit={limit}&category={categoryNo}&display=T&selling=T&adult_certification=F&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out";

                        var response = await client.GetAsync(url);
                        if (response.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            try
                            {
                                ProductListResult result = JsonConvert.DeserializeObject<ProductListResult>(response.Content.ReadAsStringAsync().Result);

                                tempProducts = tempProducts.Concat(result.products);

                                count = result.products.Count();
                                //if (!count.Equals(0))
                                //    lastProductNo = result.products.OrderBy(product => product.product_no).LastOrDefault().product_no;
                                //else
                                //    lastProductNo = 9999;   // to exit loop when there is no products for the specific category
                                offset += limit;


                                test2 += count;
                                urlcheck += url + Environment.NewLine;
                                categoryCount += count;


                                //Random random = new Random();
                                //int randomNumber = random.Next(0, 300);

                                //System.Threading.Thread.Sleep(randomNumber);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {tempProducts.Count()}" + Environment.NewLine + urlcheck + Environment.NewLine + response.Content.ReadAsStringAsync().Result);
                            }

                            var apiLimit = response.Headers.GetValues("X-Api-Call-Limit").FirstOrDefault();
                            if (!string.IsNullOrEmpty(apiLimit))
                            {
                                if (apiLimit.Contains("29"))
                                    System.Threading.Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            offset = 5000;
                            error += response.Content.ReadAsStringAsync().Result + Environment.NewLine;
                            //lastProductNo = 9999;  // to exit loop

                        }



                    }

                    categoryCountString += $"{category.category_no}/{category.category_name}: {categoryCount}" + Environment.NewLine + error;
                    test1++;
                    //throw new Exception(test1.ToString() + Environment.NewLine + test2.ToString());
                }

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.Now.AddHours(1);
                //var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && product.adult_certification.Equals("F") && product.buy_limit_by_product.Equals("F"));
                var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && !(product.product_name.Contains("결제") || product.product_name.Contains("개인"))).DistinctBy(product => product.product_no);
                cache.Set(mallId, filteredProducts, policy);
                return filteredProducts;
            }
            else
            {
                return (IEnumerable<Product>)products;
            }



            //throw new Exception($"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {products.Count()}" + Environment.NewLine + urlcheck);
            //return products.Where(product => product.sold_out.Equals("F"));
        }


        private async Task<IEnumerable<Product>> GetAllProducts(string mallId)
        {
            var cache = MemoryCache.Default;

            //IEnumerable<Product> products = new List<Product>();
            var products = cache.Get(mallId);
            if (products == null)
            {
                IEnumerable<Product> tempProducts = new List<Product>();

                int limit = 100;


                int test1 = 0;
                int test2 = 0;
                string urlcheck = "";
                string categoryCountString = "";
                //foreach (Category category in categories)
                //{
                    int offset = 0;
                    int lastProductNo = 0;
                    //int categoryNo = category.category_no;

                    int categoryCount = 0;
                    string error = "";


                    //int productCount = await CountProducts(mallId, categoryNo);
                    //if (productCount.Equals(0))
                    //    continue;

                    int count = 0;
                    while (count > 0 || lastProductNo.Equals(0))
                    //while (count > 0 || offset.Equals(0))
                    {


                    //if (offset.Equals(5000))
                    //    break;

                    //string url = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit}&offset={offset}&fields=product_no,product_name,price,retail_price,summary_description,detail_imagesold_out";
                    //string url = $"https://{mallId}.cafe24api.com/api/v2/products?since_product_no={lastProductNo}&limit={limit}&display=T&selling=T&fields=product_no,product_name,price,retail_price,detail_image,sold_out";
                    string url = lastProductNo.Equals(0) ?
                        $"https://{mallId}.cafe24api.com/api/v2/products?order=asc&sort=created_date&limit={limit}&display=T&selling=T&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out" :
                        $"https://{mallId}.cafe24api.com/api/v2/products?since_product_no={lastProductNo}&limit={limit}&display=T&selling=T&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out";



                    var response = await client.GetAsync(url);
                        if (response.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            try
                            {
                                ProductListResult result = JsonConvert.DeserializeObject<ProductListResult>(response.Content.ReadAsStringAsync().Result);

                                tempProducts = tempProducts.Concat(result.products);

                                count = result.products.Count();
                                if (!count.Equals(0))
                                    lastProductNo = result.products.OrderBy(product => product.product_no).LastOrDefault().product_no;
                                else
                                    lastProductNo = 99999999;   // to exit loop when there is no products for the specific category
                                //offset += limit;


                                test2 += count;
                                urlcheck += url + Environment.NewLine;
                                categoryCount += count;


                                //Random random = new Random();
                                //int randomNumber = random.Next(0, 300);

                                //System.Threading.Thread.Sleep(randomNumber);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {tempProducts.Count()}" + Environment.NewLine + urlcheck + Environment.NewLine + response.Content.ReadAsStringAsync().Result);
                            }

                            var apiLimit = response.Headers.GetValues("X-Api-Call-Limit").FirstOrDefault();
                            if (!string.IsNullOrEmpty(apiLimit))
                            {
                                if (apiLimit.Contains("29"))
                                    System.Threading.Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            //offset = 5000;
                            error += response.Content.ReadAsStringAsync().Result + Environment.NewLine;
                            lastProductNo = 99999999;  // to exit loop

                        }



                    }

                    //categoryCountString += $"{category.category_no}/{category.category_name}: {categoryCount}" + Environment.NewLine + error;
                    test1++;
                    //throw new Exception(test1.ToString() + Environment.NewLine + test2.ToString());
                //}



                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.Now.AddHours(1);
                //var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && product.adult_certification.Equals("F") && product.buy_limit_by_product.Equals("F"));
                var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && !product.product_name.Contains("결제") && !product.product_name.Contains("개인")).DistinctBy(product => product.product_no);
                cache.Set(mallId, filteredProducts, policy);
                return filteredProducts;
            }
            else
            {
                return (IEnumerable<Product>)products;
            }



            //throw new Exception($"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {products.Count()}" + Environment.NewLine + urlcheck);
            //return products.Where(product => product.sold_out.Equals("F"));
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

            int test = 0;
            string test2 = "";

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

                    //throw new Exception(response.Content.ReadAsStringAsync().Result + " | " + url);
                }
                test++;
                test2 += "|" + url;
            }

            //throw new Exception(test.ToString() + Environment.NewLine + test2 );
            return categories.Where(category => !category.category_name.Contains("\uacb0\uc81c") && !category.category_name.Contains("\ucfe0\ud3f0") && !category.category_name.Contains("\uc0ac\uc6a9\uae08\uc9c0") && !category.category_name.Contains("\uc801\ub9bd\uae08") && !category.category_name.Contains("\uac1c\uc778"));
            
        }

        private async Task<int> CountProducts(string mallId, int categoryNo)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
            var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/products/count?category={categoryNo}&display=T&selling=T&adult_certification=F&buy_limit_type=F");
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                dynamic jsonResult = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                return jsonResult.count;
            }

            return 0;
        }

        //private async Task<CategoryListResult> GetAllCategories(string mallId)
        //{
        //    HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        //    var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/categories?limit=100&fields=category_no,category_name,full_category_no,full_category_name");
        //    if (response.StatusCode.Equals(HttpStatusCode.OK))
        //    {
        //        CategoryListResult categories = JsonConvert.DeserializeObject<CategoryListResult>(response.Content.ReadAsStringAsync().Result);
        //        return categories;
        //    }

        //    return null;
        //}

        //private async Task<int> CountCategories(string mallId)
        //{
        //    //int count = 0;

        //    HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        //    var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/categories/count");
        //    if (response.StatusCode.Equals(HttpStatusCode.OK))
        //    {
        //        dynamic jsonResult = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
        //        return jsonResult.count;
        //    }

        //    return 0;
        //}
    }
}