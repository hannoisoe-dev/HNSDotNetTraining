using System;
using System.Collections.Generic;

namespace HNSDotNetTraining.Database.Models;

public partial class TblPayment
{
    public int PaymentId { get; set; }

    public int AppointmentId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string ReceiptNumber { get; set; } = null!;

    public virtual TblAppointment Appointment { get; set; } = null!;
}
