using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERT.Models
{
	public class Return
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Shop ID")]
        public int Return_Id { get; set; }

        [Required]
        [DisplayName("What is the issue with the item you bought?")]
        public string Return_Comment { get; set; }

        [DisplayName("Please upload the image of your item here:")]
        public byte[] Item_Image { get; set; }

        
        [DisplayName("What is the issue with the item you bought?")]
        public bool Return_Refund { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

    }
}