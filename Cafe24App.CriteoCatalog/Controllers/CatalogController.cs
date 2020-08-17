using Cafe24App.CriteoCatalog.Models;
using Cafe24App.CriteoCatalog.Models.Cafe24;
using Cafe24App.CriteoCatalog.Models.Criteo;
using MoreLinq;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
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
        //Dictionary<string, Task> taskDictionary = new Dictionary<string, Task>();

        public CatalogController()
        {
            client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        }

        [Route("all")]
        public async Task<ActionResult> Index()
        {
            string output = string.Empty;

            string[] installedMalls = ReadInstalledMalls();


            //return Content(string.Join(",", installedMalls));
            //return Content(installedMalls.Length.ToString());
            //foreach (string mallId in installedMalls)
            //{
            //    Stopwatch stopwatch = new Stopwatch();
            //    stopwatch.Start();
            //    string timeTaken = await GenerateCatalog(mallId);
            //    stopwatch.Stop();

            //    output += $"Time elapsed for {mallId}: {stopwatch.Elapsed}" + Environment.NewLine;
            //}

            //List<Task> taskList = new List<Task>();
            foreach (string mallId in installedMalls)
            {
                var sessionObj = Session[mallId];
                if (sessionObj != null)
                {
                    TaskStatus taskStatus = (TaskStatus)sessionObj;
                    if (taskStatus.Equals(TaskStatus.RanToCompletion) || taskStatus.Equals(TaskStatus.Faulted) || taskStatus.Equals(TaskStatus.Canceled))
                    {
                        Task currentTask = Task.Run(() => GenerateCatalog(mallId)).ContinueWith(task => UpdateSession(mallId, task));
                        Session[mallId] = currentTask.Status;
                        output += $"{mallId}: Previous task was {taskStatus}. Generate catalog task re-created and triggered{Environment.NewLine}";
                    }
                    else
                    {
                        output += $"{mallId}: Generate catalog task is still running. Last successful generation was {GetLastSuccessfulGeneration(mallId).ToString("yyyy-MM-dd HH:mm:ss")}{Environment.NewLine}";
                    }
                }
                else
                {
                    Task currentTask = Task.Run(() => GenerateCatalog(mallId)).ContinueWith(task => UpdateSession(mallId, task));
                    Session[mallId] = currentTask.Status;
                    output += $"{mallId}: Generate catalog task created and triggered{Environment.NewLine}";
                }
                

                //if (!taskDictionary.ContainsKey(mallId))
                //{
                //    Task currentTask = Task.Run(() => GenerateCatalog(mallId));
                //    taskDictionary[mallId] = currentTask;
                //    output += $"{mallId}: Generate catalog task created and triggered{Environment.NewLine}";
                //}
                //else
                //{
                //    Task currentTask = taskDictionary[mallId];
                //    if (currentTask.IsCompleted)
                //    {
                //        string previousTaskStatus = currentTask.Status.ToString();

                //        currentTask = Task.Run(() => GenerateCatalog(mallId));
                //        taskDictionary[mallId] = currentTask;
                //        output += $"{mallId}: Previous task was {previousTaskStatus}. Generate catalog task re-created and triggered{Environment.NewLine}";
                //    }
                //    else
                //    {
                //        output += $"{mallId}: Generate catalog task is already running";
                //    }
                //}
                
            }

            return Content(output);

            //if (string.IsNullOrEmpty(output))
            //    //return Task.FromResult(Content("No installed malls"));
            //    return Content("No installed malls");
            //else
            //    //return Task.FromResult(Content(output));
            //    return Content(output);
        }

        // GET: Catalog
        [Route("{mallId}")]
        public async Task<ActionResult> Index(string mallId, string generate = "0")
        {
            if (!string.IsNullOrEmpty(mallId))
            {
                if (generate.Equals("1"))
                {
                    //var allCategories = GetAllCategories(mallId);
                    //var productListResult = await GetAllProducts(mallId);
                    //var categoriesCount = await CountCategories(mallId);
                    //var productsCount = await CountProducts(mallId);


                    //var topCategories = await GetTopCategories(mallId);
                    //var allProducts = await GetAllProducts(mallId, topCategories.Where(x => x.category_no.Equals(3839)));
                    //var allProducts = await GetAllProducts(mallId, topCategories);



                    //var allProducts = GetAllProducts(mallId);

                    //var leafCategories = FindLeafCategories(await allCategories);
                    //var productCategoryList = GetAllProductCategories(mallId, leafCategories);

                    //var criteoCatalogs = ConvertToCriteoCatalogs(mallId, await allProducts, await productCategoryList);





                    //string cat = "";
                    //int totalCount = 0;
                    //foreach (Category category in leafCategories)
                    //{
                    //    int catCount = await CountProducts(mallId, category.category_no);
                    //    totalCount += catCount;
                    //    cat += $"{category.category_no} / {category.category_name}: {catCount}" + Environment.NewLine;
                    //}
                    //throw new Exception(cat + $"total: {totalCount}");


                    //List<Catalog> criteoCatalogs = new List<Catalog>();
                    //string baseURL = string.Format("{0}://{1}", new Uri(allProducts.FirstOrDefault().detail_image).Scheme, new Uri(allProducts.FirstOrDefault().detail_image).Host);
                    //var criteoCatalogs = allProducts.Select(async product => new Catalog
                    //{
                    //    id = product.product_no.ToString(),
                    //    title = product.product_name,
                    //    price = product.price,
                    //    sale_price = product.retail_price,
                    //    image_link = product.detail_image,
                    //    link = string.Format("{0}/product/detail.html?product_no={1}", baseURL, product.product_no),
                    //    categoryid1 = await GetProductCategories(mallId, product.product_no)
                    //});

                    //var catalog = new Catalog
                    //{
                    //    id = allProducts.ElementAt(0).product_no.ToString(),
                    //    title = allProducts.ElementAt(0).product_name,
                    //    price = allProducts.ElementAt(0).price,
                    //    sale_price = allProducts.ElementAt(0).retail_price,
                    //    image_link = allProducts.ElementAt(0).detail_image,
                    //    link = string.Format("{0}/product/detail.html?product_no={1}", baseURL, allProducts.ElementAt(0).product_no),
                    //    categoryid1 = await GetProductCategories(mallId, allProducts.ElementAt(0).product_no)
                    //};





                    //string csvString = CsvSerializer.SerializeToString<List<Task<Catalog>>>(criteoCatalogs.ToList());
                    //string csvString = CsvSerializer.SerializeToString<IEnumerable<Category>>(await allCategories);
                    //string csvString = CsvSerializer.SerializeToString<IEnumerable<Catalog>>(criteoCatalogs);

                    //string csvString = CsvSerializer.SerializeToString<Catalog>(catalog);
                    //string csvString = CsvSerializer.SerializeToString<IEnumerable<Category>>(leafCategories);
                    //string csvString = CsvSerializer.SerializeToString<IEnumerable<ProductCategory>>(await criteoCatalogs);




                    //string path = WriteFile(mallId, csvString);



                    string path = await GenerateCatalog(mallId);
                    return Content($"Exported to {path}");




                    //return File(new System.Text.UTF8Encoding().GetBytes(csvString), "text/csv", "oneeight.csv");
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
                else
                {
                    string csvString = ReadCatalog(mallId);
                    return File(new System.Text.UTF8Encoding().GetBytes(csvString), "text/csv", $"{mallId}.csv");
                }
            }

            return Content("MallId cannot be nulll or empty");
        }

        private void UpdateSession(string mallId, Task<string> task)
        {
            Session[mallId] = task.Status;
        }

        //private async void GenerateCatalog(string mallId)
        private async Task<string> GenerateCatalog(string mallId)
        {
            var allCategories = GetAllCategories(mallId);
            var allProducts = GetAllProducts(mallId);

            var leafCategories = FindLeafCategories(await allCategories);
            var productCategoryList = GetAllProductCategories(mallId, leafCategories);

            var criteoCatalogs = ConvertToCriteoCatalogs(mallId, await allProducts, await productCategoryList);

            string csvString = CsvSerializer.SerializeToString<IEnumerable<Catalog>>(criteoCatalogs);
            string path = WriteCatalog(mallId, csvString);

            return path;
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

        private string ReadCatalog(string mallId)
        {
            string path = Path.Combine(Server.MapPath("~/Catalogs"));
            string filename = $"{mallId}.csv";

            using (StreamReader sr = new StreamReader(Path.Combine(path, filename)))
            {
                // Read the stream to a string, and write the string to the console.
                string line = sr.ReadToEnd();
                return line;
            }
        }

        private string WriteCatalog(string mallId, string csvString)
        {
            string path = Path.Combine(Server.MapPath("~/Catalogs"));
            string filename = $"{mallId}.csv";

            Directory.CreateDirectory(path);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, filename), false))
            {
                outputFile.WriteLine(csvString);
            }

            return Path.Combine(path, filename);
        }

        private IEnumerable<Catalog> ConvertToCriteoCatalogs(string mallId, IEnumerable<Product> products, IEnumerable<ProductCategory> productCategories)
        {
            var cache = MemoryCache.Default;

            var catalogs = cache.Get($"{mallId}-catalogs");
            if (catalogs == null)
            {
                IEnumerable<Catalog> criteoCatalogs = new List<Catalog>();
                string baseURL = string.Format("{0}://{1}", new Uri(products.FirstOrDefault().detail_image).Scheme, new Uri(products.FirstOrDefault().detail_image).Host);

                var test = productCategories;
                criteoCatalogs = products.Select(product => new Catalog
                {
                    id = product.product_no.ToString(),
                    title = product.product_name,
                    price = product.retail_price,
                    sale_price = product.price,
                    image_link = product.detail_image,
                    link = string.Format("{0}/product/detail.html?product_no={1}", baseURL, product.product_no),
                    categoryid1 = productCategories.Where(productCategory => productCategory.product_no.Equals(product.product_no)).FirstOrDefault()?.category.GetCategory1(),
                    categoryid2 = productCategories.Where(productCategory => productCategory.product_no.Equals(product.product_no)).FirstOrDefault()?.category.GetCategory2(),
                    categoryid3 = productCategories.Where(productCategory => productCategory.product_no.Equals(product.product_no)).FirstOrDefault()?.category.GetCategory3()
                });

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.Now.AddHours(1);
                cache.Set($"{mallId}-catalogs", criteoCatalogs, policy);

                return criteoCatalogs;
            }
            else
                return (IEnumerable<Catalog>)catalogs;
        }

        private async Task<IEnumerable<ProductCategory>> GetAllProductCategories(string mallId, IEnumerable<Category> leafCategories)
        {
            int totalCount = leafCategories.Count();

            var cache = MemoryCache.Default;

            var productCategories = cache.Get($"{mallId}-pc");
            if (productCategories == null)
            {
                List<ProductCategory> tempProductCategories = new List<ProductCategory>();
                //List<int> tempProductNoList = new List<int>();


                //int currentIndex = 1;
                //int limit1 = 200;
                //int limit2 = 100;
                int limit = 200;

                //string url1 = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit1}&offset={offset}&fields=product_no";
                //string url2 = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit}&offset={offset}&fields=product_no";

                int test1 = 0;
                int test2 = 0;
                string urlcheck = "";
                string categoryCountString = "";
                foreach (Category category in leafCategories)
                {
                    urlcheck = "";

                    int offset = 0;
                    //int lastProductNo = 0;
                    int categoryNo = category.category_no;

                    int categoryCount = 0;
                    string error = "";


                    int productCount = await CountProducts(mallId, categoryNo);
                    if (productCount.Equals(0))
                        continue;


                    //string url1 = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit1}&offset=$offset$&fields=product_no";
                    //string url2 = $"https://{mallId}.cafe24api.com/api/v2/products?category={categoryNo}&display=T&selling=T&limit={limit2}&offset=$offset$&fields=product_no";
                    int count = 0;
                    //while (count > 0 || lastProductNo.Equals(0))
                    while (count > 0 || offset.Equals(0))
                    {


                        if (offset.Equals(8000))
                            break;

                        //string url = "";
                        //if (currentIndex.Equals(1))
                        //    url = url1.Replace("$offset$", offset.ToString());
                        //else if (currentIndex.Equals(2))
                        //    url = url2.Replace("$offset$", offset.ToString());

                        string url = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit}&offset={offset}&fields=product_no";
                        //string url = $"https://{mallId}.cafe24api.com/api/v2/products?limit={limit}&offset={offset}&category={categoryNo}&display=T&selling=T&fields=product_no,product_name,price,retail_price,detail_image,sold_out";
                        //string url = lastProductNo.Equals(0) ?
                        //    $"https://{mallId}.cafe24api.com/api/v2/products?order=asc&sort=created_date&limit={limit}&category={categoryNo}&display=T&selling=T&adult_certification=F&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out" :
                        //    $"https://{mallId}.cafe24api.com/api/v2/products?since_product_no={lastProductNo}&limit={limit}&category={categoryNo}&display=T&selling=T&adult_certification=F&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out";

                        var response = await client.GetAsync(url);
                        if (response.StatusCode.Equals(HttpStatusCode.OK))
                        {
                            try
                            {
                                ProductListResult result = JsonConvert.DeserializeObject<ProductListResult>(response.Content.ReadAsStringAsync().Result);
                                //dynamic result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                                //foreach (dynamic product in result.products)
                                foreach (Product product in result.products)
                                {
                                    int product_no = product.product_no;
                                    if (!tempProductCategories.Exists(productCategory => productCategory.product_no.Equals(product_no)))
                                    {
                                        tempProductCategories.Add(new ProductCategory { product_no = product_no, category = category });
                                    }
                                }

                                count = result.products.Count();
                                offset += limit;
                                //if (currentIndex.Equals(1))
                                //    offset += limit1;
                                //else if (currentIndex.Equals(2))
                                //    offset += limit2;




                                test2 += count;
                                urlcheck += url + Environment.NewLine;
                                categoryCount += count;

                                System.Threading.Thread.Sleep(100);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"{ex.ToString()}" + Environment.NewLine + $"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {tempProductCategories.Count()}" + Environment.NewLine + urlcheck + Environment.NewLine + response.Content.ReadAsStringAsync().Result);
                            }

                            var apiLimitString = response.Headers?.GetValues("X-Api-Call-Limit").FirstOrDefault();
                            if (!string.IsNullOrEmpty(apiLimitString))
                            {


                                //int apiLimit = int.Parse(apiLimitString.Split('/')[0]);

                                urlcheck += "apiLimit: " + apiLimitString + Environment.NewLine;
                                //if (limit > 25)
                                //{
                                //    System.Threading.Thread.Sleep(1000);
                                //}

                                if (apiLimitString.Contains("28"))
                                    System.Threading.Thread.Sleep(1000);
                            }

                            if (count < limit)
                                break;
                        }
                        else
                        {
                            offset = 8000;
                            error += response.Content.ReadAsStringAsync().Result + Environment.NewLine;
                            //lastProductNo = 9999;  // to exit loop

                        }



                    }

                    categoryCountString += $"{category.category_no}/{category.category_name}: {categoryCount}" + Environment.NewLine + urlcheck +  Environment.NewLine + error;
                    test1++;

                    //throw new Exception($"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {tempProductCategories.Count()}");
                }

                


                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.Now.AddHours(1);
                //var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && product.adult_certification.Equals("F") && product.buy_limit_by_product.Equals("F"));
                //var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && !(product.product_name.Contains("결제") || product.product_name.Contains("개인"))).DistinctBy(product => product.product_no);
                cache.Set($"{mallId}-pc", tempProductCategories, policy);


                //throw new Exception("total category: " + totalCount + Environment.NewLine + $"categoryProcessed: " + test1.ToString());
                //throw new Exception("total category: " + totalCount + Environment.NewLine + $"categoryProcessed: " + test1.ToString() + Environment.NewLine + "total count: " + test2.ToString() + Environment.NewLine + categoryCountString);
                return tempProductCategories;
            }
            else
            {
                return (List<ProductCategory>)productCategories;
            }
        }

        //private async Task<string> GetProductCategories(string mallId, int productNo)
        //{
        //    List<string> categories = new List<string>();
        //    //string category = "";

        //    string url = $"https://{mallId}.cafe24api.com/api/v2/products/{productNo.ToString()}?fields=category";
        //    var response = await client.GetAsync(url);
        //    //var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/products?fields=product_no,product_name,price,retail_price,display,selling,summary_description,adult_certification,detail_image,small_image,sold_out,category");
        //    //var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/mains/2/products");
        //    if (response.StatusCode.Equals(HttpStatusCode.OK))
        //    {
        //        dynamic productCategory = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
        //        return productCategory.product.category[0]?.category_no;
        //    }

        //    var apiLimit = response.Headers.GetValues("X-Api-Call-Limit").FirstOrDefault();
        //    if (!string.IsNullOrEmpty(apiLimit))
        //    {
        //        if (apiLimit.Contains("29"))
        //            System.Threading.Thread.Sleep(1000);
        //    }

        //    return "";
        //}

        //private async Task<IEnumerable<Product>> GetAllProducts(string mallId, IEnumerable<Category> categories)
        //{
        //    var cache = MemoryCache.Default;

        //    //IEnumerable<Product> products = new List<Product>();
        //    var products = cache.Get(mallId);
        //    if (products == null)
        //    {
        //        IEnumerable<Product> tempProducts = new List<Product>();

        //        int limit = 100;


        //        int test1 = 0;
        //        int test2 = 0;
        //        string urlcheck = "";
        //        string categoryCountString = "";
        //        foreach (Category category in categories)
        //        {
        //            int offset = 0;
        //            //int lastProductNo = 0;
        //            int categoryNo = category.category_no;

        //            int categoryCount = 0;
        //            string error = "";


        //            int productCount = await CountProducts(mallId, categoryNo);
        //            if (productCount.Equals(0))
        //                continue;

        //            int count = 0;
        //            //while (count > 0 || lastProductNo.Equals(0))
        //            while (count > 0 || offset.Equals(0))
        //            {


        //                if (offset.Equals(5000))
        //                    break;

        //                //string url = $"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products?display_group=1&limit={limit}&offset={offset}&fields=product_no,product_name,price,retail_price,summary_description,detail_imagesold_out";
        //                string url = $"https://{mallId}.cafe24api.com/api/v2/products?limit={limit}&offset={offset}&category={categoryNo}&display=T&selling=T&fields=product_no,product_name,price,retail_price,detail_image,sold_out";
        //                //string url = lastProductNo.Equals(0) ?
        //                //    $"https://{mallId}.cafe24api.com/api/v2/products?order=asc&sort=created_date&limit={limit}&category={categoryNo}&display=T&selling=T&adult_certification=F&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out" :
        //                //    $"https://{mallId}.cafe24api.com/api/v2/products?since_product_no={lastProductNo}&limit={limit}&category={categoryNo}&display=T&selling=T&adult_certification=F&fields=product_no,product_name,price,retail_price,summary_description,detail_image,sold_out";

        //                var response = await client.GetAsync(url);
        //                if (response.StatusCode.Equals(HttpStatusCode.OK))
        //                {
        //                    try
        //                    {
        //                        ProductListResult result = JsonConvert.DeserializeObject<ProductListResult>(response.Content.ReadAsStringAsync().Result);

        //                        tempProducts = tempProducts.Concat(result.products);

        //                        count = result.products.Count();
        //                        //if (!count.Equals(0))
        //                        //    lastProductNo = result.products.OrderBy(product => product.product_no).LastOrDefault().product_no;
        //                        //else
        //                        //    lastProductNo = 9999;   // to exit loop when there is no products for the specific category
        //                        offset += limit;


        //                        test2 += count;
        //                        urlcheck += url + Environment.NewLine;
        //                        categoryCount += count;


        //                        //Random random = new Random();
        //                        //int randomNumber = random.Next(0, 300);

        //                        //System.Threading.Thread.Sleep(randomNumber);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        throw new Exception($"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {tempProducts.Count()}" + Environment.NewLine + urlcheck + Environment.NewLine + response.Content.ReadAsStringAsync().Result);
        //                    }

        //                    var apiLimit = response.Headers.GetValues("X-Api-Call-Limit").FirstOrDefault();
        //                    if (!string.IsNullOrEmpty(apiLimit))
        //                    {
        //                        if (apiLimit.Contains("29"))
        //                            System.Threading.Thread.Sleep(1000);
        //                    }
        //                }
        //                else
        //                {
        //                    offset = 5000;
        //                    error += response.Content.ReadAsStringAsync().Result + Environment.NewLine;
        //                    //lastProductNo = 9999;  // to exit loop

        //                }



        //            }

        //            categoryCountString += $"{category.category_no}/{category.category_name}: {categoryCount}" + Environment.NewLine + error;
        //            test1++;
        //            //throw new Exception(test1.ToString() + Environment.NewLine + test2.ToString());
        //        }

        //        CacheItemPolicy policy = new CacheItemPolicy();
        //        policy.AbsoluteExpiration = DateTime.Now.AddHours(1);
        //        //var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && product.adult_certification.Equals("F") && product.buy_limit_by_product.Equals("F"));
        //        var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && !(product.product_name.Contains("결제") || product.product_name.Contains("개인"))).DistinctBy(product => product.product_no);
        //        cache.Set(mallId, filteredProducts, policy);
        //        return filteredProducts;
        //    }
        //    else
        //    {
        //        return (IEnumerable<Product>)products;
        //    }



        //    //throw new Exception($"categoryProcessed: {test1}" + Environment.NewLine + categoryCountString + Environment.NewLine + $"totalProducts: {products.Count()}" + Environment.NewLine + urlcheck);
        //    //return products.Where(product => product.sold_out.Equals("F"));
        //}


        private async Task<IEnumerable<Product>> GetAllProducts(string mallId)
        {
            var cache = MemoryCache.Default;

            //IEnumerable<Product> products = new List<Product>();
            var products = cache.Get($"{mallId}-p");
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

                            var apiLimit = response.Headers?.GetValues("X-Api-Call-Limit").FirstOrDefault();
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
                var filteredProducts = tempProducts.Where(product => product.sold_out.Equals("F") && !string.IsNullOrEmpty(product.detail_image) && !product.product_name.Contains("결제") && !product.product_name.Contains("개인") && !product.product_name.Contains("배송비")).DistinctBy(product => product.product_no);
                cache.Set($"{mallId}-p", filteredProducts, policy);
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

        private IEnumerable<Category> FindLeafCategories(IEnumerable<Category> categories)
        {
            //List<Category> leafCategories = new List<Category>();

            string allParentCategoryNoString = string.Join(",", categories.ToArray().Select(category => $"'{category.parent_category_no}'"));

            return categories.Where(category => !allParentCategoryNoString.Contains($"'{category.category_no}'"));

            //return leafCategories;
        }

        private async Task<IEnumerable<Category>> GetAllCategories(string mallId)
        {
            List<Category> categories = new List<Category>();

            int limit = 100;
            int offset = 0;

            int test = 0;
            string test2 = "";

            int count = 0;
            while (count > 0 || offset.Equals(0))
            {
                string url = $"https://{mallId}.cafe24api.com/api/v2/categories?limit={limit}&offset={offset}&fields=category_no,category_name,parent_category_no,full_category_no,full_category_name";


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
            var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/products/count?category={categoryNo}&display=T&selling=T");
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                dynamic jsonResult = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                var apiLimitString = response.Headers?.GetValues("X-Api-Call-Limit").FirstOrDefault();
                if (!string.IsNullOrEmpty(apiLimitString))
                {
                    if (apiLimitString.Contains("28"))
                        System.Threading.Thread.Sleep(1000);
                }

                return jsonResult.count;
            }

            return 0;
        }

        //private async Task<int> CountCategory(string mallId, int categoryNo)
        //{
        //    HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("X-Cafe24-Client-Id", clientId);
        //    var response = await client.GetAsync($"https://{mallId}.cafe24api.com/api/v2/categories/{categoryNo}/products/count?display_group=1");
        //    if (response.StatusCode.Equals(HttpStatusCode.OK))
        //    {
        //        dynamic jsonResult = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
        //        return jsonResult.count;
        //    }

        //    return 0;
        //}

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