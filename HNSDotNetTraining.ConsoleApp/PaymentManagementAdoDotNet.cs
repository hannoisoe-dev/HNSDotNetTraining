using System;
using System.Data;
using System.Data.SqlClient;

namespace HNSDotNetTraining.ConsoleApp
{
    public class PaymentManagementAdoDotNet : IPaymentManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void ProcessPayment(int appointmentId, string paymentMethod)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();

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

                string customerName = "";
                string phoneNumber = "";
                string serviceName = "";
                string therapistName = "";
                decimal servicePrice = 0;
                string currentPaymentStatus = "";
                DateTime apptDate = DateTime.Now;
                TimeSpan startTime = TimeSpan.Zero;

                using (SqlCommand cmd = new SqlCommand(checkQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        Console.WriteLine($"Error: Appointment #{appointmentId} not found.");
                        transaction.Rollback();
                        return;
                    }

                    currentPaymentStatus = reader["PaymentStatus"].ToString() ?? "Unpaid";
                    customerName = reader["CustomerName"].ToString() ?? "";
                    phoneNumber = reader["PhoneNumber"].ToString() ?? "";
                    serviceName = reader["ServiceName"].ToString() ?? "";
                    therapistName = reader["TherapistName"].ToString() ?? "";
                    servicePrice = Convert.ToDecimal(reader["Price"]);
                    apptDate = Convert.ToDateTime(reader["AppointmentDate"]);
                    startTime = (TimeSpan)reader["StartTime"];
                }

                if (currentPaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[Payment Notice] Appointment #{appointmentId} has already been paid!");
                    transaction.Rollback();
                    return;
                }

                // 2. Generate Receipt Number
                string receiptNumber = $"REC-{DateTime.Now:yyyyMMdd}-{appointmentId:D4}";

                // 3. Insert Payment Record
                string insertPaymentQuery = @"
                    INSERT INTO Tbl_Payment (AppointmentId, Amount, PaymentMethod, PaymentDate, ReceiptNumber)
                    VALUES (@AppointmentId, @Amount, @PaymentMethod, GETDATE(), @ReceiptNumber);";

                using (SqlCommand payCmd = new SqlCommand(insertPaymentQuery, connection, transaction))
                {
                    payCmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    payCmd.Parameters.AddWithValue("@Amount", servicePrice);
                    payCmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    payCmd.Parameters.AddWithValue("@ReceiptNumber", receiptNumber);
                    payCmd.ExecuteNonQuery();
                }

                // 4. Update Appointment PaymentStatus to 'Paid' and Status to 'Completed' (or remain Confirmed/Completed)
                string updateApptQuery = "UPDATE Tbl_Appointment SET PaymentStatus = 'Paid', Status = 'Completed' WHERE AppointmentId = @AppointmentId";
                using (SqlCommand updateCmd = new SqlCommand(updateApptQuery, connection, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    updateCmd.ExecuteNonQuery();
                }

                transaction.Commit();

                // 5. Generate and print clean receipt
                PrintReceipt(receiptNumber, appointmentId, customerName, phoneNumber, serviceName, therapistName, apptDate, startTime, servicePrice, paymentMethod);
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
            Console.WriteLine("                    OFFICIAL SPA PAYMENT RECEIPT                  ");
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
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT p.PaymentId, p.ReceiptNumber, p.AppointmentId, c.CustomerName, s.ServiceName, p.Amount, p.PaymentMethod, p.PaymentDate
                FROM Tbl_Payment p
                INNER JOIN Tbl_Appointment a ON p.AppointmentId = a.AppointmentId
                INNER JOIN Tbl_Customer c ON a.CustomerId = c.CustomerId
                INNER JOIN Tbl_SpaService s ON a.ServiceId = s.ServiceId
                ORDER BY p.PaymentDate DESC";

            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            Console.WriteLine("\n======================== Payment & Transactions Report ========================");
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No payments recorded yet.");
                return;
            }

            decimal totalRevenue = 0;
            foreach (DataRow row in dt.Rows)
            {
                decimal amt = Convert.ToDecimal(row["Amount"]);
                totalRevenue += amt;
                Console.WriteLine($"Receipt: {row["ReceiptNumber"]} | Date: {Convert.ToDateTime(row["PaymentDate"]):yyyy-MM-dd HH:mm} | Customer: {row["CustomerName"]} | Service: {row["ServiceName"]} | Method: {row["PaymentMethod"]} | Amount: ${amt:F2}");
            }
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine($"Total Revenue Collected: ${totalRevenue:F2}");
            Console.WriteLine("===============================================================================");
        }
    }
}
