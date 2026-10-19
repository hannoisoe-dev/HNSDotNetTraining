using System;
using System.Data;
using System.Data.SqlClient;

namespace HNSDotNetTraining.ConsoleApp
{
    public class TherapistManagementAdoDotNet : ITherapistManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void Create(string therapistName, string specialty, string workingHours = "09:00 - 18:00")
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                INSERT INTO Tbl_Therapist (TherapistName, Specialty, WorkingHours, IsAvailable, DeleteFlag)
                VALUES (@TherapistName, @Specialty, @WorkingHours, 1, 0);
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TherapistName", therapistName);
            cmd.Parameters.AddWithValue("@Specialty", specialty);
            cmd.Parameters.AddWithValue("@WorkingHours", workingHours);

            connection.Open();
            object newId = cmd.ExecuteScalar();
            Console.WriteLine($"[ADO.NET] Therapist '{therapistName}' registered successfully with ID: {newId} (Specialty: {specialty}).");
        }

        public void Read()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "SELECT TherapistId, TherapistName, Specialty, WorkingHours, IsAvailable FROM Tbl_Therapist WHERE DeleteFlag = 0 ORDER BY TherapistId";

            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No therapists found.");
                return;
            }

            Console.WriteLine("==================== Therapist List (ADO.NET) ====================");
            foreach (DataRow row in dt.Rows)
            {
                bool isAvail = Convert.ToBoolean(row["IsAvailable"]);
                Console.WriteLine($"ID: {row["TherapistId"]} | Name: {row["TherapistName"]} | Specialty: {row["Specialty"]} | Hours: {row["WorkingHours"]} | Status: {(isAvail ? "Available" : "Not Available")}");
            }
            Console.WriteLine("==================================================================");
        }

        public void Edit(int therapistId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "SELECT TherapistId, TherapistName, Specialty, WorkingHours, IsAvailable FROM Tbl_Therapist WHERE TherapistId = @TherapistId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TherapistId", therapistId);

            connection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                Console.WriteLine($"Therapist with ID {therapistId} not found!");
                return;
            }

            Console.WriteLine($"Therapist ID: {reader["TherapistId"]}");
            Console.WriteLine($"Name: {reader["TherapistName"]}");
            Console.WriteLine($"Specialty: {reader["Specialty"]}");
            Console.WriteLine($"Working Hours: {reader["WorkingHours"]}");
            Console.WriteLine($"Availability: {(Convert.ToBoolean(reader["IsAvailable"]) ? "Available" : "Unavailable")}");
        }

        public void Update(int therapistId, string therapistName, string specialty, string workingHours, bool isAvailable)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                UPDATE Tbl_Therapist 
                SET TherapistName = @TherapistName, Specialty = @Specialty, WorkingHours = @WorkingHours, IsAvailable = @IsAvailable 
                WHERE TherapistId = @TherapistId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TherapistId", therapistId);
            cmd.Parameters.AddWithValue("@TherapistName", therapistName);
            cmd.Parameters.AddWithValue("@Specialty", specialty);
            cmd.Parameters.AddWithValue("@WorkingHours", workingHours);
            cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);

            connection.Open();
            int rows = cmd.ExecuteNonQuery();
            string message = rows > 0 ? "[ADO.NET] Therapist updated successfully." : "Therapist not found!";
            Console.WriteLine(message);
        }

        public void Delete(int therapistId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "UPDATE Tbl_Therapist SET DeleteFlag = 1 WHERE TherapistId = @TherapistId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TherapistId", therapistId);

            connection.Open();
            int rows = cmd.ExecuteNonQuery();
            string message = rows > 0 ? "[ADO.NET] Therapist deleted successfully." : "Therapist not found!";
            Console.WriteLine(message);
        }
    }
}
