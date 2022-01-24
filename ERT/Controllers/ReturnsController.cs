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

namespace ERT.Controllers
{
    public class ReturnsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Returns
        public ActionResult Index()
        {
            var returns = db.Returns.Include(b => b.Order);
            return View(returns.ToList());
        }

        // GET: Returns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Return @return = db.Returns.Find(id);
            if (@return == null)
            {
                return HttpNotFound();
            }
            return View(@return);
        }

        // GET: Returns/Create
        public ActionResult Create(int id)
        {
            var @return = new Return();
            @return.OrderId = id;
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username");
            return View(@return);
        }

        // POST: Returns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Return_Id,Return_Comment,Item_Image,Return_Refund,OrderId")] Return @return, HttpPostedFileBase filelist)
        {
            if (ModelState.IsValid)
            {
                @return.Item_Image = ConvertToBytes(filelist);
                var status = db.Orders.Find(@return.OrderId);
                status.OrderStatus.Status_Name = "Return";
                db.Entry(status).State = EntityState.Modified;
                db.Returns.Add(@return);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", @return.OrderId);
            return View(@return);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        // GET: Returns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Return @return = db.Returns.Find(id);
            if (@return == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", @return.OrderId);
            return View(@return);
        }

        // POST: Returns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Return_Id,Return_Comment,Item_Image,Return_Refund,OrderId")] Return @return)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@return).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", @return.OrderId);
            return View(@return);
        }

        // GET: Returns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Return @return = db.Returns.Find(id);
            if (@return == null)
            {
                return HttpNotFound();
            }
            return View(@return);
        }

        // POST: Returns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Return @return = db.Returns.Find(id);
            db.Returns.Remove(@return);
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
