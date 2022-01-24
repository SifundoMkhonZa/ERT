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
    public class EshopsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Eshops
        public ActionResult Index()
        {
            var eshops = db.Eshops.Include(e => e.Supplier);
            return View(eshops.ToList());
        }
        [ChildActionOnly]
        public ActionResult Myshops()
        {
            var eshops = db.Eshops.Include(e => e.Supplier);
            return PartialView(eshops.ToList());
       
        }
        public ActionResult RatingDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eshop eshop = db.Eshops.Find(id);
            if (eshop == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArticleId = id.Value;

            var comments = db.Comments.Where(d => d.Shop_Id.Equals(id.Value)).ToList();
            ViewBag.Comments = comments;

            var ratings = db.Comments.Where(d => d.Shop_Id.Equals(id.Value)).ToList();
            if (ratings.Count() > 0)
            {
                var ratingSum = ratings.Sum(d => d.Rating.Value);
                ViewBag.RatingSum = ratingSum;
                var ratingCount = ratings.Count();
                ViewBag.RatingCount = ratingCount;
            }
            else
            {
                ViewBag.RatingSum = 0;
                ViewBag.RatingCount = 0;
            }

            return View(eshop);
        }
        // GET: Eshops/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eshop eshop = db.Eshops.Find(id);
            if (eshop == null)
            {
                return HttpNotFound();
            }
            return View(eshop);
        }

        // GET: Eshops/Create
        public ActionResult Create(string id)
        {
            var eshop = new Eshop();
            eshop.Supplier_Id = id;
            ViewBag.Supplier_Id = new SelectList(db.Suppliers, "Supplier_Id", "Supplier_Name");
            return View(eshop);
        }

        // POST: Eshops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Shop_Id,Shop_Name,Shop_Image,Supplier_Id")] Eshop eshop, HttpPostedFileBase filelist)
        {
            if (ModelState.IsValid)
            { 
                eshop.Shop_Image = ConvertToBytes(filelist);
                db.Eshops.Add(eshop);
                db.SaveChanges();
                return RedirectToAction("Successfull", "Suppliers");
            }

            ViewBag.Supplier_Id = new SelectList(db.Suppliers, "Supplier_Id", "Supplier_Name", eshop.Supplier_Id);
            return View(eshop);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        // GET: Eshops/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eshop eshop = db.Eshops.Find(id);
            if (eshop == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supplier_Id = new SelectList(db.Suppliers, "Supplier_Id", "Supplier_Name", eshop.Supplier_Id);
            return View(eshop);
        }

        // POST: Eshops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Shop_Id,Shop_Name,Shop_Image,Supplier_Id")] Eshop eshop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eshop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Supplier_Id = new SelectList(db.Suppliers, "Supplier_Id", "Supplier_Name", eshop.Supplier_Id);
            return View(eshop);
        }

        // GET: Eshops/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eshop eshop = db.Eshops.Find(id);
            if (eshop == null)
            {
                return HttpNotFound();
            }
            return View(eshop);
        }

        // POST: Eshops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Eshop eshop = db.Eshops.Find(id);
            db.Eshops.Remove(eshop);
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
