using Dapper;
using HNSDotNetTraining.ConsoleApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class SpaServiceManagementDapper : ISpaServiceManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void Create(string serviceName, string? description, int durationMinutes, decimal price)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                INSERT INTO Tbl_SpaService (ServiceName, Description, DurationMinutes, Price, DeleteFlag)
                VALUES (@ServiceName, @Description, @DurationMinutes, @Price, 0);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            int newId = db.QuerySingle<int>(sql, new { ServiceName = serviceName, Description = description, DurationMinutes = durationMinutes, Price = price });
            Console.WriteLine($"[Dapper] Spa Service '{serviceName}' created successfully with ID: {newId} (${price:F2}, {durationMinutes} mins).");
        }

        public void Read()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT ServiceId, ServiceName, Description, DurationMinutes, Price FROM Tbl_SpaService WHERE DeleteFlag = 0 ORDER BY ServiceId";
            var services = db.Query<SpaServiceDataModel>(sql).ToList();

            if (!services.Any())
            {
                Console.WriteLine("No spa services found.");
                return;
            }

            Console.WriteLine("==================== Spa Services List (Dapper) ====================");
            foreach (var s in services)
            {
                Console.WriteLine($"ID: {s.ServiceId} | {s.ServiceName} | {s.DurationMinutes} mins | ${s.Price:F2} | {s.Description ?? "No description"}");
            }
            Console.WriteLine("====================================================================");
        }

        public void Edit(int serviceId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT ServiceId, ServiceName, Description, DurationMinutes, Price FROM Tbl_SpaService WHERE ServiceId = @ServiceId AND DeleteFlag = 0";
            var service = db.QueryFirstOrDefault<SpaServiceDataModel>(sql, new { ServiceId = serviceId });

            if (service == null)
            {
                Console.WriteLine($"Service with ID {serviceId} not found!");
                return;
            }

            Console.WriteLine($"Service ID: {service.ServiceId}");
            Console.WriteLine($"Service Name: {service.ServiceName}");
            Console.WriteLine($"Description: {service.Description ?? "N/A"}");
            Console.WriteLine($"Duration: {service.DurationMinutes} minutes");
            Console.WriteLine($"Price: ${service.Price:F2}");
        }

        public void Update(int serviceId, string serviceName, string? description, int durationMinutes, decimal price)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                UPDATE Tbl_SpaService 
                SET ServiceName = @ServiceName, Description = @Description, DurationMinutes = @DurationMinutes, Price = @Price 
                WHERE ServiceId = @ServiceId AND DeleteFlag = 0";

            int rows = db.Execute(sql, new { ServiceId = serviceId, ServiceName = serviceName, Description = description, DurationMinutes = durationMinutes, Price = price });
            string message = rows > 0 ? "[Dapper] Service updated successfully." : "Service not found!";
            Console.WriteLine(message);
        }

        public void Delete(int serviceId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "UPDATE Tbl_SpaService SET DeleteFlag = 1 WHERE ServiceId = @ServiceId AND DeleteFlag = 0";

            int rows = db.Execute(sql, new { ServiceId = serviceId });
            string message = rows > 0 ? "[Dapper] Service deleted successfully." : "Service not found!";
            Console.WriteLine(message);
        }
    }
}
