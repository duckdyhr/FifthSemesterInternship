using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing console...");
            var connection = getConnection();
            try
            {
                connection.Open();
                string query = "select name from Student";
                var command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader["name"] as string);
//                    Console.WriteLine("id " + reader["id"]);
                }
            }
            finally
            {
                connection.Close();
            }
            Console.Read();
        }
        private static SqlConnection getConnection()
        {
            string connStr = @"Data Source=localhost\SQLExpress; database=FifthSemester; user id=sa; password=anne";
            var connection = new SqlConnection(connStr);
            return connection;
        }
    }
}
