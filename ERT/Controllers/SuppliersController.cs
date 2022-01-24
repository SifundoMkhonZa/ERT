using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ERT.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ERT.Controllers
{
    public class SuppliersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        // GET: Suppliers
        public ActionResult Index()
        {
            return View(db.Suppliers.Where(a=>a.Supplier_Contract == false).ToList());
        }
        public ActionResult FullScreen(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        public ActionResult Accept(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Supplier supplier = (from c in db.Suppliers
                                 where c.Supplier_Id == id
                                 select c).SingleOrDefault();
            supplier.Supplier_Contract = true;
            db.SaveChanges();
            if (supplier.Supplier_Contract == true)
                UserManager.AddToRole(supplier.Supplier_Id, "Supplier");


            //Emails emails = new Emails();
            //ViewBag.Subject = " Partnership Confirmation ";
            //ViewBag.Body = $"https://localhost:44340/Orders/Index/ {supplier.Supplier_Name}" + "<br/>" +
            //    $"Your application was successfully approved for Imfuyo Ranch supplier program." + "<br/>" +
            //    $"Access the system by using your credentials for your account settings." + "<br/>" +
            //    $"Thank you." + "<br/>" +
            //    $"Imfuyo Ranch Team.";
            //emails.SendConfirmation(ViewBag.Subject, ViewBag.Body);
          

            return RedirectToAction("Index");
        }
        public ActionResult Decline(string id)
        {
            var eshopId = (from i in db.Eshops
                           where i.Supplier_Id == id
                           select i.Shop_Id).FirstOrDefault();
            Eshop eshop = db.Eshops.Find(eshopId);
            db.Eshops.Remove(eshop);
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Suppliers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Suppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Supplier_Id,Supplier_Name,Supplier_Image,Supplier_LastName,Supplier_Address,Supplier_ContactNumber,Supplier_Email,Supplier_Contract,Supplier_Password,Supplier_Ducuments")] Supplier supplier, HttpPostedFileBase filelist, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (filelist != null && filelist.ContentLength > 0)
                {
                    supplier.Supplier_Image = ConvertToBytes(filelist);
                }
                if (upload != null && upload.ContentLength > 0)
                {
                    int filelength = upload.ContentLength;
                    Byte[] array = new Byte[filelength];
                    upload.InputStream.Read(array, 0, filelength);
                    supplier.Supplier_Ducuments = array;
                }
                var user = new ApplicationUser { UserName = supplier.Supplier_Email, Email = supplier.Supplier_Email };
                await UserManager.CreateAsync(user, supplier.Supplier_Password);
                supplier.Supplier_Id = user.Id;
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Create", "Eshops",new { id = supplier.Supplier_Id});
            }

            return View(supplier);
        }
        public ActionResult Successfull()
        {
            return View();
        }


        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        // GET: Suppliers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Supplier_Id,Supplier_Name,Supplier_Image,Supplier_LastName,Supplier_Address,Supplier_ContactNumber,Supplier_Email,Supplier_Contract,Supplier_Password,Supplier_Ducuments")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
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
