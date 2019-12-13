using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WebAppEkaS2019.Models;

namespace WebAppEkaS2019.Controllers
{
    public class ShippersController : Controller
    {
        northwindEntities db = new northwindEntities();
        // GET: Shippers
        public ActionResult Index()
        {
            var shipperit = db.Shippers.Include(s => s.Region);
            ViewBag.RiviLkm = db.Shippers.Count();
            return View(shipperit.ToList());
        }


        public ActionResult Edit(int? id)
        {
            //List<Region> Regioonat = db.Region.ToList();
            //ViewBag.Regioonat = Regioonat;

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Shippers shipperi = db.Shippers.Find(id);
            if (shipperi == null) return HttpNotFound();

            List<SelectListItem> regioonat = new List<SelectListItem>();
            foreach (Region regioona in db.Region)
            {
                regioonat.Add(new SelectListItem
                {
                    Value = regioona.RegionID.ToString(),
                    Text = regioona.RegionID.ToString() + " " + regioona.RegionDescription + " kukkuu"
                });
            }
                       
            ViewBag.RegionID = new SelectList(regioonat, "Value", "Text", shipperi.RegionID);
           // ViewBag.RegionID = new SelectList(db.Region, "RegionID", "RegionDescription", shipperi.RegionID);
            return View(shipperi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ShipperID,CompanyName,Phone,RegionID")] Shippers shipperi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipperi).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.RegionID = new SelectList(db.Region, "RegionID", "RegionDescription", shipperi.RegionID);
                return RedirectToAction("Index");
            }
            return View(shipperi);
        }

        public ActionResult Create()
        {
            ViewBag.RegionID = new SelectList(db.Region, "RegionID", "RegionDescription", null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShipperID, CompanyName, Phone, RegionID")] Shippers shipperi)
        {
            if (ModelState.IsValid)
            {
                db.Shippers.Add(shipperi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shipperi);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Shippers shipperi = db.Shippers.Find(id);
            if (shipperi == null) return HttpNotFound();
            return View(shipperi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            List<Orders> orders = db.Orders.Where(d => d.ShipVia == id).ToList();
            Shippers shipperi = db.Shippers.Find(id);
            if (orders.Count == 0) {
                ViewBag.Herja = "";
                db.Shippers.Remove(shipperi);
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                ViewBag.Herja = "Rahtaria ei voi poistaa, koska sillä on tilauksia!";
                return View(shipperi);
            }
        }
    }
}