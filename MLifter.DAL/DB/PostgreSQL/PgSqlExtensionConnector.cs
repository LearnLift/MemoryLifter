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
using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.Generics;
using NpgsqlTypes;
using Npgsql;
using System.IO;

namespace MLifter.DAL.DB.PostgreSQL
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>Documented by Dev08, 2009-07-02</remarks>
	public class PgSqlExtensionConnector : IDbExtensionConnector
	{
		private static Dictionary<ConnectionStringStruct, PgSqlExtensionConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlExtensionConnector>();
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <param name="parentClass">The parent class.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-07-02</remarks>
		public static PgSqlExtensionConnector GetInstance(ParentClass parentClass)
		{
			ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

			if (!instances.ContainsKey(connection))
				instances.Add(connection, new PgSqlExtensionConnector(parentClass));

			return instances[connection];
		}

		private ParentClass Parent;
		private PgSqlExtensionConnector(ParentClass parentClass)
		{
			Parent = parentClass;
			Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
		}

		private void Parent_DictionaryClosed(object sender, EventArgs e)
		{
			IParent parent = sender as IParent;
			instances.Remove(parent.Parent.CurrentUser.ConnectionString);
		}
		/// <summary>
		/// The size of chunks to read from the db at once.
		/// </summary>
		private readonly int chunkSize = 204800; //200 KB

		/// <summary>
		/// Writes the content of a buffer into a LargeObject.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="largeObject">The large object.</param>
		/// <remarks>Documented by Dev02, 2008-08-08</remarks>
		private void BufferToLargeObject(byte[] buffer, LargeObject largeObject)
		{
			largeObject.Seek(0);

			int offset = 0;
			int size = buffer.Length;
			while (offset < size)
			{
				largeObject.Write(buffer, offset, Math.Min(chunkSize, size - offset));
				offset += chunkSize;
			}
		}

		/// <summary>
		/// Writes the content of a buffer into a LargeObject.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="largeObject">The large object.</param>
		/// <param name="rpu">The rpu.</param>
		/// <param name="caller">The calling object.</param>
		/// <remarks>Documented by Dev02, 2008-08-08</remarks>
		private void BufferToLargeObject(byte[] buffer, LargeObject largeObject, StatusMessageReportProgress rpu, object caller)
		{
			largeObject.Seek(0);

			int offset = 0;
			int size = buffer.Length;
			StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.CreateMediaProgress, buffer.Length);
			while (offset < size)
			{
				largeObject.Write(buffer, offset, Math.Min(chunkSize, size - offset));
				offset += chunkSize;
				args.Progress = offset;
				if (rpu != null)
					rpu(args, caller);
			}
		}

		/// <summary>
		/// Gets the contents of a LargeObject into a buffer.
		/// </summary>
		/// <param name="largeObject">The large object.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-08-08</remarks>
		private byte[] LargeObjectToBuffer(LargeObject largeObject)
		{
			largeObject.Seek(0);
			int size = largeObject.Size();
			byte[] buffer = new byte[size];

			int offset = 0;
			while (offset < size)
			{
				largeObject.Read(buffer, offset, Math.Min(chunkSize, size - offset));
				offset += chunkSize;
			}

			return buffer;
		}

		#region IDbExtensionConnector Members

		/// <summary>
		/// Adds the new extension.
		/// </summary>
		/// <returns></returns>
		public Guid AddNewExtension()
		{
			Guid newGuid = Guid.NewGuid();
			return AddNewExtension(newGuid);
		}

		/// <summary>
		/// Adds the new extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		public Guid AddNewExtension(Guid guid)
		{
			DeleteExtension(guid);

			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "INSERT INTO \"Extensions\" (guid, name, version, type, data) VALUES (:guid, '', '', :type, 0)";
					cmd.Parameters.Add("guid", guid.ToString());
					cmd.Parameters.Add("type", ExtensionType.Unknown.ToString());
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

					return guid;
				}
			}
		}

		/// <summary>
		/// Sets the extension LM.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="lmid">The lmid.</param>
		public void SetExtensionLM(Guid guid, int lmid)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "UPDATE \"Extensions\" SET lm_id=:lmid WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					cmd.Parameters.Add("lmid", lmid);
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}
			}
		}

		/// <summary>
		/// Deletes the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		public void DeleteExtension(Guid guid)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM \"Extensions\" WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}
			}
		}

		/// <summary>
		/// Gets the name of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		public string GetExtensionName(Guid guid)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "SELECT name FROM \"Extensions\" WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					return PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString();
				}
			}
		}

		/// <summary>
		/// Sets the name of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionName">Name of the extension.</param>
		public void SetExtensionName(Guid guid, string extensionName)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "UPDATE \"Extensions\" SET name=:name WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					cmd.Parameters.Add("name", extensionName);
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}
			}
		}

		/// <summary>
		/// Gets the extension version.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		public Version GetExtensionVersion(Guid guid)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "SELECT version FROM \"Extensions\" WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					return new Version(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString());
				}
			}
		}

		/// <summary>
		/// Sets the extension version.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="versionName">Name of the version.</param>
		public void SetExtensionVersion(Guid guid, Version versionName)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "UPDATE \"Extensions\" SET version=:version WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					cmd.Parameters.Add("version", versionName.ToString());
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}
			}
		}

		/// <summary>
		/// Gets the type of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		public ExtensionType GetExtensionType(Guid guid)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "SELECT type FROM \"Extensions\" WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					try
					{
						return (ExtensionType)Enum.Parse(typeof(ExtensionType), PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString());
					}
					catch (ArgumentException)
					{
						return ExtensionType.Unknown;
					}
				}
			}
		}

		/// <summary>
		/// Sets the type of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionType">Type of the extension.</param>
		public void SetExtensionType(Guid guid, ExtensionType extensionType)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "UPDATE \"Extensions\" SET type=:type WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					cmd.Parameters.Add("type", extensionType.ToString());
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}
			}
		}

		/// <summary>
		/// Gets the extension start file.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		public string GetExtensionStartFile(Guid guid)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "SELECT startfile FROM \"Extensions\" WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					return PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser).ToString();
				}
			}
		}

		/// <summary>
		/// Sets the extension start file.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="startFile">The start file.</param>
		public void SetExtensionStartFile(Guid guid, string startFile)
		{
			if (startFile == null)
				startFile = string.Empty;

			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "UPDATE \"Extensions\" SET startfile=:startfile WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					cmd.Parameters.Add("startfile", startFile);
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}
			}
		}

		/// <summary>
		/// Gets the extension stream.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		public Stream GetExtensionStream(Guid guid)
		{
			MemoryStream stream = null;
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				int noid = 0;
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "SELECT data FROM \"Extensions\" WHERE guid=:guid;";
					cmd.Parameters.Add("guid", guid.ToString());
					object obj = PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser);
					if (obj == null || obj == DBNull.Value || !(obj as long?).HasValue)
						return stream;
					noid = Convert.ToInt32((obj as long?).Value);
				}

				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					LargeObjectManager lbm = new LargeObjectManager(con);
					LargeObject largeObject = lbm.Open(noid, LargeObjectManager.READWRITE);
					byte[] buffer = LargeObjectToBuffer(largeObject);
					stream = new MemoryStream(buffer);
					largeObject.Close();
				}
				catch { }
				finally { tran.Commit(); }
			}
			return stream;
		}

		/// <summary>
		/// Sets the extension stream.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionStream">The extension stream.</param>
		public void SetExtensionStream(Guid guid, Stream extensionStream)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				NpgsqlTransaction tran = con.BeginTransaction();

				LargeObjectManager lbm = new LargeObjectManager(con);
				int noid = lbm.Create(LargeObjectManager.READWRITE);
				LargeObject largeObject = lbm.Open(noid, LargeObjectManager.READWRITE);
				byte[] buffer = new byte[extensionStream.Length];
				extensionStream.Read(buffer, 0, (int)extensionStream.Length);
				BufferToLargeObject(buffer, largeObject);
				largeObject.Close();

				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "UPDATE \"Extensions\" SET data=:data WHERE guid=:guid";
					cmd.Parameters.Add("data", noid);
					cmd.Parameters.Add("guid", guid.ToString());
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}
				tran.Commit();
			}
		}

		/// <summary>
		/// Determines whether the stream is available.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns>
		///   <c>true</c> if stream is available; otherwise, <c>false</c>.
		/// </returns>
		public bool IsStreamAvailable(Guid guid)
		{
			return true;
		}

		/// <summary>
		/// Gets the extension actions.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		public IList<ExtensionAction> GetExtensionActions(Guid guid)
		{
			List<ExtensionAction> actions = new List<ExtensionAction>();
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "SELECT action, execution FROM \"ExtensionActions\" WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
					while (reader.Read())
					{
						ExtensionAction action = new ExtensionAction();
						action.Kind = (ExtensionActionKind)Enum.Parse(typeof(ExtensionActionKind), Convert.ToString(reader["action"]));
						action.Execution = (ExtensionActionExecution)Enum.Parse(typeof(ExtensionActionExecution), Convert.ToString(reader["execution"]));
						actions.Add(action);
					}
				}
			}
			return actions;
		}

		/// <summary>
		/// Sets the extension actions.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionActions">The extension actions.</param>
		public void SetExtensionActions(Guid guid, IList<ExtensionAction> extensionActions)
		{
			using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
			{
				NpgsqlTransaction tran = con.BeginTransaction();

				using (NpgsqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM \"ExtensionActions\" WHERE guid=:guid";
					cmd.Parameters.Add("guid", guid.ToString());
					PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
				}

				foreach (ExtensionAction action in extensionActions)
				{
					using (NpgsqlCommand cmd = con.CreateCommand())
					{
						cmd.CommandText = "INSERT INTO \"ExtensionActions\" (guid, action, execution) VALUES (:guid, :action, :execution)";
						cmd.Parameters.Add("guid", guid.ToString());
						cmd.Parameters.Add("action", action.Kind.ToString());
						cmd.Parameters.Add("execution", action.Execution.ToString());
						PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
					}
				}

				tran.Commit();
			}
		}

		#endregion
	}
}
