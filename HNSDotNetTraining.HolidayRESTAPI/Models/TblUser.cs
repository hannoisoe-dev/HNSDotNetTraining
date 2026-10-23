using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.HolidayRESTAPI.Models;

public partial class TblUser
{
    public int No { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
