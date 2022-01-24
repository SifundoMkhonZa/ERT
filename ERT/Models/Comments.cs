using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERT.Models
{
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        public string Comment { get; set; }

        public DateTime? ThisDateTime { get; set; }

        public int Shop_Id { get; set; }

        public int? Rating { get; set; }
    }
}