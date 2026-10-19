using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.Database.Models;

public partial class TblSpaService
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public decimal Price { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual ICollection<TblAppointment> TblAppointments { get; set; } = new List<TblAppointment>();
}
