using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class PaymentManagementDapper : IPaymentManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void ProcessPayment(int appointmentId, string paymentMethod)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            db.Open();

            using var transaction = db.BeginTransaction();
            try
            {
                // 1. Get Appointment and Service details
                string checkQuery = @"
                    SELECT a.AppointmentId, a.Status, a.PaymentStatus, a.AppointmentDate, a.StartTime,
                           c.CustomerName, c.PhoneNumber,
                           s.ServiceName, s.Price,
                           t.TherapistName
                    FROM Tbl_Appointment a
                    INNER JOIN Tbl_Customer c ON a.CustomerId = c.CustomerId
                    INNER JOIN Tbl_SpaService s ON a.ServiceId = s.ServiceId
                    INNER JOIN Tbl_Therapist t ON a.TherapistId = t.TherapistId
                    WHERE a.AppointmentId = @AppointmentId AND a.DeleteFlag = 0";

                var appt = db.QueryFirstOrDefault(checkQuery, new { AppointmentId = appointmentId }, transaction: transaction);
                if (appt == null)
                {
                    Console.WriteLine($"Error: Appointment #{appointmentId} not found.");
                    transaction.Rollback();
                    return;
                }

                if (string.Equals((string)appt.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[Payment Notice] Appointment #{appointmentId} has already been paid!");
                    transaction.Rollback();
                    return;
                }

                decimal servicePrice = (decimal)appt.Price;
                string receiptNumber = $"REC-{DateTime.Now:yyyyMMdd}-{appointmentId:D4}";

                // 2. Insert Payment Record
                string insertPaymentQuery = @"
                    INSERT INTO Tbl_Payment (AppointmentId, Amount, PaymentMethod, PaymentDate, ReceiptNumber)
                    VALUES (@AppointmentId, @Amount, @PaymentMethod, GETDATE(), @ReceiptNumber);";

                db.Execute(insertPaymentQuery, new
                {
                    AppointmentId = appointmentId,
                    Amount = servicePrice,
                    PaymentMethod = paymentMethod,
                    ReceiptNumber = receiptNumber
                }, transaction: transaction);

                // 3. Update Appointment
                string updateApptQuery = "UPDATE Tbl_Appointment SET PaymentStatus = 'Paid', Status = 'Completed' WHERE AppointmentId = @AppointmentId";
                db.Execute(updateApptQuery, new { AppointmentId = appointmentId }, transaction: transaction);

                transaction.Commit();

                PrintReceipt(receiptNumber, appointmentId, (string)appt.CustomerName, (string)appt.PhoneNumber, (string)appt.ServiceName, (string)appt.TherapistName, (DateTime)appt.AppointmentDate, (TimeSpan)appt.StartTime, servicePrice, paymentMethod);
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
            Console.WriteLine("             OFFICIAL SPA PAYMENT RECEIPT (DAPPER)                ");
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
            using IDbConnection db = new SqlConnection(_connectionString);
            string query = @"
                SELECT p.PaymentId, p.ReceiptNumber, p.AppointmentId, c.CustomerName, s.ServiceName, p.Amount, p.PaymentMethod, p.PaymentDate
                FROM Tbl_Payment p
                INNER JOIN Tbl_Appointment a ON p.AppointmentId = a.AppointmentId
                INNER JOIN Tbl_Customer c ON a.CustomerId = c.CustomerId
                INNER JOIN Tbl_SpaService s ON a.ServiceId = s.ServiceId
                ORDER BY p.PaymentDate DESC";

            var list = db.Query(query).ToList();

            Console.WriteLine("\n================== Payment & Transactions Report (Dapper) ==================");
            if (!list.Any())
            {
                Console.WriteLine("No payments recorded yet.");
                return;
            }

            decimal totalRevenue = 0;
            foreach (var row in list)
            {
                decimal amt = (decimal)row.Amount;
                totalRevenue += amt;
                Console.WriteLine($"Receipt: {row.ReceiptNumber} | Date: {Convert.ToDateTime(row.PaymentDate):yyyy-MM-dd HH:mm} | Customer: {row.CustomerName} | Service: {row.ServiceName} | Method: {row.PaymentMethod} | Amount: ${amt:F2}");
            }
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine($"Total Revenue Collected: ${totalRevenue:F2}");
            Console.WriteLine("============================================================================");
        }
    }
}
