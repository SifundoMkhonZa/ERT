using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERT.Models
{
	public class CartCopy
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        public string CartId { get; set; }

        public int Count { get; set; }
        public System.DateTime DateCreated { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int Product_id { get; set; }
        public virtual Product Product { get; set; }
    }
}