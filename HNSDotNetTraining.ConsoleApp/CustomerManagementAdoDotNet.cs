using System;
using System.Data;
using System.Data.SqlClient;

namespace HNSDotNetTraining.ConsoleApp
{
    public class CustomerManagementAdoDotNet : ICustomerManagement
    {
        private readonly string _connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;TrustServerCertificate=true";

        public void Create(string customerName, string phoneNumber, string? email)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                INSERT INTO Tbl_Customer (CustomerName, PhoneNumber, Email, DeleteFlag)
                VALUES (@CustomerName, @PhoneNumber, @Email, 0);
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@CustomerName", customerName);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);

            connection.Open();
            object newId = cmd.ExecuteScalar();
            Console.WriteLine($"[ADO.NET] Customer '{customerName}' registered successfully with ID: {newId}");
        }

        public void Read()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "SELECT CustomerId, CustomerName, PhoneNumber, Email FROM Tbl_Customer WHERE DeleteFlag = 0 ORDER BY CustomerId";

            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }

            Console.WriteLine("==================== Customer List (ADO.NET) ====================");
            foreach (DataRow row in dt.Rows)
            {
                string email = row["Email"] == DBNull.Value ? "N/A" : row["Email"].ToString()!;
                Console.WriteLine($"ID: {row["CustomerId"]} | Name: {row["CustomerName"]} | Phone: {row["PhoneNumber"]} | Email: {email}");
            }
            Console.WriteLine("==================================================================");
        }

        public void Edit(int customerId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = "SELECT CustomerId, CustomerName, PhoneNumber, Email FROM Tbl_Customer WHERE CustomerId = @CustomerId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);

            connection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                Console.WriteLine($"Customer with ID {customerId} not found!");
                return;
            }

            Console.WriteLine($"Customer ID: {reader["CustomerId"]}");
            Console.WriteLine($"Name: {reader["CustomerName"]}");
            Console.WriteLine($"Phone: {reader["PhoneNumber"]}");
            Console.WriteLine($"Email: {(reader["Email"] == DBNull.Value ? "N/A" : reader["Email"].ToString())}");
        }

        public void Update(int customerId, string customerName, string phoneNumber, string? email)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string query = @"
                UPDATE Tbl_Customer 
                SET CustomerName = @CustomerName, PhoneNumber = @PhoneNumber, Email = @Email 
                WHERE CustomerId = @CustomerId AND DeleteFlag = 0";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@CustomerName", customerName);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);

            connection.Open();
            int rows = cmd.ExecuteNonQuery();
            string message = rows > 0 ? "[ADO.NET] Customer updated successfully." : "Customer not found!";
            Console.WriteLine(message);
        }

        public void ViewHistory(int customerId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            string customerQuery = "SELECT CustomerName, PhoneNumber FROM Tbl_Customer WHERE CustomerId = @CustomerId AND DeleteFlag = 0";
            SqlCommand custCmd = new SqlCommand(customerQuery, connection);
            custCmd.Parameters.AddWithValue("@CustomerId", customerId);

            connection.Open();
            string custName = "";
            string phone = "";
            using (SqlDataReader custReader = custCmd.ExecuteReader())
            {
                if (!custReader.Read())
                {
                    Console.WriteLine($"Customer with ID {customerId} not found!");
                    return;
                }
                custName = custReader["CustomerName"].ToString()!;
                phone = custReader["PhoneNumber"].ToString()!;
            }

            string historyQuery = @"
                SELECT a.AppointmentId, a.AppointmentDate, a.StartTime, a.EndTime, a.Status, a.PaymentStatus,
                       s.ServiceName, s.Price,
                       t.TherapistName
                FROM Tbl_Appointment a
                INNER JOIN Tbl_SpaService s ON a.ServiceId = s.ServiceId
                INNER JOIN Tbl_Therapist t ON a.TherapistId = t.TherapistId
                WHERE a.CustomerId = @CustomerId AND a.DeleteFlag = 0
                ORDER BY a.AppointmentDate DESC, a.StartTime DESC";

            SqlCommand histCmd = new SqlCommand(historyQuery, connection);
            histCmd.Parameters.AddWithValue("@CustomerId", customerId);

            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(histCmd);
            adapter.Fill(dt);

            Console.WriteLine($"\n================ Appointment History for {custName} (Phone: {phone}) [ADO.NET] ================");
            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No appointment history for this customer.");
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                DateTime dtVal = Convert.ToDateTime(row["AppointmentDate"]);
                TimeSpan st = (TimeSpan)row["StartTime"];
                TimeSpan et = (TimeSpan)row["EndTime"];
                decimal price = Convert.ToDecimal(row["Price"]);
                Console.WriteLine($"Appt #{row["AppointmentId"]} | Date: {dtVal:yyyy-MM-dd} {st:hh\\:mm}-{et:hh\\:mm} | Service: {row["ServiceName"]} (${price:F2}) | Therapist: {row["TherapistName"]} | Status: {row["Status"]} | Payment: {row["PaymentStatus"]}");
            }
            Console.WriteLine("===================================================================================================");
        }
    }
}
