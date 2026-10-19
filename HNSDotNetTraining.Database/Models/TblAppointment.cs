using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.Database.Models;

public partial class TblAppointment
{
    public int AppointmentId { get; set; }

    public int CustomerId { get; set; }

    public int TherapistId { get; set; }

    public int ServiceId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public bool DeleteFlag { get; set; }

    public virtual TblCustomer Customer { get; set; } = null!;

    public virtual TblSpaService Service { get; set; } = null!;

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();

    public virtual TblTherapist Therapist { get; set; } = null!;
}
