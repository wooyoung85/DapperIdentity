using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DapperIdentity.Data.Connections
{
    public class DbManager : IDisposable
    {
        private IDbConnection con { get; set; }

        /// <summary>
        /// Return open connection
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                return con;
            }
        }

        /// <summary>
        /// Create a new Sql database connection
        /// </summary>
        /// <param name="connString">The name of the connection string</param>
        public DbManager(string connString)
        {
            // Use first?
            if (connString == "")
                connString = ConfigurationManager.ConnectionStrings[0].Name;

            con = new SqlConnection(connString);
        }

        /// <summary>
        /// Close and dispose of the database connection
        /// </summary>
        public void Dispose()
        {
            if (con != null)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                con = null;
            }
        }
    }
}
