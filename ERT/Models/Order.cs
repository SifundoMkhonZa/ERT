using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERT.Models
{
    public partial class Order
    {
        [ScaffoldColumn(false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Category ID")]
        public int OrderId { get; set; }

        [ScaffoldColumn(false)]
        public System.DateTime OrderDate { get; set; }

        [ScaffoldColumn(false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Render service address is required")]
        [DisplayName("Physical address")]
        [StringLength(70)]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Phone is required")]
        [StringLength(24)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }
		public bool Payment { get; set; }
		public List<OrderDetail> OrderDetails { get; set; }
        public int OrderStatus_ID { get; set; }
        public virtual OrderStatus OrderStatus{ get; set; }
        public string Client_Id { get; set; }
        public virtual Client Client { get; set; }

        [ScaffoldColumn(false)]
        public string Reference { get; set; }

        [ScaffoldColumn(false)]
        [DisplayName("Distance (km)")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Out of range")]
        public double Distance { get; set; }

        ApplicationDbContext db = new ApplicationDbContext();
        public int GetStatus(string status)
        {
            var statusId = (from p in db.OrderStatus
                            where p.Status_Name == status
                            select p.OrderStatus_ID).FirstOrDefault();
            return statusId;
        }
        public void UpdateStatus(int id, string status)
        {
            Order upDatestatus = (from c in db.Orders
                                      where c.OrderId == id
                                      select c).SingleOrDefault();
            upDatestatus.OrderStatus_ID = GetStatus(status);
            db.SaveChanges();
        }
    }
}
