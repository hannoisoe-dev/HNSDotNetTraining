using System;
using System.Data;
using System.Data.SqlClient;

namespace HNSDotNetTraining.ConsoleApp
{
    public class AppointmentManagementAdoDotNet : IAppointmentManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void BookAppointment(int customerId, int therapistId, int serviceId, DateTime date, TimeSpan startTime)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            // 1. Verify customer exists
            string checkCustomer = "SELECT COUNT(*) FROM Tbl_Customer WHERE CustomerId = @CustomerId AND DeleteFlag = 0";
            using (SqlCommand cmdCust = new SqlCommand(checkCustomer, connection))
            {
                cmdCust.Parameters.AddWithValue("@CustomerId", customerId);
                int count = Convert.ToInt32(cmdCust.ExecuteScalar());
                if (count == 0)
                {
                    Console.WriteLine($"Error: Customer ID {customerId} does not exist or is inactive.");
                    return;
                }
            }

            // 2. Verify therapist exists and is available
            string checkTherapist = "SELECT TherapistName, IsAvailable FROM Tbl_Therapist WHERE TherapistId = @TherapistId AND DeleteFlag = 0";
            string therapistName = "";
            bool isAvailable = false;
            using (SqlCommand cmdTherapist = new SqlCommand(checkTherapist, connection))
            {
                cmdTherapist.Parameters.AddWithValue("@TherapistId", therapistId);
                using SqlDataReader reader = cmdTherapist.ExecuteReader();
                if (!reader.Read())
                {
                    Console.WriteLine($"Error: Therapist ID {therapistId} does not exist.");
                    return;
                }
                therapistName = reader["TherapistName"].ToString()!;
                isAvailable = Convert.ToBoolean(reader["IsAvailable"]);
            }

            if (!isAvailable)
            {
                Console.WriteLine($"Error: Therapist '{therapistName}' is currently marked as unavailable.");
                return;
            }

            // 3. Verify service exists and calculate end time
            string checkService = "SELECT DurationMinutes, Price, ServiceName FROM Tbl_SpaService WHERE ServiceId = @ServiceId AND DeleteFlag = 0";
            int duration = 0;
            decimal price = 0;
            string serviceName = "";
            using (SqlCommand cmdService = new SqlCommand(checkService, connection))
            {
                cmdService.Parameters.AddWithValue("@ServiceId", serviceId);
                using SqlDataReader reader = cmdService.ExecuteReader();
                if (!reader.Read())
                {
                    Console.WriteLine($"Error: Spa Service ID {serviceId} does not exist.");
                    return;
                }
                duration = Convert.ToInt32(reader["DurationMinutes"]);
                price = Convert.ToDecimal(reader["Price"]);
                serviceName = reader["ServiceName"].ToString()!;
            }

            TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(duration));

            // 4. Double-booking check
            string overlapQuery = @"
                SELECT COUNT(*) 
                FROM Tbl_Appointment 
                WHERE TherapistId = @TherapistId 
                  AND AppointmentDate = @AppointmentDate
                  AND Status NOT IN ('Cancelled')
                  AND DeleteFlag = 0
                  AND StartTime < @EndTime 
                  AND EndTime > @StartTime";

            using (SqlCommand cmdOverlap = new SqlCommand(overlapQuery, connection))
            {
                cmdOverlap.Parameters.AddWithValue("@TherapistId", therapistId);
                cmdOverlap.Parameters.AddWithValue("@AppointmentDate", date.Date);
                cmdOverlap.Parameters.AddWithValue("@StartTime", startTime);
                cmdOverlap.Parameters.AddWithValue("@EndTime", endTime);
                int overlapCount = Convert.ToInt32(cmdOverlap.ExecuteScalar());
                if (overlapCount > 0)
                {
                    Console.WriteLine($"\n[Booking Rejected] DOUBLE-BOOKING PREVENTED (ADO.NET)!");
                    Console.WriteLine($"Therapist '{therapistName}' already has an overlapping appointment on {date:yyyy-MM-dd} during {startTime:hh\\:mm} - {endTime:hh\\:mm}. Please select another time or therapist.");
                    return;
                }
            }

            // 5. Insert Appointment
            string insertSql = @"
                INSERT INTO Tbl_Appointment 
                (CustomerId, TherapistId, ServiceId, AppointmentDate, StartTime, EndTime, Status, PaymentStatus, DeleteFlag)
                VALUES 
                (@CustomerId, @TherapistId, @ServiceId, @AppointmentDate, @StartTime, @EndTime, 'Confirmed', 'Unpaid', 0);
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmdInsert = new SqlCommand(insertSql, connection))
            {
                cmdInsert.Parameters.AddWithValue("@CustomerId", customerId);
                cmdInsert.Parameters.AddWithValue("@TherapistId", therapistId);
                cmdInsert.Parameters.AddWithValue("@ServiceId", serviceId);
                cmdInsert.Parameters.AddWithValue("@AppointmentDate", date.Date);
                cmdInsert.Parameters.AddWithValue("@StartTime", startTime);
                cmdInsert.Parameters.AddWithValue("@EndTime", endTime);

                object newApptId = cmdInsert.ExecuteScalar();
                Console.WriteLine($"\n[Booking Successful - ADO.NET] Appointment #{newApptId} confirmed!");
                Console.WriteLine($"Date: {date:yyyy-MM-dd} | Time: {startTime:hh\\:mm} - {endTime:hh\\:mm} ({duration} mins)");
                Console.WriteLine($"Service: {serviceName} (${price:F2}) | Therapist: {therapistName}");
                Console.WriteLine($"Payment Status: Unpaid");
            }
        }

        public void ViewAppointments(string? statusFilter = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                SELECT a.AppointmentId, 
                       c.CustomerId, c.CustomerName, c.PhoneNumber,
                       t.TherapistId, t.TherapistName,
                       s.ServiceId, s.ServiceName, s.DurationMinutes, s.Price,
                       a.AppointmentDate, a.StartTime, a.EndTime, a.Status, a.PaymentStatus
                FROM Tbl_Appointment a
                INNER JOIN Tbl_Customer c ON a.CustomerId = c.CustomerId
                INNER JOIN Tbl_Therapist t ON a.TherapistId = t.TherapistId
                INNER JOIN Tbl_SpaService s ON a.ServiceId = s.ServiceId
                WHERE a.DeleteFlag = 0";

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query += " AND a.Status = @Status";
            }

            query += " ORDER BY a.AppointmentDate DESC, a.StartTime DESC";

            SqlCommand cmd = new SqlCommand(query, connection);
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                cmd.Parameters.AddWithValue("@Status", statusFilter);
            }

            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            Console.WriteLine("\n========================= Spa Appointments (ADO.NET) =========================");
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No appointments found.");
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                DateTime dtVal = Convert.ToDateTime(row["AppointmentDate"]);
                TimeSpan st = (TimeSpan)row["StartTime"];
                TimeSpan et = (TimeSpan)row["EndTime"];
                decimal price = Convert.ToDecimal(row["Price"]);
                Console.WriteLine($"Appt #{row["AppointmentId"]} | Date: {dtVal:yyyy-MM-dd} {st:hh\\:mm}-{et:hh\\:mm} | Customer: {row["CustomerName"]} ({row["PhoneNumber"]}) | Service: {row["ServiceName"]} (${price:F2}) | Therapist: {row["TherapistName"]} | Status: [{row["Status"]}] | Payment: [{row["PaymentStatus"]}]");
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

            using SqlConnection connection = new SqlConnection(_connectionString);
            string updateQuery = "UPDATE Tbl_Appointment SET Status = @Status WHERE AppointmentId = @AppointmentId AND DeleteFlag = 0";
            SqlCommand cmd = new SqlCommand(updateQuery, connection);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            connection.Open();
            int rows = cmd.ExecuteNonQuery();

            if (rows > 0)
            {
                Console.WriteLine($"[ADO.NET] Appointment #{appointmentId} status updated to [{status}].");
            }
            else
            {
                Console.WriteLine($"Appointment #{appointmentId} not found.");
            }
        }
    }
}
