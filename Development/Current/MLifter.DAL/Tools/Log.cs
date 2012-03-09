using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using MLifter.DAL;
using MLifter.DAL.DB;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using MLifter.DAL.XML;
using Npgsql;

namespace MLifter.DAL
{
	/// <summary>
	/// This is the logger for learning modules.
	/// </summary>
	public static class Log
	{
		private static int lastSessionId = -1;
		private static ParentClass globalParent = new ParentClass(new DummyUser(new ConnectionStringStruct()), null);
		private static bool hasParentChanged = true;
		private static IDbSessionConnector GetSessionConnector(ParentClass parent)
		{
			switch (parent.CurrentUser.ConnectionString.Typ)
			{
				case DatabaseType.PostgreSQL:
					return PgSqlSessionConnector.GetInstance(parent);
				case DatabaseType.MsSqlCe:
					return MsSqlCeSessionConnector.GetInstance(parent);
				default:
					throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
			}
		}
		private static IDbLearnLoggingConnector GetLearnLoggingConnector(ParentClass parent)
		{
			switch (parent.CurrentUser.ConnectionString.Typ)
			{
				case DatabaseType.PostgreSQL:
					return PgSqlLearnLoggingConnector.GetInstance(parent);
				case DatabaseType.MsSqlCe:
					return MsSqlCeLearnLoggingConnector.GetInstance(parent);
				default:
					throw new UnsupportedDatabaseTypeException(parent.CurrentUser.ConnectionString.Typ);
			}
		}

		/// <summary>
		/// Gets the last session ID.
		/// </summary>
		/// <value>The last session ID.</value>
		/// <remarks>Documented by Dev08, 2009-01-29</remarks>
		public static int LastSessionID
		{
			get
			{
				return lastSessionId;
			}
		}

		/// <summary>
		/// Opens the user session.
		/// </summary>
		/// <param name="lm_id">The lm_id.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev08, 2008-09-05</remarks>
		public static void OpenUserSession(int lm_id, ParentClass parent)
		{
			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return;
			else
			{
				lock (globalParent)
				{
					globalParent = parent;
					hasParentChanged = true;
				}

				if (!writerThread.IsAlive)
				{
					writerThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
					writerThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
					writerThread.Start();
				}

				try { lastSessionId = GetSessionConnector(parent).OpenUserSession(lm_id); }
				catch (Exception ex)
				{
					Trace.WriteLine("Error starting session: " + ex.Message);
					throw new StartLearningSessionException();
				}
			}
		}

		/// <summary>
		/// Recalculates the box sizes.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-05-06</remarks>
		public static void RecalculateBoxSizes(ParentClass parent)
		{
			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return;
			else
				GetSessionConnector(parent).RecalculateBoxSizes(lastSessionId);
		}

		/// <summary>
		/// Call if a Cards from a box is deleted.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="boxNumber">The box number.</param>
		/// <remarks>
		/// Documented by CFI, 2009-05-06
		/// </remarks>
		public static void CardFromBoxDeleted(ParentClass parent, int boxNumber)
		{
			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return;
			else
				GetSessionConnector(parent).CardFromBoxDeleted(lastSessionId, boxNumber);
		}

		/// <summary>
		/// Cards the added.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev05, 2009-05-25</remarks>
		public static void CardAdded(ParentClass parent)
		{
			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return;
			else
				GetSessionConnector(parent).CardAdded(lastSessionId);
		}

		/// <summary>
		/// Closes the user session.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev08, 2008-09-05</remarks>
		public static void CloseUserSession(ParentClass parent)
		{
			if (lastSessionId < 0) //return if CloseUserSession(...) happens before OpenUserSession(...)
				return;

			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return;
			else
			{
				try
				{
					GetSessionConnector(parent).CloseUserSession(lastSessionId);
					FlushLogQueue(parent);      //force writing all Log entries
				}
				catch (Exception ex) { Trace.WriteLine("Error ending session: " + ex.Message); }
			}

			lastSessionId = -1;
		}

		/// <summary>
		/// Restarts the learning success.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev08, 2008-11-19</remarks>
		public static IDictionary RestartLearningSuccess(ParentClass parent)
		{
			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return parent.GetParentDictionary();

			return GetSessionConnector(parent).RestartLearningSuccess(parent.CurrentUser.ConnectionString.LmId);
		}


		/// <summary>
		/// Creates the learn log entry.
		/// </summary>
		/// <param name="learnLogStruct">The learn log struct.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev08, 2008-09-05</remarks>
		public static void CreateLearnLogEntry(LearnLogStruct learnLogStruct, ParentClass parent)
		{
			if (lastSessionId < 0) //Only write if there is a valid userSession
				return;

			if (parent.CurrentUser.ConnectionString.Typ == DatabaseType.Xml)
				return;
			else
			{
				try
				{
					lock (logQueue)
					{
						logQueue.Enqueue(learnLogStruct);
						Monitor.Pulse(logQueue);
					}
				}
				catch (Exception ex) { Trace.WriteLine("Error writing log: " + ex.Message); }
			}
		}

		private static Thread writerThread = new Thread(new ThreadStart(LearnLogWriter));
		private static Queue<LearnLogStruct> logQueue = new Queue<LearnLogStruct>();
		private static void LearnLogWriter()
		{
			Thread.CurrentThread.IsBackground = true;
			Thread.CurrentThread.Name = "LogWriterThread";

			ParentClass parent = new ParentClass(new DummyUser(new ConnectionStringStruct()), null);

			while (true)
			{
				try
				{
					LearnLogStruct? lls = null;
					lock (logQueue)
					{
						while (logQueue.Count == 0)
							Monitor.Wait(logQueue);

						if (logQueue.Count > 0)
							lls = logQueue.Dequeue();
					}

					if (lls.HasValue)
					{
						if (hasParentChanged)
						{
							lock (globalParent)
							{
								parent = globalParent;
								hasParentChanged = false;
							}
						}

						IDbLearnLoggingConnector learnLogConnector = GetLearnLoggingConnector(parent);
						learnLogConnector.CreateLearnLogEntry(lls.Value);
					}
				}
				catch (Exception ex) { Trace.WriteLine("Error in learnLogWriter: " + ex.Message); }
			}
		}

		private static void FlushLogQueue(ParentClass parent)
		{
			lock (logQueue)
			{
				IDbLearnLoggingConnector con = GetLearnLoggingConnector(parent);
				while (logQueue.Count > 0)
					con.CreateLearnLogEntry(logQueue.Dequeue());
			}
		}
	}
}
