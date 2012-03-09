//#define DebugOutput

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Npgsql;
using System.Data;
using System.Diagnostics;

namespace SecurityPgSqlAdapter
{
    public class PostgreSQLConn
    {
        /// <summary>
        /// Creates the connection an directly opens it.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev11, 2008-07-24</remarks>
        public static NpgsqlConnection CreateConnection(string connectionString)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Executes the query and returns the affected rows.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-11-13</remarks>
        public static int ExecuteNonQuery(NpgsqlCommand command)
        {
#if DEBUG && DebugOutput
            WriteDebugInfos(command);
#endif
            int count = command.ExecuteNonQuery();
            return count;
        }

        /// <summary>
        /// Executes the query and returns a reader.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-11-13</remarks>
        public static NpgsqlDataReader ExecuteReader(NpgsqlCommand command)
        {
#if DEBUG && DebugOutput
            WriteDebugInfos(command);
#endif
            NpgsqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
            return reader;
        }

        /// <summary>
        /// Executes the query and returns the first cell.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-11-13</remarks>
        public static object ExecuteScalar(NpgsqlCommand command)
        {
#if DEBUG && DebugOutput
            WriteDebugInfos(command);
#endif
            object obj = command.ExecuteScalar();
            return obj;
        }

        /// <summary>
        /// Executes the query and returns the first cell in its nullable representative.
        /// </summary>
        /// <typeparam name="T">The type from which the nullable representative is returned.</typeparam>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-08-12</remarks>
        public static T? ExecuteScalar<T>(NpgsqlCommand command) where T : struct
        {
            object res = ExecuteScalar(command);
            return (res == null || res is DBNull) ? (T?)null : (T?)Convert.ChangeType(res, typeof(T));
        }

        /// <summary>
        /// Writes the debug infos.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <remarks>Documented by Dev02, 2008-07-30</remarks>
        private static void WriteDebugInfos(NpgsqlCommand command)
        {
            string str = string.Format("Query ({0}): {1}", DateTime.Now.ToString("HH:mm:ss"), command.CommandText);
            if (command.Parameters.Count > 0)
            {
                str += "\tParameters:";
                foreach (NpgsqlParameter parameter in command.Parameters)
                    str += string.Format(" {0}={1};", parameter.ParameterName, parameter.Value);
            }
            Debug.WriteLine(str);
        }
    }
}
