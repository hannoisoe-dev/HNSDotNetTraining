using Dapper;
using HNSDotNetTraining.ConsoleApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HNSDotNetTraining.ConsoleApp
{
    public class TherapistManagementDapper : ITherapistManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void Create(string therapistName, string specialty, string workingHours = "09:00 - 18:00")
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                INSERT INTO Tbl_Therapist (TherapistName, Specialty, WorkingHours, IsAvailable, DeleteFlag)
                VALUES (@TherapistName, @Specialty, @WorkingHours, 1, 0);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            int newId = db.QuerySingle<int>(sql, new { TherapistName = therapistName, Specialty = specialty, WorkingHours = workingHours });
            Console.WriteLine($"[Dapper] Therapist '{therapistName}' registered successfully with ID: {newId} (Specialty: {specialty}).");
        }

        public void Read()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT TherapistId, TherapistName, Specialty, WorkingHours, IsAvailable FROM Tbl_Therapist WHERE DeleteFlag = 0 ORDER BY TherapistId";
            var therapists = db.Query<TherapistDataModel>(sql).ToList();

            if (!therapists.Any())
            {
                Console.WriteLine("No therapists found.");
                return;
            }

            Console.WriteLine("==================== Therapist List (Dapper) ====================");
            foreach (var t in therapists)
            {
                string availStr = t.IsAvailable ? "Available" : "Not Available";
                Console.WriteLine($"ID: {t.TherapistId} | Name: {t.TherapistName} | Specialty: {t.Specialty} | Hours: {t.WorkingHours} | Status: {availStr}");
            }
            Console.WriteLine("=================================================================");
        }

        public void Edit(int therapistId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "SELECT TherapistId, TherapistName, Specialty, WorkingHours, IsAvailable FROM Tbl_Therapist WHERE TherapistId = @TherapistId AND DeleteFlag = 0";
            var therapist = db.QueryFirstOrDefault<TherapistDataModel>(sql, new { TherapistId = therapistId });

            if (therapist == null)
            {
                Console.WriteLine($"Therapist with ID {therapistId} not found!");
                return;
            }

            Console.WriteLine($"Therapist ID: {therapist.TherapistId}");
            Console.WriteLine($"Name: {therapist.TherapistName}");
            Console.WriteLine($"Specialty: {therapist.Specialty}");
            Console.WriteLine($"Working Hours: {therapist.WorkingHours}");
            Console.WriteLine($"Availability: {(therapist.IsAvailable ? "Available" : "Unavailable")}");
        }

        public void Update(int therapistId, string therapistName, string specialty, string workingHours, bool isAvailable)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                UPDATE Tbl_Therapist 
                SET TherapistName = @TherapistName, Specialty = @Specialty, WorkingHours = @WorkingHours, IsAvailable = @IsAvailable 
                WHERE TherapistId = @TherapistId AND DeleteFlag = 0";

            int rows = db.Execute(sql, new { TherapistId = therapistId, TherapistName = therapistName, Specialty = specialty, WorkingHours = workingHours, IsAvailable = isAvailable });
            string message = rows > 0 ? "[Dapper] Therapist updated successfully." : "Therapist not found!";
            Console.WriteLine(message);
        }

        public void Delete(int therapistId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = "UPDATE Tbl_Therapist SET DeleteFlag = 1 WHERE TherapistId = @TherapistId AND DeleteFlag = 0";

            int rows = db.Execute(sql, new { TherapistId = therapistId });
            string message = rows > 0 ? "[Dapper] Therapist deleted successfully." : "Therapist not found!";
            Console.WriteLine(message);
        }
    }
}
