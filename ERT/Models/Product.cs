using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERT.Models
{
    public class Product
    {
        [ScaffoldColumn(false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Product_id { get; set; }

        [Required(ErrorMessage = "An Album Title is required")]
        [StringLength(160)]
        public string Product_Name { get; set; }


        [DisplayName("Product Price")]
        [Required(ErrorMessage = "Price is required"),DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Image")]
        public byte[] Image { get; set; }

        [DisplayName("Category Name")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int Shop_Id { get; set; }
        public virtual Eshop Eshop { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}