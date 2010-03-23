using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Threading;

namespace AnalyzeData
{
    class DatabaseReplicator
    {
        public delegate IEnumerable<string> GetInsertStmt(object[] args);

        private static SQLiteConnection _sqliteConnection;
        private static SqlConnection _sqlConnection;

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var builder = new SQLiteConnectionStringBuilder {DataSource = @"D:\!\Database\quik_sqlite.db3"};
            _sqliteConnection = new SQLiteConnection(builder.ConnectionString);
            _sqliteConnection.Open();

            var sqlBuilder = new SqlConnectionStringBuilder
                              {
                                  DataSource = "localhost",
                                  InitialCatalog = "QUIK",
                                  IntegratedSecurity = false,
                                  UserID = "Misha2Kota",
                                  Password = "2KotaSignature"
                              };
            _sqlConnection = new SqlConnection(sqlBuilder.ConnectionString);
            _sqlConnection.Open();

        }

        private void ReplicateDatabase()
        {
            UpdateTable("FutLimits");
            UpdateTable("MicexLimits");
            UpdateTable("Orders");
            UpdateTable("Portfolio");
            UpdateTable("Stakan_EDM0");
            UpdateTable("Stakan_EuM0");
            UpdateTable("Stakan_SiM0");
            UpdateTable("Trades");
        }

        private static IEnumerable<string> GetInsertStatements(object[] args)
        {
            foreach (var o in args)
            {
                if(o == DBNull.Value)
                {
                    yield return "NULL";
                }
                else if(o.GetType() == typeof(string))
                {
                    yield return "'" + o + "'";
                }
                else
                {
                    yield return o.ToString();
                }
            }
        }

        private static void UpdateTable(string tableName)
        {
            string inputTableName = tableName;
            if(inputTableName == "Trades")
            {
                inputTableName = "Transactions";
            }
            var sqlCommand = new SqlCommand(
                "select * from " + inputTableName,
                _sqlConnection);

            var reader = sqlCommand.ExecuteReader();

            using (var t = _sqliteConnection.BeginTransaction())
            {
                var sqliteCommand = new SQLiteCommand("delete from " + tableName, _sqliteConnection, t);
                sqliteCommand.ExecuteNonQuery();

                while (reader.Read())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);

                    var outVals = GetInsertStatements(values);

                    var bld = new StringBuilder("insert into ");
                    bld.Append(tableName);
                    bld.Append(" values (");
                    bool first = true;
                    foreach (var elem in outVals)
                    {
                        if (!first)
                        {
                            bld.Append(", ");
                        }
                        bld.Append(elem);
                        first = false;
                    }
                    bld.Append(")");

                    sqliteCommand.CommandText = bld.ToString();
                    sqliteCommand.ExecuteNonQuery();                    
                }

                t.Commit();
            }

            reader.Close();
        }
    }
}
