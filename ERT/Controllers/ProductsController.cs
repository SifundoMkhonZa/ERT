using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERT.Models;
using Microsoft.AspNet.Identity;

namespace ERT.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        public ActionResult Index(int id)
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Eshop).Where(p=>p.Shop_Id==id);
            return View(products.ToList());
        }
        public ActionResult ShopItems()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Eshop);
            return View(products.ToList());
        }
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [Authorize]
        // GET: Products/Create
        public ActionResult Create()
        {
            var uid = User.Identity.GetUserId();
            var product = new Product();
            var findShopId = (from i in db.Eshops
                              where i.Supplier_Id == uid
                              select i.Shop_Id).FirstOrDefault();
            product.Shop_Id = findShopId;
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            ViewBag.Shop_Id = new SelectList(db.Eshops, "Shop_Id", "Shop_Name");
            return View(product);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_id,Product_Name,Price,Image,CategoryId,Shop_Id")] Product product, HttpPostedFileBase filelist)
        {
            var uid = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                product.Image = ConvertToBytes(filelist);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("ShopItems");
            }

            ViewBag.CategoryId = new SelectList(db.Categories.Where(o=>o.Supplier_Id == uid), "CategoryId", "Name", product.CategoryId);
            ViewBag.Shop_Id = new SelectList(db.Eshops, "Shop_Id", "Shop_Name", product.Shop_Id);
            return View(product);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            ViewBag.Shop_Id = new SelectList(db.Eshops, "Shop_Id", "Shop_Name", product.Shop_Id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_id,Product_Name,Price,Image,CategoryId,Shop_Id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            ViewBag.Shop_Id = new SelectList(db.Eshops, "Shop_Id", "Shop_Name", product.Shop_Id);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
