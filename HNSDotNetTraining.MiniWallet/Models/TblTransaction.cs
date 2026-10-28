using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.MiniWallet.Models;

public partial class TblTransaction
{
    public int Id { get; set; }

    public string FromMb { get; set; } = null!;

    public string ToMb { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }
}
