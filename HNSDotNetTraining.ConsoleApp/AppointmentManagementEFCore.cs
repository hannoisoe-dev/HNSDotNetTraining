using HNSDotNetTraining.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class AppointmentManagementEFCore : IAppointmentManagement
    {
        public void BookAppointment(int customerId, int therapistId, int serviceId, DateTime date, TimeSpan startTime)
        {
            using AppDBContext db = new AppDBContext();

            // 1. Verify customer
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId && !c.DeleteFlag);
            if (customer == null)
            {
                Console.WriteLine($"Error: Customer ID {customerId} does not exist or is inactive.");
                return;
            }

            // 2. Verify therapist
            var therapist = db.Therapists.FirstOrDefault(t => t.TherapistId == therapistId && !t.DeleteFlag);
            if (therapist == null)
            {
                Console.WriteLine($"Error: Therapist ID {therapistId} does not exist.");
                return;
            }
            if (!therapist.IsAvailable)
            {
                Console.WriteLine($"Error: Therapist '{therapist.TherapistName}' is currently marked as unavailable.");
                return;
            }

            // 3. Verify service
            var service = db.SpaServices.FirstOrDefault(s => s.ServiceId == serviceId && !s.DeleteFlag);
            if (service == null)
            {
                Console.WriteLine($"Error: Spa Service ID {serviceId} does not exist.");
                return;
            }

            int duration = service.DurationMinutes;
            TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(duration));

            // 4. Double-booking check
            bool hasOverlap = db.Appointments.Any(a =>
                a.TherapistId == therapistId &&
                a.AppointmentDate.Date == date.Date &&
                a.Status != "Cancelled" &&
                !a.DeleteFlag &&
                a.StartTime < endTime &&
                a.EndTime > startTime);

            if (hasOverlap)
            {
                Console.WriteLine($"\n[Booking Rejected] DOUBLE-BOOKING PREVENTED (EF Core)!");
                Console.WriteLine($"Therapist '{therapist.TherapistName}' already has an overlapping appointment on {date:yyyy-MM-dd} during {startTime:hh\\:mm} - {endTime:hh\\:mm}. Please select another time or therapist.");
                return;
            }

            // 5. Insert Appointment
            var appt = new AppointmentDataModel
            {
                CustomerId = customerId,
                TherapistId = therapistId,
                ServiceId = serviceId,
                AppointmentDate = date.Date,
                StartTime = startTime,
                EndTime = endTime,
                Status = "Confirmed",
                PaymentStatus = "Unpaid",
                DeleteFlag = false
            };

            db.Appointments.Add(appt);
            db.SaveChanges();

            Console.WriteLine($"\n[Booking Successful - EF Core] Appointment #{appt.AppointmentId} confirmed!");
            Console.WriteLine($"Date: {date:yyyy-MM-dd} | Time: {startTime:hh\\:mm} - {endTime:hh\\:mm} ({duration} mins)");
            Console.WriteLine($"Service: {service.ServiceName} (${service.Price:F2}) | Therapist: {therapist.TherapistName}");
            Console.WriteLine($"Payment Status: Unpaid");
        }

        public void ViewAppointments(string? statusFilter = null)
        {
            using AppDBContext db = new AppDBContext();

            var query = db.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Therapist)
                .Include(a => a.Service)
                .Where(a => !a.DeleteFlag);

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(a => a.Status == statusFilter);
            }

            var list = query.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.StartTime).ToList();

            Console.WriteLine("\n========================= Spa Appointments (EF Core) =========================");
            if (!list.Any())
            {
                Console.WriteLine("No appointments found.");
                return;
            }

            foreach (var a in list)
            {
                string custName = a.Customer?.CustomerName ?? "Unknown";
                string phone = a.Customer?.PhoneNumber ?? "N/A";
                string serviceName = a.Service?.ServiceName ?? "Unknown";
                decimal price = a.Service?.Price ?? 0;
                string therapistName = a.Therapist?.TherapistName ?? "Unassigned";

                Console.WriteLine($"Appt #{a.AppointmentId} | Date: {a.AppointmentDate:yyyy-MM-dd} {a.StartTime:hh\\:mm}-{a.EndTime:hh\\:mm} | Customer: {custName} ({phone}) | Service: {serviceName} (${price:F2}) | Therapist: {therapistName} | Status: [{a.Status}] | Payment: [{a.PaymentStatus}]");
            }
            Console.WriteLine("=============================================================================");
        }

        public void UpdateStatus(int appointmentId, string newStatus)
        {
            string status = newStatus.Trim();
            if (status != "Pending" && status != "Confirmed" && status != "Completed" && status != "Cancelled")
            {
                Console.WriteLine("Invalid status! Options are: Pending, Confirmed, Completed, Cancelled");
                return;
            }

            using AppDBContext db = new AppDBContext();
            var appt = db.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId && !a.DeleteFlag);

            if (appt == null)
            {
                Console.WriteLine($"Appointment #{appointmentId} not found.");
                return;
            }

            appt.Status = status;
            db.SaveChanges();
            Console.WriteLine($"[EF Core] Appointment #{appointmentId} status updated to [{status}].");
        }
    }
}
