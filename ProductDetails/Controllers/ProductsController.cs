using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using ProductDetails.Models;
using System.Configuration;
namespace ProductDetails.Controllers
{
    public class ProductsController : Controller
    {
        string filepath = ConfigurationManager.AppSettings["MyFilePath"]; //web.Config file
        static List<ProductModel> products;
        public static string FilePath;
        public ProductsController()
        {  
            FilePath = System.Web.Hosting.HostingEnvironment.MapPath(filepath);
            products = new List<ProductModel>();
        }

        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductModel model)
        {
            products.Add(model);
            using (StreamWriter writer = new StreamWriter(FilePath, true))
            {
                writer.WriteLine($"{model.ProductId},{model.ProductName},{model.ProductPrice}");
            }
            return RedirectToAction("AddProduct");
        }
        public List<ProductModel> ReadFromFile()
        {
            List<ProductModel> productlist = new List<ProductModel>();

            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] words = line.Split(',');

                        int pid = int.Parse(words[0]);
                        string pname = words[1];
                        double price = double.Parse(words[2]);

                        ProductModel product = new ProductModel { ProductId = pid, ProductName = pname, ProductPrice = price };
                        productlist.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from file: {ex.Message}");
            }

            return productlist;
        }
        public ActionResult GetProducts()
        {
            List<ProductModel> productlist = new List<ProductModel>();
            try
            {
                if (!System.IO.File.Exists(FilePath))
                {
                    return Content("File not found");
                }

                products = ReadFromFile();
                return View(products);
            }
            catch (Exception ex)
            {
                return Content($"Error reading from file: {ex.Message}");
            }
        }

        public ActionResult AddToCart(int id)
        {
            TempData["td_id"] = id;
            TempData.Keep();
            ViewBag.id = id;
            return View(ViewBag.id);
        }

        [HttpPost]
        public ActionResult AddToCart(int productqty,string address)
        {
            int selectedid = Convert.ToInt32(TempData["td_id"]);
            List<ProductModel> productlist = new List<ProductModel>();
            productlist = ReadFromFile();
            ProductModel productid;
            productid = productlist.Find(p => p.ProductId == selectedid);
            try
            {
                if (productid != null)
                {
                         double amt= productqty * productid.ProductPrice;
                        TempData["Total_Price"] = amt;
                        TempData["addr"] = address;
                        TempData.Keep();
                        return RedirectToAction("PaymentMode");//
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return Content($"Error processing request: {ex.Message}");
            }
        }

        public ActionResult PaymentMode()
        {
            ViewBag.TotalAmount = TempData["Total_Price"];
            ViewBag.Deliver = TempData["addr"];
            return View();
        }
    }
}
