using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HNSDotNetTraining.ConsoleApp.Models
{
    [Table("Tbl_SpaService")]
    public class SpaServiceDataModel
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public bool DeleteFlag { get; set; } = false;

        public virtual ICollection<AppointmentDataModel> Appointments { get; set; } = new List<AppointmentDataModel>();
    }

    [Table("Tbl_Customer")]
    public class CustomerDataModel
    {
        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool DeleteFlag { get; set; } = false;

        public virtual ICollection<AppointmentDataModel> Appointments { get; set; } = new List<AppointmentDataModel>();
    }

    [Table("Tbl_Therapist")]
    public class TherapistDataModel
    {
        [Key]
        public int TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = "09:00 - 18:00";
        public bool IsAvailable { get; set; } = true;
        public bool DeleteFlag { get; set; } = false;

        public virtual ICollection<AppointmentDataModel> Appointments { get; set; } = new List<AppointmentDataModel>();
    }

    [Table("Tbl_Appointment")]
    public class AppointmentDataModel
    {
        [Key]
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int TherapistId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid
        public bool DeleteFlag { get; set; } = false;

        [ForeignKey("CustomerId")]
        public virtual CustomerDataModel? Customer { get; set; }

        [ForeignKey("TherapistId")]
        public virtual TherapistDataModel? Therapist { get; set; }

        [ForeignKey("ServiceId")]
        public virtual SpaServiceDataModel? Service { get; set; }

        public virtual ICollection<PaymentDataModel> Payments { get; set; } = new List<PaymentDataModel>();
    }

    [Table("Tbl_Payment")]
    public class PaymentDataModel
    {
        [Key]
        public int PaymentId { get; set; }
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string ReceiptNumber { get; set; } = string.Empty;

        [ForeignKey("AppointmentId")]
        public virtual AppointmentDataModel? Appointment { get; set; }
    }

    // DTOs for Dapper / Display queries
    public class AppointmentDisplayModel
    {
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
