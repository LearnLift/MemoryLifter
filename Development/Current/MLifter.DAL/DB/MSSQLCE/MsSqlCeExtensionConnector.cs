using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces.DB;
using MLifter.Generics;
using System.Data.SqlServerCe;
using System.IO;
using System.Data;

namespace MLifter.DAL.DB.MsSqlCe
{
	/// <summary>
	/// MsSqlCeExtension
	/// </summary>
	/// <remarks>Documented by Dev08, 2009-07-02</remarks>
	public class MsSqlCeExtensionConnector : IDbExtensionConnector
	{
		private static Dictionary<ConnectionStringStruct, MsSqlCeExtensionConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeExtensionConnector>();
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <param name="parentClass">The parent class.</param>
		/// <returns></returns>
		public static MsSqlCeExtensionConnector GetInstance(ParentClass parentClass)
		{
			lock (instances)
			{
				ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

				if (!instances.ContainsKey(connection))
					instances.Add(connection, new MsSqlCeExtensionConnector(parentClass));

				return instances[connection];
			}
		}

		private ParentClass parent;
		private MsSqlCeExtensionConnector(ParentClass parentClass)
		{
			parent = parentClass;
			parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
		}

		private void parent_DictionaryClosed(object sender, EventArgs e)
		{
			IParent parent = sender as IParent;
			instances.Remove(parent.Parent.CurrentUser.ConnectionString);
		}

		/// <summary>
		/// Gets the byte array from stream.
		/// </summary>
		/// <param name="media">The media.</param>
		/// <param name="rpu">The rpu.</param>
		/// <param name="caller">The caller.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-16</remarks>
		private static byte[] GetByteArrayFromStream(System.IO.Stream media, MLifter.DAL.Tools.StatusMessageReportProgress rpu, object caller)
		{
			int buffer_length = 10240;
			byte[] data = new byte[media.Length];
			StatusMessageEventArgs args = new StatusMessageEventArgs(StatusMessageType.CreateMediaProgress, (int)media.Length);

			media.Seek(0, SeekOrigin.Begin);

			int read = 0;
			int pos = 0;
			do
			{
				read = media.Read(data, pos, Math.Min(buffer_length, data.Length - pos));
				pos += read;
				args.Progress = pos;
				if (rpu != null)
					rpu.Invoke(args, caller);
			}
			while (read == buffer_length);
			return data;
		}

		#region IDbExtensionConnector Members

		/// <summary>
		/// Adds the new extension.
		/// </summary>
		/// <returns>The Guid of the new extension</returns>
		public Guid AddNewExtension()
		{
			Guid newGuid = Guid.NewGuid();
			return AddNewExtension(newGuid);
		}

		/// <summary>
		/// Adds the new extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns>The Guid of the new extension</returns>
		public Guid AddNewExtension(Guid guid)
		{
			DeleteExtension(guid);

			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "INSERT INTO \"Extensions\" (guid, name, version, type, data) VALUES (@guid, '', '', @type, 0x0)";
				cmd.Parameters.Add("@guid", guid.ToString());
				cmd.Parameters.Add("@type", ExtensionType.Unknown.ToString());
				MSSQLCEConn.ExecuteNonQuery(cmd);

				return guid;
			}
		}

		/// <summary>
		/// Sets the extension LM.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="lmid">The lmid.</param>
		public void SetExtensionLM(Guid guid, int lmid)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "UPDATE \"Extensions\" SET lm_id=@lmid WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				cmd.Parameters.Add("@lmid", lmid);
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Deletes the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		public void DeleteExtension(Guid guid)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "DELETE FROM \"ExtensionActions\" WHERE guid=@guid; ";
				cmd.CommandText += "DELETE FROM \"Extensions\" WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Gets the name of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public string GetExtensionName(Guid guid)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT name FROM \"Extensions\" WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				object name = MSSQLCEConn.ExecuteScalar(cmd);

