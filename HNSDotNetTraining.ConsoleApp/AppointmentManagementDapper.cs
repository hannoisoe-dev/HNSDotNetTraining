using Dapper;
using HNSDotNetTraining.ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class AppointmentManagementDapper : IAppointmentManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void BookAppointment(int customerId, int therapistId, int serviceId, DateTime date, TimeSpan startTime)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            // 1. Verify customer exists
            string checkCustomer = "SELECT COUNT(*) FROM Tbl_Customer WHERE CustomerId = @CustomerId AND DeleteFlag = 0";
            if (db.ExecuteScalar<int>(checkCustomer, new { CustomerId = customerId }) == 0)
            {
                Console.WriteLine($"Error: Customer ID {customerId} does not exist or is inactive.");
                return;
            }

            // 2. Verify therapist exists and is available
            string checkTherapist = "SELECT TherapistName, IsAvailable FROM Tbl_Therapist WHERE TherapistId = @TherapistId AND DeleteFlag = 0";
            var therapist = db.QueryFirstOrDefault(checkTherapist, new { TherapistId = therapistId });
            if (therapist == null)
            {
                Console.WriteLine($"Error: Therapist ID {therapistId} does not exist.");
                return;
            }
            if (!(bool)therapist.IsAvailable)
            {
                Console.WriteLine($"Error: Therapist '{therapist.TherapistName}' is currently marked as unavailable.");
                return;
            }

            // 3. Verify service exists and calculate end time
            string checkService = "SELECT DurationMinutes, Price, ServiceName FROM Tbl_SpaService WHERE ServiceId = @ServiceId AND DeleteFlag = 0";
            var service = db.QueryFirstOrDefault(checkService, new { ServiceId = serviceId });
            if (service == null)
            {
                Console.WriteLine($"Error: Spa Service ID {serviceId} does not exist.");
                return;
            }

            int duration = (int)service.DurationMinutes;
            TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(duration));

            // 4. CRITICAL: Prevent Double-Booking of Therapist!
            // An overlap occurs if (ExistingStartTime < NewEndTime) AND (ExistingEndTime > NewStartTime) on the same date for active status
            string overlapQuery = @"
                SELECT COUNT(*) 
                FROM Tbl_Appointment 
                WHERE TherapistId = @TherapistId 
                  AND AppointmentDate = @AppointmentDate
                  AND Status NOT IN ('Cancelled')
                  AND DeleteFlag = 0
                  AND StartTime < @EndTime 
                  AND EndTime > @StartTime";

            int overlapCount = db.ExecuteScalar<int>(overlapQuery, new
            {
                TherapistId = therapistId,
                AppointmentDate = date.Date,
                StartTime = startTime,
                EndTime = endTime
            });

            if (overlapCount > 0)
            {
                Console.WriteLine($"\n[Booking Rejected] DOUBLE-BOOKING PREVENTED!");
                Console.WriteLine($"Therapist '{therapist.TherapistName}' already has an overlapping appointment on {date:yyyy-MM-dd} during {startTime:hh\\:mm} - {endTime:hh\\:mm}. Please select another time or therapist.");
                return;
            }

            // 5. Insert Appointment
            string insertSql = @"
                INSERT INTO Tbl_Appointment 
                (CustomerId, TherapistId, ServiceId, AppointmentDate, StartTime, EndTime, Status, PaymentStatus, DeleteFlag)
                VALUES 
                (@CustomerId, @TherapistId, @ServiceId, @AppointmentDate, @StartTime, @EndTime, 'Confirmed', 'Unpaid', 0);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            int newAppointmentId = db.QuerySingle<int>(insertSql, new
            {
                CustomerId = customerId,
                TherapistId = therapistId,
                ServiceId = serviceId,
                AppointmentDate = date.Date,
                StartTime = startTime,
                EndTime = endTime
            });

            Console.WriteLine($"\n[Booking Successful] Appointment #{newAppointmentId} confirmed!");
            Console.WriteLine($"Date: {date:yyyy-MM-dd} | Time: {startTime:hh\\:mm} - {endTime:hh\\:mm} ({duration} mins)");
            Console.WriteLine($"Service: {service.ServiceName} (${service.Price:F2}) | Therapist: {therapist.TherapistName}");
            Console.WriteLine($"Payment Status: Unpaid");
        }

        public void ViewAppointments(string? statusFilter = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

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

            var list = db.Query<AppointmentDisplayModel>(query, new { Status = statusFilter }).ToList();

            Console.WriteLine("\n=============================== Spa Appointments ===============================");
            if (!list.Any())
            {
                Console.WriteLine("No appointments found.");
                return;
            }

            foreach (var a in list)
            {
                Console.WriteLine($"Appt #{a.AppointmentId} | Date: {a.AppointmentDate:yyyy-MM-dd} {a.StartTime:hh\\:mm}-{a.EndTime:hh\\:mm} | Customer: {a.CustomerName} ({a.PhoneNumber}) | Service: {a.ServiceName} (${a.Price:F2}) | Therapist: {a.TherapistName} | Status: [{a.Status}] | Payment: [{a.PaymentStatus}]");
            }
            Console.WriteLine("=================================================================================");
        }

        public void UpdateStatus(int appointmentId, string newStatus)
        {
            // Valid statuses: Pending, Confirmed, Completed, Cancelled
            string status = newStatus.Trim();
            if (status != "Pending" && status != "Confirmed" && status != "Completed" && status != "Cancelled")
            {
                Console.WriteLine("Invalid status! Options are: Pending, Confirmed, Completed, Cancelled");
                return;
            }

            using IDbConnection db = new SqlConnection(_connectionString);
            string updateQuery = "UPDATE Tbl_Appointment SET Status = @Status WHERE AppointmentId = @AppointmentId AND DeleteFlag = 0";
            int rows = db.Execute(updateQuery, new { Status = status, AppointmentId = appointmentId });

            if (rows > 0)
            {
                Console.WriteLine($"Appointment #{appointmentId} status updated to [{status}].");
            }
            else
            {
                Console.WriteLine($"Appointment #{appointmentId} not found.");
            }
        }
    }
}
