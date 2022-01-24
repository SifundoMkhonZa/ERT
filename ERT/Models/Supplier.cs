using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERT.Models
{
    public class Supplier
    {
       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Supplier ID")]
        public string Supplier_Id { get; set;}

        [Required]
        [DisplayName("Supplier Name")]
        public string Supplier_Name { get; set; }

        [DisplayName("Supplier Image")]
        public byte[] Supplier_Image { get; set; }

        [Required]
        [DisplayName("Supplier Last Name")]
        public string Supplier_LastName { get; set; }

        [Required]
        [DisplayName("Supplier Residence Address")]
        public string Supplier_Address { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Recipient contact number is required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                 ErrorMessage = "Entered Contact number format is not valid.")]
        [DisplayName("Supplier Contact Number")]
        public string Supplier_ContactNumber { get; set; }

        [Required]
        [DisplayName("Supplier Email Address")]
        [EmailAddress]
        public string Supplier_Email { get; set; }

        public bool Supplier_Contract { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Supplier_Password { get; set; }


        [DisplayName("Supporting  Documents")]
        public byte[] Supplier_Ducuments { get; set; }
		public virtual List<Eshop> Eshops { get; set; }

	}
}