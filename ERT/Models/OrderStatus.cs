using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERT.Models
{
	public class OrderStatus
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrderStatus_ID { get; set; }

		[DisplayName("Order Status ")]
		public string Status_Name  { get; set; }

		public virtual List<Order> Orders { get; set; }
	}
}