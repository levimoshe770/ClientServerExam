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
            Logger.Logger.Log("NonQuery: {0}", pSqlCommand);

            using (m_Conn = OpenConnection())
            {
                using (SQLiteCommand cmd = m_Conn.CreateCommand())
                {
                    cmd.CommandText = pSqlCommand;
                    cmd.ExecuteNonQuery();
                }

                m_Conn.Close();
            }
        }

        public SQLiteDataReader ExecuteReader(string pSqlCommand)
        {
            Logger.Logger.Log("ExecuteReader: {0}", pSqlCommand);
            m_Conn = OpenConnection();
            SQLiteCommand cmd = m_Conn.CreateCommand();
            cmd.CommandText = pSqlCommand;
            return cmd.ExecuteReader();
        }

        public SQLiteDataAdapter ExecuteAdapter(string pSqlCommand)
        {
            Logger.Logger.Log("ExecuteAdapter: {0}", pSqlCommand);
            m_Conn = OpenConnection();
            SQLiteCommand cmd = m_Conn.CreateCommand();
            cmd.CommandText = pSqlCommand;
            return new SQLiteDataAdapter(cmd);
        }

        public void CloseConnection()
        {
            Logger.Logger.Log("Connection close");
            m_Conn.Close();
            m_Conn.Dispose();
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

            dr.Close();
            dr.Dispose();
            CloseConnection();

            return cnt > 0;
        }

        private SQLiteConnection OpenConnection()
        {
            Logger.Logger.Log("Connection open");
            SQLiteConnection conn = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True");
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
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
