using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERT.Models;
using Microsoft.AspNet.Identity;

namespace ERT.Controllers
{
    public class Beneficiary_SignatureController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Beneficiary_Signature
        public ActionResult Index()
        {
            var beneficiary_Signature = db.Beneficiary_Signature.Include(b => b.Order);
            return View(beneficiary_Signature.ToList());
        }

        // GET: Beneficiary_Signature/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            if (beneficiary_Signature == null)
            {
                return HttpNotFound();
            }
            return View(beneficiary_Signature);
        }

        // GET: Beneficiary_Signature/Create
        public ActionResult Create(int id)
        {
            Beneficiary_Signature beneficiary_Signature = new Beneficiary_Signature();
            beneficiary_Signature.OrderId = id;
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username");
            return View(beneficiary_Signature);
        }

        // POST: Beneficiary_Signature/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Signaturee_ID,Sign_Date,MySignature,SignedBy,OrderId")] Beneficiary_Signature beneficiary_Signature)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order();
                beneficiary_Signature.Sign_Date = DateTime.Now;
                db.Beneficiary_Signature.Add(beneficiary_Signature);
                db.SaveChanges();
                var uid = User.Identity.GetUserId();
                var getstatus = (from i in db.Orders
                                 where i.OrderId == beneficiary_Signature.OrderId
                                 select i.OrderStatus.Status_Name).SingleOrDefault();
                var getDriverId = (from i in db.Schedules
                                   where i.OrderId == beneficiary_Signature.OrderId
                                   select i.Driver_ID).SingleOrDefault();

                var getClientId = (from i in db.Schedules
                                   where i.OrderId == beneficiary_Signature.OrderId
                                   select i.Order.Client_Id).SingleOrDefault();

                var getClientEmail = (from i in db.Schedules
                                   where i.OrderId == beneficiary_Signature.OrderId
                                   select i.Order.Client.Client_Email).SingleOrDefault();

                var getClientName = (from i in db.Schedules
                                      where i.OrderId == beneficiary_Signature.OrderId
                                      select i.Order.Client.Client_Name).SingleOrDefault();
                var getShopId = (from i in db.OrderDetails
                                     where i.OrderId== beneficiary_Signature.OrderId
                                     select i.Product.Eshop.Shop_Id).SingleOrDefault();

                if (getstatus == "Out to collect" && getDriverId == uid)
                {
                    order.UpdateStatus(beneficiary_Signature.OrderId, "Collected");
                    return RedirectToAction("DriverView", "Schedules");
                }

                if (getstatus == "Collected")
                {
                    order.UpdateStatus(beneficiary_Signature.OrderId, "At the warehouse");
                    return RedirectToAction("EmployeeView", "Schedules");
                }

                if (getstatus == "Expected to Ship" && getClientId == uid)
                {
                    order.UpdateStatus(beneficiary_Signature.OrderId, "Delivery by");
                    if (getstatus == "Delivery by") { 
                    ViewBag.Body = $"Hi " + getClientName + "<br/>" +
                      $"Congratulations!!! Please rate our services here https://grp20abcargo/Comments/Create/" + getShopId +
                     $"If you are not satisfied  with the state of your order please return here https://grp20abcargo/Returns/Create/" +beneficiary_Signature.OrderId +". Thank you for choosing us." +
                      $"<br/>" +
                      $"AB CARGO";
                    Email email = new Email();
                    email.Gmail("Order delivery Information", ViewBag.Body, getClientEmail);
                }
                    return RedirectToAction("DriverDeliveryList", "Schedules");
                }
            }

            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", beneficiary_Signature.OrderId);
            return View(beneficiary_Signature);
        }

        // GET: Beneficiary_Signature/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            if (beneficiary_Signature == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", beneficiary_Signature.OrderId);
            return View(beneficiary_Signature);
        }

        // POST: Beneficiary_Signature/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Signaturee_ID,Sign_Date,MySignature,SignedBy,OrderId")] Beneficiary_Signature beneficiary_Signature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beneficiary_Signature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", beneficiary_Signature.OrderId);
            return View(beneficiary_Signature);
        }

        // GET: Beneficiary_Signature/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            if (beneficiary_Signature == null)
            {
                return HttpNotFound();
            }
            return View(beneficiary_Signature);
        }

        // POST: Beneficiary_Signature/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            db.Beneficiary_Signature.Remove(beneficiary_Signature);
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
