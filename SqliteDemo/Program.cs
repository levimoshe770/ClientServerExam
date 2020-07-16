using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection conn = CreateConnection();
            Console.WriteLine("Status: {0}", conn.State);
            CreateTable(conn);

            Console.ReadKey();
        }

        private static void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand command = conn.CreateCommand();
            string createTbl = string.Format("CREATE TABLE {0} ({1}, {2})",
                "USERTABLE",
                "USERNAME VARCHAR(50)",
                "PASSWORD VARCHAR(50)"
                );
            command.CommandText = createTbl;
            command.ExecuteNonQuery();
        }

        static SQLiteConnection CreateConnection()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True;");
            try
            {
                conn.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine("{0}", e.Message);
            }

            return conn;

        }
    }
}
