using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.MiniWallet.Models;

public partial class TblAccount
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public decimal Balance { get; set; }

    public int Pin { get; set; }
}
