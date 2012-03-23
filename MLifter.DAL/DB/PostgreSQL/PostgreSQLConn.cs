/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
//# define debug_output

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Npgsql;
using System.Data;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.Generics;

namespace MLifter.DAL.DB.PostgreSQL
{
	/// <summary>
	/// This class manages PgSQL connections and queries.
	/// </summary>
	public class PostgreSQLConn
	{
		private static Cache cache = new Cache(15);

		/// <summary>
		/// Creates the connection an directly opens it.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev11, 2008-07-24</remarks>
		public static NpgsqlConnection CreateConnection(IUser user)
		{
			NpgsqlConnection conn = new NpgsqlConnection(user.ConnectionString.ConnectionString);
			conn.Open();
			return conn;
		}

		/// <summary>
		/// Executes the query and returns the affected rows.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		public static int ExecuteNonQuery(NpgsqlCommand command, IUser user)
		{
			return ExecuteNonQuery(command, user, true);
		}

		/// <summary>
		/// Executes the query and returns the affected rows.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <param name="checkSession">if set to <c>true</c> [check session].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-07-25</remarks>
		public static int ExecuteNonQuery(NpgsqlCommand command, IUser user, bool checkSession)
		{
			if (checkSession)
				if (!CheckSession(command, user))
					return -1;

#if DEBUG && debug_output
			WriteDebugInfos(command);
#endif
			int count = command.ExecuteNonQuery();
			return count;
		}

		/// <summary>
		/// Executes the query and returns a reader.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		public static NpgsqlDataReader ExecuteReader(NpgsqlCommand command, IUser user)
		{
			return ExecuteReader(command, user, true);
		}

		/// <summary>
		/// Executes the query and returns a reader.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <param name="checkSession">if set to <c>true</c> [check session].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-07-25</remarks>
		public static NpgsqlDataReader ExecuteReader(NpgsqlCommand command, IUser user, bool checkSession)
		{
			if (checkSession)
				if (!CheckSession(command, user))
					return null;

#if DEBUG && debug_output
			WriteDebugInfos(command);
#endif
			NpgsqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
			return reader;
		}

		/// <summary>
		/// Executes the query and returns the first cell.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		public static object ExecuteScalar(NpgsqlCommand command, IUser user)
		{
			return ExecuteScalar(command, user, true);
		}

		/// <summary>
		/// Executes the query and returns the first cell.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <param name="checkSession">if set to <c>true</c> [check session].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-07-25</remarks>
		public static object ExecuteScalar(NpgsqlCommand command, IUser user, bool checkSession)
		{
			if (checkSession)
				if (!CheckSession(command, user))
					return null;

#if DEBUG && debug_output
			WriteDebugInfos(command);
#endif
			object obj = command.ExecuteScalar();
			return obj;
		}

		/// <summary>
		/// Executes the query and returns the first cell in its nullable representative.
		/// </summary>
		/// <typeparam name="T">The type from which the nullable representative is returned.</typeparam>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by CFI, 2008-08-12.
		/// </remarks>
		public static T? ExecuteScalar<T>(NpgsqlCommand command, IUser user) where T : struct
		{
			return ExecuteScalar<T>(command, user, true);
		}

		/// <summary>
		/// Executes the query and returns the first cell in its nullable representative.
		/// </summary>
		/// <typeparam name="T">The type from which the nullable representative is returned.</typeparam>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <param name="checkSession">if set to <c>true</c> check session.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-08-12</remarks>
		public static T? ExecuteScalar<T>(NpgsqlCommand command, IUser user, bool checkSession) where T : struct
		{
			object res = ExecuteScalar(command, user, checkSession);
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

		/// <summary>
		/// Adds the session check to the sql command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by DAC, 2008-11-13.
		/// </remarks>
		public static bool CheckSession(NpgsqlCommand command, IUser user)
		{
			if (user == null || user.Parent == null || user.Parent.CurrentSessionId == null)
				return false;

			bool? alive = cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.SessionAlive, user.Id)] as bool?;
			if (alive.HasValue)
				return alive.Value;

			NpgsqlCommand checkcommand = new NpgsqlCommand("SELECT \"CheckUserSession\"(:sessionid);");
			checkcommand.Connection = command.Connection;
			checkcommand.Parameters.Add("sessionid", user.Parent.CurrentSessionId.ToString());

#if DEBUG && debug_output
			WriteDebugInfos(checkcommand);
#endif

			CheckSessionInvalid(checkcommand.ExecuteScalar());

			cache[ObjectLifetimeIdentifier.Create(CacheObject.SessionAlive, user.Id, new TimeSpan(0, 0, 1))] = true;
			return true;
		}

		/// <summary>
		/// Checks if the session is invalid.
		/// </summary>
		/// <param name="obj">The DB response.</param>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		private static void CheckSessionInvalid(object obj)
		{
			if (obj == null)
				return;

			if (obj is string && ((string)obj) == "SessionInvalid")
				throw new UserSessionInvalidException("The user session was invalidated by the server.");
		}

		/// <summary>
		/// The version of the database which is required.
		/// </summary>
		public static readonly Version RequiredDatabaseVersion = new Version(1, 0, 2);
	}
}
