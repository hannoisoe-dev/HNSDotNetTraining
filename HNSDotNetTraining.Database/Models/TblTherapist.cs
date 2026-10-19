using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.Database.Models;

public partial class TblTherapist
{
    public int TherapistId { get; set; }

    public string TherapistName { get; set; } = null!;

    public string Specialty { get; set; } = null!;

    public string WorkingHours { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual ICollection<TblAppointment> TblAppointments { get; set; } = new List<TblAppointment>();
}
