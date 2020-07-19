using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
//using System.Data.Entity.Infrastructure.Interception;

namespace DataLayer
{
    internal class DataServices
    {
        public void ExecuteNonQuery (string pSqlCommand)
        {
            m_Conn = OpenConnection();
            SQLiteCommand cmd = m_Conn.CreateCommand();
            cmd.CommandText = pSqlCommand;         
            cmd.ExecuteNonQuery();
            m_Conn.Close();
        }

        public SQLiteDataReader ExecuteReader(string pSqlCommand)
        {
            m_Conn = OpenConnection();
            SQLiteCommand cmd = m_Conn.CreateCommand();
            cmd.CommandText = pSqlCommand;
            return cmd.ExecuteReader();
        }

        public void CloseConnection()
        {
            m_Conn.Close();
        }

        public bool TableExists(string pTableName)
        {
            string cmd = string.Format("SELECT COUNT(1) FROM SQLITE_MASTER WHERE TYPE = 'table' AND NAME = '{0}'", pTableName);

            SQLiteDataReader dr = ExecuteReader(cmd);
            int cnt = 0;
            while (dr.Read())
            {
                cnt = dr.GetInt32(0);
            }

            return cnt > 0;
        }

        private SQLiteConnection OpenConnection()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True");
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Logger.Logger.Log(string.Format("Failed to open connection: {0}", e.Message));
            }

            return conn;
        }

        private SQLiteConnection m_Conn;
    }
}
