using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.ConsoleApp.Models
{
    public class BlogDapperDataModel
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }

        public string BlogAuthor { get; set;  }

        public string BlogContent { get; set; }

        public bool DeleteFlag { get; set; }

    }

    [Table("Tbl_Blog")]
    public class BlogDataModel
    {
        [Key]
        public int BlogId { get; set; }

        [Column("BlogTitle")]
        public string BlogTitle { get; set; }

        public string BlogAuthor { get; set; }

        public string BlogContent { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
