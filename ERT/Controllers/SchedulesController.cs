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
    public class SchedulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Schedules
        public ActionResult DriverView()
        {
            var uid = User.Identity.GetUserId();
            var schedules = db.Schedules.Include(s => s.Driver).Include(s => s.Order).Where(b => b.Order.OrderStatus.Status_Name == "Assigned to Driver" && b.Driver_ID == uid);
            return View(schedules.ToList());
        }
        public ActionResult DriverDeliveryList()
        {
            var uid = User.Identity.GetUserId();
            var schedules = db.Schedules.Include(s => s.Driver).Include(s => s.Order).Where(b => b.Order.OrderStatus.Status_Name == "Expected to Ship" && b.Driver_ID == uid);
            return View(schedules.ToList());
        }
        public ActionResult EmployeeView()
        {
            var uid = User.Identity.GetUserId();
            var schedules = db.Schedules.Include(s => s.Driver).Include(s => s.Order).Where(b => b.Order.OrderStatus.Status_Name == "At the warehouse");
            return View(schedules.ToList());
        }

        // GET: Schedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            var getId = (from i in db.Schedules
                         where i.Schedule_ID == id
                         select i.OrderId).Single();
            Order order = new Order();
            order.UpdateStatus(getId, "Out to collect");
            return View(schedule);
        }
   
        // GET: Schedules/Details/5
        public ActionResult DistanceCalculation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }
        // GET: Schedules/Create
        public ActionResult Create(int id)
        {
            Schedule schedule = new Schedule();
            schedule.OrderId = id;
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name");
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username");
            return View(schedule);
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Schedule_ID,Schedule_Date,OrderId,Driver_ID")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order();
                schedule.Schedule_Date = DateTime.Now;
                db.Schedules.Add(schedule);
                db.SaveChanges();
                order.UpdateStatus(schedule.OrderId, "Assigned to Driver");
                return RedirectToAction("AdminSchedules", "Checkout");
            }

            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", schedule.Driver_ID);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", schedule.OrderId);
            return View(schedule);
        }

        // GET: Schedules/Edit/5
        public ActionResult Edit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }


          
         
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", schedule.Driver_ID);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", schedule.OrderId);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Schedule_ID,Schedule_Date,OrderId,Driver_ID")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order(); 
                var getId = (from i in db.Schedules
                             where i.Schedule_ID == schedule.Schedule_ID
                            select i.OrderId).SingleOrDefault();
                schedule.OrderId = getId;
                db.Entry(schedule).State = EntityState.Modified;
                db.SaveChanges();
                order.UpdateStatus(getId, "Expected to Ship");
                var getclientEmal = (from i in db.Orders
                                     where i.OrderId == schedule.OrderId
                                     select i.Email).FirstOrDefault();
                var getclientName = (from i in db.Orders
                                     where i.OrderId == schedule.OrderId
                                     select i.FirstName).FirstOrDefault();
                var getlastName = (from i in db.Orders
                                   where i.OrderId == schedule.OrderId
                                   select i.LastName).FirstOrDefault();
    

                ViewBag.Body = $"Hi " + getclientName + "<br/>" +
      $"Your Package is at the warehouse, but it is being scheduled to be deliver on the {schedule.Schedule_Date}. Please continues to track your order here: https://localhost:44340/Checkout/TrackOrder/ . Thank you for choosing us." +
      $"<br/>" +
      $"AB CARGO";
                Email email = new Email();
                email.Gmail("Order delivery Information", ViewBag.Body, getclientEmal);
                return RedirectToAction("EmployeeView");
            }
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", schedule.Driver_ID);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", schedule.OrderId);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedule schedule = db.Schedules.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schedule schedule = db.Schedules.Find(id);
            db.Schedules.Remove(schedule);
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
