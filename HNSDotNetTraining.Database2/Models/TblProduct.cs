using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HNSDotNetTraining.Database2.Models;

[Table("Tbl_Product")]
public partial class TblProduct
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "decimal(18, 0)")]
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? ProductCode { get; set; }
}
