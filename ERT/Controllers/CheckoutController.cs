using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERT.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PayFast;
using System.Net;
using System.Threading.Tasks;
using PayFast.AspNet;
using System.Data.Entity;
using System.Configuration;
using System.IO;
using ZXing;
using System.Drawing;
using System.Drawing.Imaging;

namespace ERT.Controllers
{
    //[Authorize]
    public class CheckoutController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Checkout

        public CheckoutController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }



        private string GenerateQRCode(string qrcodeText)
        {
            string folderPath = "~/Images/";
            string imagePath = "~/Images/QrCode.jpg";
            // create new Directory if not exist
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }

        public ActionResult Read()
        {
            return View(ReadQRCode());
        }

        private QRCode ReadQRCode()
        {
            QRCode barcodeModel = new QRCode();
            string barcodeText = "";
            string imagePath = "~/Images/QrCode.jpg";
            string barcodePath = Server.MapPath(imagePath);
            var barcodeReader = new BarcodeReader();
            //Decode the image to text
            var result = barcodeReader.Decode(new Bitmap(barcodePath));
            if (result != null)
            {
                barcodeText = result.Text;
            }
            return new QRCode() { QRCodeText = barcodeText, QRCodeImagePath = imagePath };
        }


        public ActionResult AddNew(int id)
        {
            QRCode objQR = new QRCode();
            objQR.OrderId = id;
            objQR.QRCodeText = "https://grp20abcargo.azurewebsites.net/Create/Beneficiary_Signature/" + objQR.OrderId;
            objQR.QRCodeImagePath = GenerateQRCode(objQR.QRCodeText);
            db.QRCodes.Add(objQR);
            db.SaveChanges();
            return RedirectToAction("OnceOff", new { id = objQR.OrderId });
        }
        public ActionResult SupplierOrders()
        {
            var uid = User.Identity.GetUserId();
            var details = db.OrderDetails.Include(b => b.Order).Where(b=>b.Order.OrderStatus.Status_Name == "Ordered" && b.Product.Eshop.Supplier_Id == uid);
            return View(details.ToList());
        }
        public ActionResult OrdersForReturn()
        {
            var uid = User.Identity.GetUserId();
            var details = db.OrderDetails.Include(b => b.Order).Where(b => b.Order.OrderStatus.Status_Name == "Return" && b.Product.Eshop.Supplier_Id == uid);
            return View(details.ToList());
        }

        public ActionResult AdminSchedules()
        {
            var details = db.OrderDetails.Include(b => b.Order).Where(b => b.Order.OrderStatus.Status_Name == "Ready for collection");
            return View(details.ToList());
        }
        //Driver assigned
        public ActionResult Process(int id)
        {
            Order order = new Order();
            Order upDatestatus = (from c in db.Orders
                                  where c.OrderId == id
                                  select c).SingleOrDefault();
            order.UpdateStatus(id, "Ready for collection");
            return RedirectToAction("SupplierOrders");
        }

        
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QRCode qRCode = db.QRCodes.Find(id);
            if (qRCode == null)
            {
                return HttpNotFound();
            }
            return View(qRCode);
        }

        // GET: Checkout/Details/5
        public ActionResult Success()
        {
            return View();
        }

        public ActionResult TrackOrder(string searchString)
        {


            var trackings = from s in db.Orders
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                trackings = trackings.Where(s =>
               s.Reference.ToUpper().Contains(searchString.ToUpper()));


                return View(trackings.ToList());
            }


            trackings = trackings.Where(s =>
                   s.Reference== null);


            return View(trackings.ToList());



        }
        // GET: Checkout/Create
        public ActionResult AddressAndPayment()
        {
            return View();
        }

        // POST: Checkout/Create
        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {

         
            var order = new Order();
            var cart = ShoppingCart.GetCart(this.HttpContext);
            TryUpdateModel(order);
            if (ModelState.IsValid)
            {
                var uid = User.Identity.GetUserId();
                    order.OrderDate = DateTime.Now;
                    order.Client_Id = uid;
                    order.Reference = RandomString(8);
                    order.Username = User.Identity.Name;
                    order.OrderStatus_ID = order.GetStatus("Ordered");
                    order.Total = cart.GetTotal();
                db.Orders.Add(order);
                db.SaveChanges();

                cart.CreateOrder(order);

                return RedirectToAction("AddNew", new { id = order.OrderId });
                
            }

            return View(order);

        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        // GET: Checkout/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Checkout/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Checkout/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Checkout/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region Payment
        #region Fields

        private readonly PayFastSettings payFastSettings;

        #endregion Fields

        public ActionResult Recurring()
        {
            var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            // Merchant Details
            recurringRequest.merchant_id = this.payFastSettings.MerchantId;
            recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
            recurringRequest.return_url = this.payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
            recurringRequest.notify_url = this.payFastSettings.NotifyUrl;
            // Buyer Details
            recurringRequest.email_address = "nkosi@finalstride.com";
            // Transaction Details
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Some details about the recurring option";
            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "drnendwandwe@gmail.com";
            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.Now;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;
            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";
            return Redirect(redirectUrl);
        }


        public ActionResult OnceOff(int id)
        {

            //var uid = User.Identity.GetUserId();
            //var appointments = db.Appointments.Include(a => a.Client).Where(x => x.ClientId == uid).Where(a => a.paymentstatus == false).Where(a => a.status == false);
            Order order = db.Orders.Find(id);



            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;
            // Buyer Details
            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            double amount = (double)order.Total;
            //var products = db.Items.Select(x => x.Item_Name).ToList();
            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = amount;
            onceOffRequest.item_name = "Your appointment Number is " + id;
            onceOffRequest.item_description = "You are now paying your rental fee";
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";

            order.Payment = true;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            var getclientEmal = (from i in db.Orders
                                 where i.OrderId == id
                                 select i.Email).FirstOrDefault();
            var getclientName = (from i in db.Orders
                                 where i.OrderId == id
                                 select i.FirstName).FirstOrDefault();
            var getlastName = (from i in db.Orders
                               where i.OrderId == id
                               select i.LastName).FirstOrDefault();
            var getAddress = (from i in db.Orders
                              where i.OrderId == id
                              select i.Address).FirstOrDefault();
            var getReference = (from i in db.Orders
                                where i.OrderId == id
                                select i.Reference).FirstOrDefault();
            var getInvoice = (from i in db.QRCodes
                              where i.OrderId == id
                              select i.QRId).FirstOrDefault();

            ViewBag.Body = $"Hi " + getclientName + "<br/>" +
  $"You have successfully paid for an order to be delivered for you. Here is your reference number: " + getReference + ". you can track your order here: https://grp20abcargo/Checkout/TrackOrder/ . Your order will be delivered to this Adress: " + getAddress + ".Thank you for choosing us." +
  $"<br/>" +
  $"AB CARGO";
            Email email = new Email();
            email.Gmail("Order delivery Information", ViewBag.Body, getclientEmal);
            return Redirect(redirectUrl);
        }


        public ActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            adHocRequest.merchant_id = this.payFastSettings.MerchantId;
            adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
            adHocRequest.return_url = this.payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
            adHocRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            adHocRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            adHocRequest.m_payment_id = "";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion Payment
    }
}
