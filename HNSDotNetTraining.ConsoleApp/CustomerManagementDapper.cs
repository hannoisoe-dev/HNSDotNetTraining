using Dapper;
using HNSDotNetTraining.ConsoleApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class CustomerManagementDapper : ICustomerManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void Create(string customerName, string phoneNumber, string? email)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                INSERT INTO Tbl_Customer (CustomerName, PhoneNumber, Email, DeleteFlag)
                VALUES (@CustomerName, @PhoneNumber, @Email, 0);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            int newId = db.QuerySingle<int>(sql, new { CustomerName = customerName, PhoneNumber = phoneNumber, Email = email });
            Console.WriteLine($"[Dapper] Customer '{customerName}' registered successfully with ID: {newId}");
        }

        public void Read()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT CustomerId, CustomerName, PhoneNumber, Email FROM Tbl_Customer WHERE DeleteFlag = 0 ORDER BY CustomerId";
            var customers = db.Query<CustomerDataModel>(sql).ToList();

            if (!customers.Any())
            {
                Console.WriteLine("No customers found.");
                return;
            }

            Console.WriteLine("==================== Customer List (Dapper) ====================");
            foreach (var c in customers)
            {
                Console.WriteLine($"ID: {c.CustomerId} | Name: {c.CustomerName} | Phone: {c.PhoneNumber} | Email: {c.Email ?? "N/A"}");
            }
            Console.WriteLine("================================================================");
        }

        public void Edit(int customerId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT CustomerId, CustomerName, PhoneNumber, Email FROM Tbl_Customer WHERE CustomerId = @CustomerId AND DeleteFlag = 0";
            var customer = db.QueryFirstOrDefault<CustomerDataModel>(sql, new { CustomerId = customerId });

            if (customer == null)
            {
                Console.WriteLine($"Customer with ID {customerId} not found!");
                return;
            }

            Console.WriteLine($"Customer ID: {customer.CustomerId}");
            Console.WriteLine($"Name: {customer.CustomerName}");
            Console.WriteLine($"Phone: {customer.PhoneNumber}");
            Console.WriteLine($"Email: {customer.Email ?? "N/A"}");
        }

        public void Update(int customerId, string customerName, string phoneNumber, string? email)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                UPDATE Tbl_Customer 
                SET CustomerName = @CustomerName, PhoneNumber = @PhoneNumber, Email = @Email 
                WHERE CustomerId = @CustomerId AND DeleteFlag = 0";

            int rows = db.Execute(sql, new { CustomerId = customerId, CustomerName = customerName, PhoneNumber = phoneNumber, Email = email });
            string message = rows > 0 ? "[Dapper] Customer updated successfully." : "Customer not found!";
            Console.WriteLine(message);
        }

        public void ViewHistory(int customerId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string customerSql = "SELECT CustomerName, PhoneNumber FROM Tbl_Customer WHERE CustomerId = @CustomerId AND DeleteFlag = 0";
            var customer = db.QueryFirstOrDefault(customerSql, new { CustomerId = customerId });

            if (customer == null)
            {
                Console.WriteLine($"Customer with ID {customerId} not found!");
                return;
            }

            string historySql = @"
                SELECT a.AppointmentId, a.AppointmentDate, a.StartTime, a.EndTime, a.Status, a.PaymentStatus,
                       s.ServiceName, s.Price,
                       t.TherapistName
                FROM Tbl_Appointment a
                INNER JOIN Tbl_SpaService s ON a.ServiceId = s.ServiceId
                INNER JOIN Tbl_Therapist t ON a.TherapistId = t.TherapistId
                WHERE a.CustomerId = @CustomerId AND a.DeleteFlag = 0
                ORDER BY a.AppointmentDate DESC, a.StartTime DESC";

            var history = db.Query(historySql, new { CustomerId = customerId }).ToList();

            Console.WriteLine($"\n================ Appointment History for {customer.CustomerName} (Phone: {customer.PhoneNumber}) [Dapper] ================");
            if (!history.Any())
            {
                Console.WriteLine("No appointment history for this customer.");
                return;
            }

            foreach (var row in history)
            {
                Console.WriteLine($"Appt #{row.AppointmentId} | Date: {Convert.ToDateTime(row.AppointmentDate):yyyy-MM-dd} {((TimeSpan)row.StartTime):hh\\:mm}-{((TimeSpan)row.EndTime):hh\\:mm} | Service: {row.ServiceName} (${Convert.ToDecimal(row.Price):F2}) | Therapist: {row.TherapistName} | Status: {row.Status} | Payment: {row.PaymentStatus}");
            }
            Console.WriteLine("===================================================================================================");
        }
    }
}
