using System;
using System.Data.SqlClient;
using System.Text;

namespace sqltest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "sje-sql-server.database.windows.net";
                builder.UserID = "ServerAdm";
                builder.Password = "Kodeord1";
                builder.InitialCatalog = "SjeFirstDatabase";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT TOP 20 * ");
                    sb.Append("FROM [dbo].[People] ");
                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader sdr = command.ExecuteReader())
                        {
                            var count = sdr.FieldCount;
                            while (sdr.Read())
                            {
                                var v1 = sdr["_sqlid"] == DBNull.Value ? -1 : Convert.ToInt32(sdr["_sqlid"].ToString().Trim());
                                var v2 = sdr["transNo"] == DBNull.Value ? string.Empty : sdr["transNo"].ToString().Trim();
                                var v3 = sdr["source"] == DBNull.Value ? string.Empty : sdr["source"].ToString().Trim();
                                var v4 = sdr["status"] == DBNull.Value ? string.Empty : sdr["status"].ToString().Trim();

                                Console.WriteLine($"{v1}\t{v2}\t{v3}\t{v4}");
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}