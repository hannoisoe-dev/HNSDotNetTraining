using System;
using System.Data;
using System.Data.SqlClient;

namespace HNSDotNetTraining.ConsoleApp
{
    public class SpaServiceManagementAdoDotNet : ISpaServiceManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void Create(string serviceName, string? description, int durationMinutes, decimal price)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                INSERT INTO Tbl_SpaService (ServiceName, Description, DurationMinutes, Price, DeleteFlag)
                VALUES (@ServiceName, @Description, @DurationMinutes, @Price, 0);
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ServiceName", serviceName);
            cmd.Parameters.AddWithValue("@Description", (object?)description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DurationMinutes", durationMinutes);
            cmd.Parameters.AddWithValue("@Price", price);

            connection.Open();
            object newId = cmd.ExecuteScalar();
            Console.WriteLine($"[ADO.NET] Spa Service '{serviceName}' created successfully with ID: {newId} (${price:F2}, {durationMinutes} mins).");
        }

        public void Read()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "SELECT ServiceId, ServiceName, Description, DurationMinutes, Price FROM Tbl_SpaService WHERE DeleteFlag = 0 ORDER BY ServiceId";

            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No spa services found.");
                return;
            }

            Console.WriteLine("==================== Spa Services List (ADO.NET) ====================");
            foreach (DataRow row in dt.Rows)
            {
                string desc = row["Description"] == DBNull.Value ? "No description" : row["Description"].ToString()!;
                Console.WriteLine($"ID: {row["ServiceId"]} | {row["ServiceName"]} | {row["DurationMinutes"]} mins | ${Convert.ToDecimal(row["Price"]):F2} | {desc}");
            }
            Console.WriteLine("=====================================================================");
        }

        public void Edit(int serviceId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "SELECT ServiceId, ServiceName, Description, DurationMinutes, Price FROM Tbl_SpaService WHERE ServiceId = @ServiceId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ServiceId", serviceId);

            connection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                Console.WriteLine($"Service with ID {serviceId} not found!");
                return;
            }

            Console.WriteLine($"Service ID: {reader["ServiceId"]}");
            Console.WriteLine($"Service Name: {reader["ServiceName"]}");
            Console.WriteLine($"Description: {(reader["Description"] == DBNull.Value ? "N/A" : reader["Description"].ToString())}");
            Console.WriteLine($"Duration: {reader["DurationMinutes"]} minutes");
            Console.WriteLine($"Price: ${Convert.ToDecimal(reader["Price"]):F2}");
        }

        public void Update(int serviceId, string serviceName, string? description, int durationMinutes, decimal price)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                UPDATE Tbl_SpaService 
                SET ServiceName = @ServiceName, Description = @Description, DurationMinutes = @DurationMinutes, Price = @Price 
                WHERE ServiceId = @ServiceId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ServiceId", serviceId);
            cmd.Parameters.AddWithValue("@ServiceName", serviceName);
            cmd.Parameters.AddWithValue("@Description", (object?)description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DurationMinutes", durationMinutes);
            cmd.Parameters.AddWithValue("@Price", price);

            connection.Open();
            int rows = cmd.ExecuteNonQuery();
            string message = rows > 0 ? "[ADO.NET] Service updated successfully." : "Service not found!";
            Console.WriteLine(message);
        }

        public void Delete(int serviceId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "UPDATE Tbl_SpaService SET DeleteFlag = 1 WHERE ServiceId = @ServiceId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ServiceId", serviceId);

            connection.Open();
            int rows = cmd.ExecuteNonQuery();
            string message = rows > 0 ? "[ADO.NET] Service deleted successfully." : "Service not found!";
            Console.WriteLine(message);
        }
    }
}
