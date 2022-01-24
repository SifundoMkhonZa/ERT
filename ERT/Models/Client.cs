using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERT.Models
{
	public class Client
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[DisplayName("Client Id")]
		public string Client_Id { get; set; }
		//[Required(ErrorMessage = "ID number is required")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "This field must be 13 characters")]
        [DisplayName("ID number")]
        public string Client_IDNo { get; set; }

        //[Required(ErrorMessage = "First name is required")]
        [DisplayName("First Name")]
        public string Client_Name { get; set; }

        //[Required(ErrorMessage = "Last name is required")]
        [DisplayName("Last Name")]
        public string Client_Surname { get; set; }

     
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact Number")]
        //[Required(ErrorMessage = "Contact Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
          ErrorMessage = "Entered Contact number format is not valid.")]
        public string Client_Cellnumber { get; set; }

        //[Required(ErrorMessage = "Residence address is required")]
        [DisplayName("Residence address")]
        public string Client_Address { get; set; }

        //[Required(ErrorMessage = "Email address is required")]
        [DisplayName("Email address")]
        [EmailAddress]
        public string Client_Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telephone")]
       // [Required(ErrorMessage = "Telephone number is Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
          ErrorMessage = "Entered tellephone number format is not valid.")]
        public string Client_Tellnum { get; set; }
		public virtual List<Order> Orders{ get; set; }

	}
}