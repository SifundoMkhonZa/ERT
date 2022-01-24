using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERT.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Category ID")]
        public int CategoryId { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [StringLength(250, MinimumLength = 2, ErrorMessage = "The Delivery Note should be between 2 - 250 Characters")]
        [DisplayName("Category Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string Supplier_Id { get; set; }
		public virtual Supplier Supplier { get; set; }
		public List<Product> Products { get; set; }
    }
}