				return name as string == null ? string.Empty : name as string;
			}
		}

		/// <summary>
		/// Sets the name of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionName">Name of the extension.</param>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public void SetExtensionName(Guid guid, string extensionName)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "UPDATE \"Extensions\" SET name=@name WHERE guid=@guid;";
				cmd.Parameters.Add("@guid", guid.ToString());
				cmd.Parameters.Add("@name", extensionName);
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Gets the extension version.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public Version GetExtensionVersion(Guid guid)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT version FROM \"Extensions\" WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				try
				{
					return new Version(MSSQLCEConn.ExecuteScalar(cmd).ToString());
				}
				catch (ArgumentException)
				{
					return new Version();
				}
			}
		}

		/// <summary>
		/// Sets the extension version.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="versionName">Name of the version.</param>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public void SetExtensionVersion(Guid guid, Version versionName)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "UPDATE \"Extensions\" SET version=@version WHERE guid=@guid;";
				cmd.Parameters.Add("@guid", guid.ToString());
				cmd.Parameters.Add("@version", versionName.ToString());
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Gets the type of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public ExtensionType GetExtensionType(Guid guid)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT type FROM \"Extensions\" WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				object type = MSSQLCEConn.ExecuteScalar(cmd);

				if (type as string == null)
					return ExtensionType.Unknown;

				return (ExtensionType)Enum.Parse(typeof(ExtensionType), type as string);
			}
		}

		/// <summary>
		/// Sets the type of the extension.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionType">Type of the extension.</param>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public void SetExtensionType(Guid guid, ExtensionType extensionType)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "UPDATE \"Extensions\" SET type=@type WHERE guid=@guid;";
				cmd.Parameters.Add("@guid", guid.ToString());
				cmd.Parameters.Add("@type", extensionType.ToString());
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Gets the extension start file.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public string GetExtensionStartFile(Guid guid)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT startfile FROM \"Extensions\" WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				object startfile = MSSQLCEConn.ExecuteScalar(cmd);

				return startfile as string == null ? string.Empty : startfile as string;
			}
		}

		/// <summary>
		/// Sets the extension start file.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="startFile">The start file.</param>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public void SetExtensionStartFile(Guid guid, string startFile)
		{
			if (startFile == null)
				startFile = string.Empty;

			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "UPDATE \"Extensions\" SET startFile=@startFile WHERE guid=@guid;";
				cmd.Parameters.Add("@guid", guid.ToString());
				cmd.Parameters.Add("@startFile", startFile);
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Gets the extension stream.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public Stream GetExtensionStream(Guid guid)
		{
			MemoryStream stream = null;
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT data FROM \"Extensions\" WHERE guid=@guid;";
				cmd.Parameters.Add("@guid", guid.ToString());
				object obj = MSSQLCEConn.ExecuteScalar(cmd);

				if (obj == null || obj == DBNull.Value)
					return stream;

				stream = new MemoryStream((byte[])obj);
				stream.Seek(0, SeekOrigin.Begin);
			}
			return stream;
		}

		/// <summary>
		/// Sets the extension stream.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionStream">The extension stream.</param>
		/// <remarks>Documented by Dev08, 2009-07-02</remarks>
		public void SetExtensionStream(Guid guid, Stream extensionStream)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "UPDATE \"Extensions\" SET data=@data WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				cmd.Parameters.Add("@data", SqlDbType.Image);
				cmd.Parameters["@data"].Value = GetByteArrayFromStream(extensionStream, null, null);
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Determines whether [is stream available] [the specified GUID].
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns>
		/// 	<c>true</c> if [is stream available] [the specified GUID]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		public bool IsStreamAvailable(Guid guid)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT count(*) FROM Extensions WHERE guid=@guid AND data IS NOT NULL";
				cmd.Parameters.Add("@guid", guid.ToString());

				return MSSQLCEConn.ExecuteScalar<int>(cmd).Value > 0;
			}
		}

		/// <summary>
		/// Gets the extension actions.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-07-03</remarks>
		public IList<ExtensionAction> GetExtensionActions(Guid guid)
		{
			IList<ExtensionAction> extensionActions = new List<ExtensionAction>();
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "SELECT action, execution FROM \"ExtensionActions\" WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());

				SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
				while (reader.Read())
				{
					object action = reader["action"];
					object execution = reader["execution"];

					ExtensionAction extensionAction = new ExtensionAction();
					extensionAction.Kind = (ExtensionActionKind)Enum.Parse(typeof(ExtensionActionKind), action as string);
					extensionAction.Execution = (ExtensionActionExecution)Enum.Parse(typeof(ExtensionActionExecution), execution as string);

					extensionActions.Add(extensionAction);
				}
			}
			return extensionActions;
		}

		/// <summary>
		/// Sets the extension actions.
		/// </summary>
		/// <param name="guid">The GUID.</param>
		/// <param name="extensionActions">The extension actions.</param>
		/// <remarks>Documented by Dev02, 2009-07-03</remarks>
		public void SetExtensionActions(Guid guid, IList<ExtensionAction> extensionActions)
		{
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
			{
				cmd.CommandText = "DELETE FROM \"ExtensionActions\" WHERE guid=@guid";
				cmd.Parameters.Add("@guid", guid.ToString());
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}

			foreach (ExtensionAction action in extensionActions)
			{
				using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
				{
					cmd.CommandText = "INSERT INTO \"ExtensionActions\" (guid, action, execution) VALUES (@guid, @action, @execution)";
					cmd.Parameters.Add("@guid", guid.ToString());
					cmd.Parameters.Add("@action", action.Kind.ToString());
					cmd.Parameters.Add("@execution", action.Execution.ToString());
					MSSQLCEConn.ExecuteNonQuery(cmd);
				}
			}
		}

		#endregion
	}
}
