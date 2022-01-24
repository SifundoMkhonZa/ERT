using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERT.Models
{
	public class Eshop
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Shop ID")]
        public int Shop_Id { get; set; }

        [Required]
        [DisplayName("Shop Name")]
        public string Shop_Name { get; set; }

        [DisplayName("Shop Image")]
        public byte[] Shop_Image { get; set; }

        public string Supplier_Id { get; set; }
		public virtual Supplier Supplier { get; set; }
	}
}