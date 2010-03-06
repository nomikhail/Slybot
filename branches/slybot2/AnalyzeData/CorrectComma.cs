using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AnalyzeData
{
    class CorrectComma
    {
        static string query = @"select * from log
                        where logger = 'stakans' and message like 'ed%(1,%'
                        order by id";

        public static void Correct()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "localhost";
            builder.InitialCatalog = "SlyBot";
            builder.IntegratedSecurity = false;
            builder.UserID = "Misha2Kota";
            builder.Password = "2KotaSignature";

            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);
            var reader = command.ExecuteReader();

            Dictionary<int, string> newMsgs = new Dictionary<int, string>();


            while (reader.Read())
            {
                string message = reader["Message"].ToString();

                int id = (int)reader["Id"];

                string newMsg = message.Replace("(1,", "(1.");

                newMsgs[id] = newMsg;
            }
            reader.Close();

            int total = 0;
            int qsize = 0;
            StringBuilder qbuilder = new StringBuilder();
            foreach (var pair in newMsgs)
            {
                string updquery = "update log set message = '" + pair.Value + "' where id = " + pair.Key;
                qbuilder.AppendLine(updquery);

                ++total;
                ++qsize;

                if (qsize == 1)
                {
                    SqlCommand updcommand = new SqlCommand(qbuilder.ToString(), connection);
                    var lines = updcommand.ExecuteNonQuery();
                    if (lines != qsize)
                        Debugger.Break();

                    qsize = 0;
                    qbuilder.Length = 0;

                    Console.WriteLine(total);
                }
            }

            Console.WriteLine(total);
            Console.ReadLine();
        }
    }
}
