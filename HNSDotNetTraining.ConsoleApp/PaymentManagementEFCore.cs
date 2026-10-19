using HNSDotNetTraining.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class PaymentManagementEFCore : IPaymentManagement
    {
        public void ProcessPayment(int appointmentId, string paymentMethod)
        {
            using AppDBContext db = new AppDBContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                var appt = db.Appointments
                    .Include(a => a.Customer)
                    .Include(a => a.Therapist)
                    .Include(a => a.Service)
                    .FirstOrDefault(a => a.AppointmentId == appointmentId && !a.DeleteFlag);

                if (appt == null)
                {
                    Console.WriteLine($"Error: Appointment #{appointmentId} not found.");
                    transaction.Rollback();
                    return;
                }

                if (string.Equals(appt.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[Payment Notice] Appointment #{appointmentId} has already been paid!");
                    transaction.Rollback();
                    return;
                }

                decimal servicePrice = appt.Service?.Price ?? 0;
                string receiptNumber = $"REC-{DateTime.Now:yyyyMMdd}-{appointmentId:D4}";

                var payment = new PaymentDataModel
                {
                    AppointmentId = appointmentId,
                    Amount = servicePrice,
                    PaymentMethod = paymentMethod,
                    PaymentDate = DateTime.Now,
                    ReceiptNumber = receiptNumber
                };

                db.Payments.Add(payment);

                appt.PaymentStatus = "Paid";
                appt.Status = "Completed";

                db.SaveChanges();
                transaction.Commit();

                string custName = appt.Customer?.CustomerName ?? "Unknown";
                string phone = appt.Customer?.PhoneNumber ?? "N/A";
                string servName = appt.Service?.ServiceName ?? "Unknown";
                string therName = appt.Therapist?.TherapistName ?? "Unassigned";

                PrintReceipt(receiptNumber, appointmentId, custName, phone, servName, therName, appt.AppointmentDate, appt.StartTime, servicePrice, paymentMethod);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"[Payment Failed] Transaction rolled back due to error: {ex.Message}");
            }
        }

        private void PrintReceipt(string receiptNo, int appointmentId, string customerName, string phone, string service, string therapist, DateTime date, TimeSpan time, decimal amount, string method)
        {
            Console.WriteLine("\n==================================================================");
            Console.WriteLine("             OFFICIAL SPA PAYMENT RECEIPT (EF CORE)               ");
            Console.WriteLine("==================================================================");
            Console.WriteLine($" Receipt No       : {receiptNo}");
            Console.WriteLine($" Issue Date       : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($" Appointment ID   : #{appointmentId}");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($" Customer Name    : {customerName}");
            Console.WriteLine($" Contact Phone    : {phone}");
            Console.WriteLine($" Therapist        : {therapist}");
            Console.WriteLine($" Appt Date & Time : {date:yyyy-MM-dd} at {time:hh\\:mm}");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($" Service Provided : {service}");
            Console.WriteLine($" Total Amount     : ${amount:F2}");
            Console.WriteLine($" Payment Method   : {method}");
            Console.WriteLine($" Payment Status   : PAID");
            Console.WriteLine("==================================================================");
            Console.WriteLine("              Thank you for choosing our Spa!                     ");
            Console.WriteLine("==================================================================\n");
        }

        public void ViewPaymentReport()
        {
            using AppDBContext db = new AppDBContext();

            var list = db.Payments
                .Include(p => p.Appointment!)
                    .ThenInclude(a => a.Customer)
                .Include(p => p.Appointment!)
                    .ThenInclude(a => a.Service)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();

            Console.WriteLine("\n================== Payment & Transactions Report (EF Core) ==================");
            if (!list.Any())
            {
                Console.WriteLine("No payments recorded yet.");
                return;
            }

            decimal totalRevenue = 0;
            foreach (var p in list)
            {
                string custName = p.Appointment?.Customer?.CustomerName ?? "Unknown";
                string servName = p.Appointment?.Service?.ServiceName ?? "Unknown";
                totalRevenue += p.Amount;
                Console.WriteLine($"Receipt: {p.ReceiptNumber} | Date: {p.PaymentDate:yyyy-MM-dd HH:mm} | Customer: {custName} | Service: {servName} | Method: {p.PaymentMethod} | Amount: ${p.Amount:F2}");
            }
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine($"Total Revenue Collected: ${totalRevenue:F2}");
            Console.WriteLine("=============================================================================");
        }
    }
}
