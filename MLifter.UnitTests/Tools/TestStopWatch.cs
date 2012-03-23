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
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlServerCe;

namespace MLifterTest.Tools
{
	/// <summary>
	/// Offers stopwatch functionality and logs the results to a db.
	/// </summary>
	/// <remarks>Documented by Dev03, 2008-08-01</remarks>
	internal static class TestStopWatch
	{
		private static int TestRunId = -1;
		private static readonly string TestName = "MemoryLifter Unit Tests";
		private static readonly string ConnectionString = "Data Source=|DataDirectory|TestSources.sdf;Persist Security Info=False;";
		private static Dictionary<int, DateTime> starttimes = new Dictionary<int, DateTime>();

		private static SqlCeConnection GetConnection()
		{
			return new SqlCeConnection(ConnectionString);
		}

		/// <summary>
		/// Starts the specified test context.
		/// </summary>
		/// <param name="testContext">The test context.</param>
		/// <remarks>Documented by Dev03, 2008-08-01</remarks>
		internal static void Start(TestContext testContext)
		{
			DateTime starttime = DateTime.Now;
			using (SqlCeConnection connection = GetConnection())
			{
				connection.Open();
				if (TestRunId < 0)
				{
					SqlCeCommand command = new SqlCeCommand("INSERT INTO \"TestRun\" (date, name) VALUES (GETDATE(), @testname);", connection);
					command.Parameters.Add("@testname", SqlDbType.NVarChar, 100);
					command.Parameters["@testname"].Value = TestName;
					command.ExecuteNonQuery();
					command.Dispose();

					command = new SqlCeCommand("SELECT @@IDENTITY;", connection);
					TestRunId = Convert.ToInt32(command.ExecuteScalar());
					command.Dispose();
				}
				if (TestRunId > -1)
				{
					SqlCeCommand command = new SqlCeCommand("INSERT INTO \"TestRunData\" (run_id, cs_id, name, starttime, text) VALUES (@runid, @csid, @testname, @time, NULL)", connection);
					command.Parameters.Add("@runid", SqlDbType.Int, 8);
					command.Parameters.Add("@csid", SqlDbType.Int, 8);
					command.Parameters.Add("@testname", SqlDbType.NVarChar, 100);
					command.Parameters.Add("@time", SqlDbType.DateTime);
					command.Parameters["@runid"].Value = TestRunId;
					command.Parameters["@csid"].Value = (int)testContext.DataRow["ID"];
					command.Parameters["@testname"].Value = testContext.TestName;
					command.Parameters["@time"].Value = starttime;
					command.ExecuteNonQuery();
					command.Dispose();

					command = new SqlCeCommand("SELECT @@IDENTITY;", connection);
					starttimes.Add(Convert.ToInt32(command.ExecuteScalar()), starttime);
					command.Dispose();
				}
			}
		}

		/// <summary>
		/// Stops the specified test context.
		/// </summary>
		/// <param name="testContext">The test context.</param>
		/// <remarks>Documented by Dev03, 2008-08-01</remarks>
		internal static void Stop(TestContext testContext)
		{
			DateTime endtime = DateTime.Now;
			using (SqlCeConnection connection = GetConnection())
			{
				connection.Open();
				if (TestRunId > -1)
				{
					SqlCeCommand command = new SqlCeCommand("SELECT TOP (1) id FROM \"TestRunData\" WHERE run_id = @runid AND cs_id = @csid AND name = @testname", connection);
					command.Parameters.Add("@runid", SqlDbType.Int, 8);
					command.Parameters.Add("@csid", SqlDbType.Int, 8);
					command.Parameters.Add("@testname", SqlDbType.NVarChar, 100);
					command.Parameters["@runid"].Value = TestRunId;
					command.Parameters["@csid"].Value = (int)testContext.DataRow["ID"];
					command.Parameters["@testname"].Value = testContext.TestName;
					int id = Convert.ToInt32(command.ExecuteScalar());
					command.Dispose();

					command = new SqlCeCommand("UPDATE \"TestRunData\" SET stoptime = @time WHERE id = @id", connection);
					command.Parameters.Add("@time", SqlDbType.DateTime);
					command.Parameters.Add("@id", SqlDbType.Int, 8);
					command.Parameters["@time"].Value = endtime;
					command.Parameters["@id"].Value = id;
					command.ExecuteNonQuery();
					command.Dispose();

					DateTime starttime = starttimes[id];
					long ticks = endtime.Ticks - starttime.Ticks;
					command = new SqlCeCommand("UPDATE \"TestRunData\" SET ticks = @ticks WHERE run_id = @runid AND cs_id = @csid AND name = @testname", connection);
					command.Parameters.Add("@ticks", SqlDbType.BigInt);
					command.Parameters.Add("@runid", SqlDbType.Int, 8);
					command.Parameters.Add("@csid", SqlDbType.Int, 8);
					command.Parameters.Add("@testname", SqlDbType.NVarChar, 100);
					command.Parameters["@ticks"].Value = ticks;
					command.Parameters["@runid"].Value = TestRunId;
					command.Parameters["@csid"].Value = (int)testContext.DataRow["ID"];
					command.Parameters["@testname"].Value = testContext.TestName;
					command.ExecuteNonQuery();
					command.Dispose();

					command = new SqlCeCommand("SELECT ticks FROM \"TestLimits\" WHERE test_name = @testname " +
						"AND ((ticks > @ticks AND LOWER(type) = 'lower') OR (ticks < @ticks AND LOWER(type) = 'upper'))", connection);
					command.Parameters.Add("@testname", SqlDbType.NVarChar, 100);
					command.Parameters.Add("@ticks", SqlDbType.BigInt);
					command.Parameters["@testname"].Value = testContext.TestName;
					command.Parameters["@ticks"].Value = ticks;
					object result = command.ExecuteScalar();
					command.Dispose();

					// query only returns a row if upper or lower limit exceed
					if (result != DBNull.Value && result != null)
					{
						long limit = (long)result;
						if (limit > ticks)
							throw new TestTimeExceededException(String.Format("The LOWER test time limit was missed (limit: {0}, actual: {1}).", new TimeSpan(limit), new TimeSpan(ticks)));
						else
							throw new TestTimeExceededException(String.Format("The UPPER test time limit was missed (limit: {0}, actual: {1}).", new TimeSpan(limit), new TimeSpan(ticks)));
					}
				}
			}
		}

		/// <summary>
		/// Cleanup unsuccessfull test log entries from the database (tests which did fail anyway).
		/// </summary>
		/// <param name="testContext">The test context.</param>
		/// <remarks>Documented by Dev03, 2008-08-01</remarks>
		internal static void Cleanup()
		{
			using (SqlCeConnection connection = GetConnection())
			{
				connection.Open();
				SqlCeCommand command = new SqlCeCommand("DELETE FROM \"TestRunData\" WHERE stoptime IS NULL AND run_id = @runid", connection);
				command.Parameters.Add("@runid", SqlDbType.Int, 8);
				command.Parameters["@runid"].Value = TestRunId;
				command.ExecuteNonQuery();
				command.Dispose();
			}
		}
	}
}
