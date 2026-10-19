using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNSDotNetTraining.ConsoleApp
{
    public class AdoDotNetExample
    {
        private readonly string connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;";
        public void Read()
        {
            
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "SELECT * FROM Tbl_Blog where DeleteFlag = 0";
            SqlCommand command = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
            connection.Close();

            //----Using DataAdapter---- -
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine($"BlogId: {row["BlogId"]}, BlogTitle: {row["BlogTitle"]}, BlogContent: {row["BlogContent"]}");
            };
            
            //----Using DataReader-------
            //SqlDataReader reader = command.ExecuteReader();
            //while (reader.Read())
            //{
            //    Console.WriteLine($"BlogId: {reader["BlogId"]}, BlogTitle: {reader["BlogTitle"]}, BlogContent: {reader["BlogContent"]}");
            //};
        }

        public void Create()
        {
            Console.Write("Enter Blog Title: ");
            string blogTitle = Console.ReadLine();
            Console.Write("Enter Blog Author: ");
            string blogAuthor = Console.ReadLine();
            Console.Write("Enter Blog Content: ");
            string blogContent = Console.ReadLine();

            //-----SQL Injection Example------
            //string connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;";
            //SqlConnection connection = new SqlConnection(connectionString);
            //connection.Open();
            //string query = $@"INSERT INTO [dbo].[Tbl_Blog]
            //           ([BlogTitle]
            //           ,[BlogAuthor]
            //           ,[BlogContent]
            //           ,[DeleteFlag])
            //     VALUES
            //           ('{blogTitle}'
            //           ,'{blogAuthor}'
            //           ,'{blogContent}'
            //           ,0);";


            //string connectionString = "Data Source=.;Initial Catalog=HNSDotNetTraining;User ID=sa;Password=sasa@123;";
            //SqlConnection connection = new SqlConnection(connectionString);
            //connection.Open();
            //SqlCommand command = new SqlCommand(query, connection);
            //SqlDataAdapter adaper = new SqlDataAdapter(command);
            //DataTable dt = new DataTable();
            //adaper.Fill(dt);
            //connection.Close();

            string query = $@"INSERT INTO [dbo].[Tbl_Blog]
             ([BlogTitle]
            ,[BlogAuthor]
            ,[BlogContent]
            ,[DeleteFlag])
    VALUES
             (@BlogTitle
             ,@BlogAuthor
            ,@BlogContent
             ,0);";


            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BlogTitle", blogTitle);
            command.Parameters.AddWithValue("@BlogAuthor", blogAuthor);
            command.Parameters.AddWithValue("@BlogContent", blogContent);

            int result = command.ExecuteNonQuery();

            //if(result == 1)
            //{
            //    Console.WriteLine("Saving successful.");
            //}
            //else
            //{
            //    Console.WriteLine("Saving failed.");
            //}

            //---ternary operator---
            string message = result == 1 ? "Saving successful." : "Saving failed.";
            Console.WriteLine(message);
            connection.Close();
        }

        public void Edit()
        {
            Console.Write("Enter Blog Id: ");
            string id = Console.ReadLine();

            string query = "select * from Tbl_Blog where BlogId = @BlogId and DeleteFlag=0";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BlogId", id);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
            connection.Close();
            if(dt.Rows.Count == 0)
            {
                Console.Write("No data found.");
                return;
            }

            DataRow dr = dt.Rows[0];
            Console.WriteLine($"Blog Title: {dr["BlogTitle"]}");
            Console.WriteLine($"Blog Author: {dr["BlogAuthor"]}");
            Console.WriteLine($"Blog Content: {dr["BlogContent"]}");

        }

        public void Update()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            Console.Write("Enter Blog Id: ");
            string id = Console.ReadLine();
            string query = "SELECT * FROM Tbl_Blog where BlogId=@BlogId and DeleteFlag = 0";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BlogId", id);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
            connection.Close();

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No data found.");
                return;
            }
            Console.Write("Enter Blog Title: ");
            string blogTitle = Console.ReadLine();
            Console.Write("Enter Blog Author: ");
            string blogAuthor = Console.ReadLine();
            Console.Write("Enter Blog Content: ");
            string blogContent = Console.ReadLine();

            string updateQuery = @"UPDATE [dbo].[Tbl_Blog]
                                   SET [BlogTitle] = @BlogTitle
                                      ,[BlogAuthor] = @BlogAuthor
                                      ,[BlogContent] = @BlogContent
                                      ,[DeleteFlag] = 0
                                 WHERE BlogId = @BlogId";

            connection.Open();
            SqlCommand updateCommand=new SqlCommand(updateQuery, connection);

            updateCommand.Parameters.AddWithValue("@BlogTitle", blogTitle);
            updateCommand.Parameters.AddWithValue("@BlogAuthor", blogAuthor);
            updateCommand.Parameters.AddWithValue("@BlogContent", blogContent);
            updateCommand.Parameters.AddWithValue("@BlogId", id);

            int result =updateCommand.ExecuteNonQuery();
            connection.Close();
            string message = result == 1 ? "Update successful." : "Update failed.";

            Console.WriteLine(message);

        }

        public void Delete()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            Console.Write("Enter Blog Id: ");
            string id = Console.ReadLine();
            string query = "SELECT * FROM Tbl_Blog where BlogId=@BlogId and DeleteFlag = 0";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BlogId", id);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dt);
            connection.Close();

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No data found.");
                return;
            }
           
            Console.WriteLine("Are you sure you wanna delete (y/n)?");
            string decision = Console.ReadLine();

            if(decision.ToUpper() == "Y")
            {
                string updateQuery = @"UPDATE [dbo].[Tbl_Blog]
                                   SET [DeleteFlag] = 1
                                 WHERE BlogId = @BlogId";

                connection.Open();
                SqlCommand cmd = new SqlCommand(updateQuery, connection);
                cmd.Parameters.AddWithValue("@BlogId", id);
                int result=cmd.ExecuteNonQuery();
                connection.Close();

                string message = result == 1 ? "Delete successful." : "Delete failed.";
                Console.WriteLine(message);
                return;
            }

            
        }
    }
}
