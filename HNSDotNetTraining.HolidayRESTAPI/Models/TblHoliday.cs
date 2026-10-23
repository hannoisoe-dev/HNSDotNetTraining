using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.HolidayRESTAPI.Models;

public partial class TblHoliday
{
    public int Id { get; set; }

    public string? Name { get; set; } = null!;

    public string? Date { get; set; } = null!;

    public string? Type { get; set; } = null!;

    public bool Observed { get; set; }

    public bool Paid { get; set; }
}
