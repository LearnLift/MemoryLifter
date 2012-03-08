using System;
using System.Collections.Generic;
using System.Web;
using Npgsql;
using System.Configuration;
using MLifter.DAL;

namespace MLifterSyncService
{
    internal static class FileHandlerHelpers
    {
        /// <summary>
        /// Gets the pg connection.
        /// </summary>
        /// <returns></returns>
        internal static NpgsqlConnection GetPgConnection()
        {
            NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString);
            conn.Open();
            return conn;
        }


        /// <summary>
        /// Logins the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal static int Login(string username, string password)
        {
            using (NpgsqlConnection con = GetPgConnection())
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    //check user name
                    cmd.CommandText = "SELECT id FROM \"UserProfiles\" WHERE username=:user;";
                    cmd.Parameters.Add("user", username);

                    object result = cmd.ExecuteScalar();
                    int uid = (result == null || result is DBNull) ? -1 : Convert.ToInt32(result);

                    if (uid >= 0)
                    {

                        if (password.Length > 0)
                        {
                            //check user password
                            cmd.CommandText = "SELECT id FROM \"UserProfiles\" WHERE id=:userid AND password=:pass";
                            cmd.Parameters.Add("pass", password);
                            cmd.Parameters.Add("userid", uid);
                            result = cmd.ExecuteScalar();
                            uid = (result == null || result is DBNull) ? -2 : Convert.ToInt32(result);
                        }
                        else
                        {
                            //check authentication type
                            cmd.CommandText = "SELECT id FROM \"UserProfiles\" WHERE id=:userid AND user_type<>:usertype";
                            cmd.Parameters.Add("userid", uid);
                            cmd.Parameters.Add("usertype", UserAuthenticationTyp.FormsAuthentication.ToString());
                            result = cmd.ExecuteScalar();
                            uid = (result == null || result is DBNull) ? -4 : Convert.ToInt32(result);
                        }
                    }

                    return uid;
                }
            }
        }
    }
}
