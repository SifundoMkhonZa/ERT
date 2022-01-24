using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERT.Models;

namespace ERT.Controllers
{
    public class CartCopiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CartCopies
        public ActionResult Index(int id)
        {
            var cartCopies = db.CartCopies.Include(c => c.Order).Include(c => c.Product).Where(c=>c.OrderId == id);
            return View(cartCopies.ToList());
        }
       
        // GET: CartCopies/Details/5
        public ActionResult Details(int? id)
        {
            var copy = (from o in db.CartCopies
                        where o.OrderId == id
                        select o.RecordId).FirstOrDefault();
            if (copy == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comments = db.CartCopies.Include(c => c.Order).Include(c => c.Product).Where(c => c.OrderId == id);
            ViewBag.Comments = comments;
            CartCopy cartCopy = db.CartCopies.Find(copy);
            if (cartCopy == null)
            {
                return HttpNotFound();
            }
            return View(cartCopy);
        }
        public ActionResult ViewPDF(int id)
        {
            var report = new Rotativa.ActionAsPdf("Details", new { id = id }) { FileName = "Wabill.pdf" };
            return report;

            //return new ViewAsPdf("Invoice", new { id = id });
        }

        // GET: CartCopies/Create
        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username");
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name");
            return View();
        }

        // POST: CartCopies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RecordId,CartId,Count,DateCreated,OrderId,Product_id")] CartCopy cartCopy)
        {
            if (ModelState.IsValid)
            {
                db.CartCopies.Add(cartCopy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", cartCopy.OrderId);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", cartCopy.Product_id);
            return View(cartCopy);
        }

        // GET: CartCopies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
;
            CartCopy cartCopy = db.CartCopies.Find(id);
            if (cartCopy == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", cartCopy.OrderId);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", cartCopy.Product_id);
            return View(cartCopy);
        }

        // POST: CartCopies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RecordId,CartId,Count,DateCreated,OrderId,Product_id")] CartCopy cartCopy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cartCopy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", cartCopy.OrderId);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", cartCopy.Product_id);
            return View(cartCopy);
        }

        // GET: CartCopies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartCopy cartCopy = db.CartCopies.Find(id);
            if (cartCopy == null)
            {
                return HttpNotFound();
            }
            return View(cartCopy);
        }

        // POST: CartCopies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CartCopy cartCopy = db.CartCopies.Find(id);
            db.CartCopies.Remove(cartCopy);
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
