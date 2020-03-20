using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAppEkaS2019.Models;
using System.Web.Mvc;
using PagedList;

namespace WebAppEkaS2019.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index(string sortOrder, string currentFilter1, string searchString1, string ProductCategory, int? page, int? pagesize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProductNameSortParm = String.IsNullOrEmpty(sortOrder) ? "productname_desc" : "";
            ViewBag.UnitPriceSortParm = sortOrder == "UnitPrice" ? "unitprice_desc" : "UnitPrice";

            if (searchString1 != null)
            {
                page = 1;
            }
            else
            {
                searchString1 = currentFilter1;
            }

            ViewBag.currentFilter1 = searchString1;

            northwindEntities db = new northwindEntities();

            var tuotteet = from p in db.Products
                           select p;

            if (!String.IsNullOrEmpty(searchString1))
            {
                tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1));
            }
            //Tuoteryhmällä nolla (tyhjän valinnan oletusarvo) ei voi hakea, joten se suljetaan pois, jotta tuotehaku kannasta toimisi oikein
            if (!String.IsNullOrEmpty(ProductCategory) && (ProductCategory != "0")) 
            {
                int para = int.Parse(ProductCategory);
                tuotteet = tuotteet.Where(p => p.CategoryID == para);
            }

            switch (sortOrder)
            {
                case "productname_desc":
                    tuotteet = tuotteet.OrderByDescending(p => p.ProductName);
                    break;
                case "UnitPrice":
                    tuotteet = tuotteet.OrderBy(p => p.UnitPrice);
                    break;
                case "UnitPrice_desc":
                    tuotteet = tuotteet.OrderByDescending(p => p.UnitPrice);
                    break;
                default:
                    tuotteet = tuotteet.OrderBy(p => p.ProductName);
                    break;
            }

            //db.Dispose();
            //return View(tuotteet.ToList());

            List<Categories> lstCategories = new List<Categories>();

            var categoryList = from cat in db.Categories
                             select cat;

            Categories tyhjaCategory = new Categories();
            tyhjaCategory.CategoryID = 0;
            tyhjaCategory.CategoryName = "";
            tyhjaCategory.CategoryIDCategoryName = "";
            lstCategories.Add(tyhjaCategory);

            foreach (Categories category in categoryList)
            {
                Categories yksiCategory = new Categories();
                yksiCategory.CategoryID = category.CategoryID;
                yksiCategory.CategoryName = category.CategoryName;
                yksiCategory.CategoryIDCategoryName = category.CategoryID.ToString() + " - " + category.CategoryName;
                //Taulun luokkamääritykseen Models-kansiossa piti lisätä tämä "uusi" kenttä = CategoryIDCategoryName
                lstCategories.Add(yksiCategory);
            }
            ViewBag.CategoryID = new SelectList(lstCategories, "CategoryID", "CategoryIDCategoryName", ProductCategory);

            int pageSize = (pagesize ?? 10); //Tämä palauttaa sivukoon taikka jos pagesize on null, niin palauttaa koon 10 riviä per sivu
            int pageNumber = (page ?? 1); //Tämä palauttaa sivunumeron taikka jos page on null, niin palauttaa numeron yksi
            return View(tuotteet.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ProdCards()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("login", "home");
            }
            else
            {
                northwindEntities db = new northwindEntities();
                List<Products> tuotteet = db.Products.ToList();
                db.Dispose();
                return View(tuotteet);
            }

        }

    }
}