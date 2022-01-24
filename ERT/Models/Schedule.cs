using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERT.Models
{
	public class Schedule
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[DisplayName("Schedule ID")]
		public int Schedule_ID { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[DisplayName("Schedule Date")]
		public DateTime Schedule_Date { get; set; }

		public int OrderId { get; set; }
		public virtual Order Order { get; set; }

		public string Driver_ID { get; set; }
		public virtual Driver Driver { get; set; }



	}
}