using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLifter.DAL;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using Npgsql;
using System.Drawing;

namespace MLifterTest.DAL
{
    /// <summary>
    /// TestInfrastructure
    /// Have fun and keep on learning ;-)
    /// </summary>
    public static class TestInfrastructure
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private static string tmpRepository = string.Empty;
        private static string MachineName = System.Environment.MachineName.ToLower();
        private static bool DBCreated = false;
        private static bool dbAccessByOtherUser = false;

        //Connection String to accesss the server
        private static readonly string defaultConnectionString = "Server=localhost;Port=5432;Userid=postgres;password=M3m0ryL1ft3r;Protocol=3;SSL=false;Pooling=true;MinPoolSize=1;MaxPoolSize=20;Encoding=UNICODE;Timeout=15;SslMode=Disable;database=postgres";
        //Connection String meant to be extended by the corresponding database name
        private static readonly string connectionStringToGetExtended = "Server=localhost;Port=5432;Userid=postgres;password=M3m0ryL1ft3r;Protocol=3;SSL=false;Pooling=true;MinPoolSize=1;MaxPoolSize=20;Encoding=UNICODE;Timeout=15;SslMode=Disable;database=";

        //Lines for Debug Writeline mark
        public readonly static string beginLine = new string('-', 80);
        public readonly static string endLine = new string('=', 80);

        public static FileCleanupQueue cleanupQueue = new FileCleanupQueue();

        /// <summary>
        /// Gets the loopcount.
        /// </summary>
        /// <value>The loopcount.</value>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static int Loopcount
        {
            get { return 100; }
        }

        /// <summary>
        /// Gets the random.
        /// </summary>
        /// <value>The random.</value>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static Random Random
        {
            get { return random; }
        }

        /// <summary>
        /// Determines whether the specified test context is active.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns>
        /// 	<c>true</c> if the specified test context is active; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static bool IsActive(TestContext testContext)
        {
            return (bool)testContext.DataRow["IsValid"];
        }

        /// <summary>
        /// Connections the type.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns> string describing the connection type</returns>
        /// <remarks>Documented by Dev10, 2008-25-09</remarks>
        public static string ConnectionType(TestContext testContext)
        {
            return (string)testContext.DataRow["Type"];
        }

        /// <summary>
        /// Class clean up.
        /// </summary>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static void MyClassCleanup()
        {
            MLifterTest.Tools.TestStopWatch.Cleanup();
            cleanupQueue.DoCleanup();
        }


        /// <summary>
        /// Gets the connection. Obsolete should be replaced only for compatibility for current Unit tests.
        /// Use GetLMConnection instead, because it express more what it is all about. We are getting LM Connection
        /// not general connections.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-08-01</remarks>
        [Obsolete("Should be replaced only for compatibility for current Unit tests. " +
                  "Use GetLMConnection(LMConnectionParameter) instead, because it express more what it is all about. We are getting LM Connection not general connections.")]
        public static IDictionary GetConnection(TestContext testContext)
        {
            return GetLMConnection(testContext, string.Empty, false);
        }

        /// <summary>
        /// Gets the connection and returns an IDictionary (Learning Module instance)
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>IDictionary</returns>
        /// <remarks>Documented by Dev10, 2008-07-31</remarks>
        public static IDictionary GetLMConnection(TestContext testContext, string repositoryName)
        {
            return GetLMConnection(testContext, repositoryName, false);
        }

        /// <summary>
        /// Gets the connection and returns an IDictionary (Learning Module instance)
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="standAlone">if set to <c>true</c> a stand alone user will be created.</param>
        /// <returns>IDictionary</returns>
        /// <remarks>Documented by Dev10, 2008-07-31</remarks>
        public static IDictionary GetLMConnection(TestContext testContext, string repositoryName, bool standAlone)
        {
            return GetPersistentLMConnection(testContext, repositoryName, -1, standAlone);
        }

        /// <summary>
        /// Gets the connection and returns an IDictionary (Learning Module instance)
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>IDictionary</returns>
        /// <remarks>Documented by Dev10, 2008-07-31</remarks>
        public static IDictionary GetLMConnection(TestContext testContext, GetLoginInformation callback)
        {
            return GetLMConnection(testContext, callback, false);
        }

        /// <summary>
        /// Gets the connection and returns an IDictionary (Learning Module instance)
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="standAlone">if set to <c>true</c> a stand alone user will be created.</param>
        /// <returns>IDictionary</returns>
        /// <remarks>Documented by Dev10, 2008-07-31</remarks>
        public static IDictionary GetLMConnection(TestContext testContext, GetLoginInformation callback, bool standAlone)
        {
            return GetPersistentLMConnection(testContext, String.Empty, -1, callback, standAlone);
        }

        /// <summary>
        /// Gets the persistent LM connection.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="LMId">The LM id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-02</remarks>
        public static IDictionary GetPersistentLMConnection(TestContext testContext, string repositoryName, int LMId)
        {
            return GetPersistentLMConnection(testContext, repositoryName, LMId, false);
        }

        /// <summary>
        /// Gets the persistent LM connection.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="LMId">The LM id.</param>
        /// <param name="standAlone">if set to <c>true</c> a stand alone user will be created.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-09-02</remarks>
        public static IDictionary GetPersistentLMConnection(TestContext testContext, string repositoryName, int LMId, bool standAlone)
        {
            return GetPersistentLMConnection(testContext, repositoryName, LMId, null, standAlone);
        }

        /// <summary>
        /// Gets the persistent LM connection.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="LMId">The LM id.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-25-09</remarks>
        public static IDictionary GetPersistentLMConnection(TestContext testContext, string repositoryName, int LMId, GetLoginInformation callback)
        {
            return GetPersistentLMConnection(testContext, repositoryName, LMId, callback, false);
        }

        /// <summary>
        /// Gets the persistent LM connection.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="LMId">The LM id.</param>
        /// <param name="callback">The callback.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-25-09</remarks>
        public static IDictionary GetPersistentLMConnection(TestContext testContext, string repositoryName, int LMId, GetLoginInformation callback, bool standAlone)
        {
            return GetPersistentLMConnection(testContext, repositoryName, LMId, callback, string.Empty, standAlone, string.Empty, false);
        }

        /// <summary>
        /// Gets the LM connection.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-02-16</remarks>
        public static IDictionary GetLMConnection(LMConnectionParameter parameter)
        {
            return GetPersistentLMConnection(parameter.TestContext, parameter.RepositoryName, parameter.LearningModuleId, parameter.Callback, parameter.ConnectionType,
                parameter.standAlone, parameter.Password, parameter.IsProtected);
        }

        /// <summary>
        /// Gets the persistent LM connection.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="repositoryName">Name of the repository (Hostname of the Server).</param>
        /// <param name="LMId">The LM id.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <param name="standAlone">if set to <c>true</c> a stand alone user will be created.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2008-08-01</remarks>
        public static IDictionary GetPersistentLMConnection(TestContext testContext, string repositoryName, int LMId, GetLoginInformation callback, string connectionType, bool standAlone, string password, bool isProtected)
        {
            string connectionString = string.Empty;
            ConnectionStringStruct ConnectionString;

            string type = connectionType == "" ? (string)testContext.DataRow["Type"] : connectionType;
            bool IsValid = (bool)testContext.DataRow["IsValid"];


            switch (type.ToLower())
            {
                case "file":
                    #region ODX Tests
                    if (repositoryName == string.Empty)         //Not persistent
                    {
                        repositoryName = MachineName;
                        ConnectionString = new ConnectionStringStruct(DatabaseType.Xml, TestDic, false);
                    }
                    else            //Persistent (During Unit Test)
                    {
                        string persistentPath;
                        persistentPath = Path.GetTempPath() + repositoryName + Helper.OdxExtension;
                        cleanupQueue.Enqueue(persistentPath);
                        ConnectionString = new ConnectionStringStruct(DatabaseType.Xml, persistentPath, false);
                    }
                    #endregion
                    break;
                case "pgsql":
                    #region PgSQL Tests
                    repositoryName = MachineName.ToLower();
                    if (testContext.DataRow["ConnectionString"] != System.DBNull.Value)
                    {
                        connectionString = (string)testContext.DataRow["ConnectionString"];
                        connectionString = connectionString.Replace("DYNAMIC", repositoryName);

                        //Only create DataBase if it was not yet created, so only for the first time, one database for all unit tests in this run
                        if (!DBCreated)
                        {
                            if (dbAccessByOtherUser)
                                throw new Exception("DB is being accessed by other users");
                            else
                            {
                                try
                                {
                                    DropDB(repositoryName);
                                }
                                catch (NpgsqlException exp)
                                {
                                    if (exp.Code == "55006") //Pgsql error code for: Database is accessed by other users
                                    {
                                        dbAccessByOtherUser = true;
                                        throw new Exception("DB is being accessed by other users");
                                    }
                                }
                                CreateDB(repositoryName, testContext, isProtected);
                                DBCreated = true;
                                tmpRepository = repositoryName;
                            }
                        }
                    }
                    else
                        throw new Exception("PGSQL connection string could not be read from unit test database.");

                    if (LMId < 0)       //Persistent (During Test Run)
                        LMId = AddNewLM(repositoryName, isProtected);

                    ConnectionString = new ConnectionStringStruct(DatabaseType.PostgreSQL, connectionString, LMId);

                    #endregion
                    break;
                case "sqlce":
                    #region SqlCe Tests

                    if (repositoryName == string.Empty)     //Not persistent
                    {
                        repositoryName = MachineName;
                        ConnectionString = new ConnectionStringStruct(DatabaseType.MsSqlCe, TestDicSqlCE, false);
                    }
                    else   //Persistent (During Unit Test)
                    {
                        string persistentPath;
                        persistentPath = Path.GetTempPath() + repositoryName + Helper.EmbeddedDbExtension;
                        cleanupQueue.Enqueue(persistentPath);
                        ConnectionString = new ConnectionStringStruct(DatabaseType.MsSqlCe, persistentPath, false);
                    }

                    if (password != string.Empty)
                    {
                        ConnectionString.ProtectedLm = true;
                        ConnectionString.Password = password;
                    }

                    if (LMId < 0)
                    {
                        string sqlCeConnString = GetFullSqlCeConnectionString(ConnectionString);
                        using (SqlCeEngine clientEngine = new SqlCeEngine(sqlCeConnString))
                        {
                            clientEngine.CreateDatabase();
                            clientEngine.Dispose();
                        }
                        using (SqlCeConnection con = new SqlCeConnection(sqlCeConnString))
                        {
                            con.Open();
                            SqlCeTransaction transaction = con.BeginTransaction();
                            string tmp = Helper.GetMsSqlCeScript();
                            string[] msSqlScriptArray = Helper.GetMsSqlCeScript().Split(';');   //Split the whole DB-Script into single commands (SQL-CE can not execute multiple queries)

                            foreach (string sqlCommand in msSqlScriptArray)
                            {
                                if (sqlCommand.TrimStart(' ', '\r', '\n').StartsWith("--") || sqlCommand.TrimStart(' ', '\r', '\n').Length < 5)
                                    continue;
                                using (SqlCeCommand cmd = con.CreateCommand())
                                {
                                    cmd.CommandText = sqlCommand;
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            int cat_id;
                            using (SqlCeCommand cmd = con.CreateCommand())
                            {
                                cmd.CommandText = "SELECT id FROM Categories WHERE global_id=@cat_id;";
                                cmd.Parameters.Add("@cat_id", 1);
                                cat_id = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            using (SqlCeCommand cmd = con.CreateCommand())
                            {
                                cmd.CommandText = "INSERT INTO LearningModules (guid, title, categories_id, default_settings_id, allowed_settings_id, licence_key, content_protected, cal_count) " +
                                "VALUES (@guid, @title, @cat_id, @dset, @aset, @lk, @cp, @cals);";
                                cmd.Parameters.Add("@guid", new Guid().ToString());
                                cmd.Parameters.Add("@title", "eDB test title");
                                cmd.Parameters.Add("@cat_id", cat_id);
                                cmd.Parameters.Add("@lk", "ACDED-LicenseKey-DEDAF");
                                cmd.Parameters.Add("@cp", password == string.Empty ? 0 : 1);
                                cmd.Parameters.Add("@cals", 1);
                                cmd.Parameters.Add("@dset", MsSqlCeCreateNewSettings(sqlCeConnString));
                                cmd.Parameters.Add("@aset", MsSqlCeCreateNewAllowedSettings(sqlCeConnString));
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCeCommand cmd = con.CreateCommand())
                            {
                                cmd.CommandText = "SELECT @@IDENTITY;";
                                LMId = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            transaction.Commit();
                        }
                    }
                    ConnectionString.LmId = LMId;
                    #endregion
                    break;
                default:
                    throw new Exception("TestInfrastructure Class: conditions where set which are not applicable to the current db connection infrastructure");
            }

            IUser user;
            if (callback == null)
                user = UserFactory.Create((GetLoginInformation)GetTestUser, ConnectionString, (DataAccessErrorDelegate)delegate { return; }, standAlone);
            else
                user = UserFactory.Create(callback, ConnectionString, (DataAccessErrorDelegate)delegate { return; }, standAlone);
            return user.Open();
        }

        /// <summary>
        /// Gets the testuser.
        /// </summary>
        /// <param name="userStr">The user struct.</param>
        /// <param name="con">The connectionStringStruct.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08</remarks>
        public static UserStruct? GetTestUser(UserStruct userStr, ConnectionStringStruct con)
        {
            UserStruct userStruct = new UserStruct("testuser", UserAuthenticationTyp.ListAuthentication);
            userStruct.CloseOpenSessions = true;
            return userStruct;
        }

        /// <summary>
        /// Gets the admin user.
        /// </summary>
        /// <param name="userStr">The user struct.</param>
        /// <param name="con">The connectionStringStruct.</param>
        /// <returns>The user struct.</returns>
        /// <remarks>Documented by Dev08</remarks>
        public static UserStruct? GetAdminUser(UserStruct userStr, ConnectionStringStruct con)
        {
            UserStruct userStruct = new UserStruct("admin", "admin", UserAuthenticationTyp.FormsAuthentication, false);
            userStruct.CloseOpenSessions = true;
            return userStruct;
        }

        /// <summary>
        /// Checks whether the current data layer supports nullable values.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-20</remarks>
        public static bool SupportsNullableValues(TestContext testContext)
        {
            string type = (string)testContext.DataRow["Type"];
            if (type.ToLower() == "file")
                return false;
            else if (type.ToLower() == "pgsql")
                return true;
            else if (type.ToLower() == "sqlce")
                return true;
            else
                throw new Exception("TestContext: DataType not supported.");
        }

        /// <summary>
        /// Gets the test audio.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetTestAudio()
        {
            string testAudio = cleanupQueue.GetTempFilePath(".wav");
            byte[] buffer = new byte[Properties.Resources.homer_wav.Length];
            Properties.Resources.homer_wav.Read(buffer, 0, (int)Properties.Resources.homer_wav.Length);
            File.WriteAllBytes(testAudio, buffer);
            return testAudio;
        }

        /// <summary>
        /// Gets the test image.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetTestImage()
        {
            string testImage = cleanupQueue.GetTempFilePath(".jpg");
            Properties.Resources.homer_jpg.Save(testImage);
            return testImage;
        }

        /// <summary>
        /// Gets the test video.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetTestVideo()
        {
            string testVideo = cleanupQueue.GetTempFilePath(".avi");
            byte[] buffer = new byte[Properties.Resources.homer_avi.Length];
            Properties.Resources.homer_wav.Read(buffer, 0, (int)Properties.Resources.homer_avi.Length);
            File.WriteAllBytes(testVideo, buffer);
            return testVideo;
        }

        /// <summary>
        /// Gets the large test video.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        /// <remarks>Documented by Dev08</remarks>
        public static string GetLargeTestImage()
        {
            string testImagePath = cleanupQueue.GetTempFilePath(".jpg");
            Bitmap testBitmap = new Bitmap(TestInfrastructure.random.Next(1000, 2000), TestInfrastructure.random.Next(1000, 2000));
            testBitmap.Save(testImagePath);

            return testImagePath;
        }

        /// <summary>
        /// Gets the large test video.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetLargeTestVideo()
        {
            string testVideo = cleanupQueue.GetTempFilePath(".avi");
            byte[] buffer = new byte[TestInfrastructure.Random.Next(20000000, 50000000)];
            TestInfrastructure.Random.NextBytes(buffer);
            File.WriteAllBytes(testVideo, buffer);
            return testVideo;
        }

        /// <summary>
        /// Gets the large test video.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-20</remarks>
        public static string GetLargeTestAudio()
        {
            string testAudio = cleanupQueue.GetTempFilePath(".wav");
            byte[] buffer = new byte[TestInfrastructure.Random.Next(5000000, 20000000)];
            TestInfrastructure.Random.NextBytes(buffer);
            File.WriteAllBytes(testAudio, buffer);
            return testAudio;
        }

        /// <summary>
        /// To be used within unit Tests to mark the Test Start in Debug View
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static void DebugLineStart(TestContext testContext)
        {
            Debug.WriteLine(beginLine + " Now starting test '" + testContext.TestName + "'" + beginLine);
        }

        /// <summary>
        /// To be used within unit Tests to mark the Test End in Debug View
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <remarks>Documented by Dev10, 2008-07-30</remarks>
        public static void DebugLineEnd(TestContext testContext)
        {
            Debug.WriteLine(endLine + " '" + testContext.TestName + "' ended " + endLine);
        }

        /// <summary>
        /// Compares two DateTimes with a resolution of seconds
        /// </summary>
        /// <param name="dateTime1">The date time1.</param>
        /// <param name="dateTime2">The date time2.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-19</remarks>
        public static bool CompareDateTimes(DateTime dateTime1, DateTime dateTime2)
        {
            if (dateTime1.Year == dateTime2.Year &&
                dateTime1.Month == dateTime2.Month &&
                dateTime1.Day == dateTime2.Day &&
                dateTime1.Hour == dateTime2.Hour &&
                dateTime1.Minute == dateTime2.Minute &&
                dateTime1.Second == dateTime2.Second)
                return true;

            return false;
        }

        #region Private Methods

        #region Private odx Methods
        private static string TestDic
        {
            //Gets the test dic, created a random file path for a odx.
            get
            {
                return cleanupQueue.GetTempFilePath(Helper.OdxExtension);
            }
        }
        #endregion

        #region Private PgSql Methods

        private static int AddNewLM(string repositoryName, bool contentProtected)
        {
            return AddNewLM(Guid.NewGuid().ToString(), 1, "SlipStreamResearch", repositoryName, contentProtected);
        }

        private static int AddNewLM(string guid, int categoryId, string title, string repositoryName, bool contentProtected)
        {
            string ConnectionString = connectionStringToGetExtended + repositoryName.ToLower();
            int newLmId;

            using (NpgsqlConnection con = CreateConnectionForLMCreation(ConnectionString))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    //if no specific id is provided LM will be created with auto id from DB
                    cmd.CommandText = "SELECT \"CreateNewLearningModule\"(:guid, :categoryid, :title)";

                    cmd.Parameters.Add("guid", guid);
                    cmd.Parameters.Add("categoryid", categoryId);
                    cmd.Parameters.Add("title", title);

                    //return newly created id if data set has been newly created
                    newLmId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (contentProtected)
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE \"LearningModules\" SET content_protected = true";
                        cmd.ExecuteNonQuery();
                    }
                }

                return newLmId;
            }
        }

        private static void CreateDB(string repositoryName, TestContext testContext, bool isProtected)
        {
            using (NpgsqlConnection con = CreateConnectionForDBCreation())
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    //we always have low char strings
                    repositoryName = repositoryName.ToLower();

                    //get the sql script
                    string TablesCmdString = File.ReadAllText(Path.Combine(testContext.TestDeploymentDir, Properties.Resources.MLIFTERDBSQL));
                    string DBCreateCmdString;

                    //create the db
                    DBCreateCmdString = "CREATE DATABASE " + repositoryName + " ENCODING 'UTF8';";
                    cmd.CommandText = DBCreateCmdString;
                    cmd.ExecuteNonQuery();

                    //Why?? Fabthe
                    //fill it with data from sql script above
                    con.ChangeDatabase(repositoryName);
                    cmd.CommandText = TablesCmdString;
                    cmd.ExecuteNonQuery();

                    //create test user
                    string DBCreateTestuserCmdString = "SELECT \"InsertUserProfile\"('testuser', '', '', 'ListAuthentication'); SELECT \"AddGroupToUserByName\"('testuser', 'Student');";
                    cmd.CommandText = DBCreateTestuserCmdString;
                    cmd.ExecuteNonQuery();


                    if (isProtected)
                    {
                        using (NpgsqlCommand cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = "UPDATE \"LearningModules\" SET content_protected = true";
                            cmd2.ExecuteNonQuery();
                        }
                    }

                }
            }

        }

        private static void DropDB(string repositoryName)
        {
            using (NpgsqlConnection con = CreateConnectionForDBCreation())
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    //we always have low char strings
                    repositoryName = repositoryName.ToLower();

                    //delete already existent table
                    string DBCreateCmdString;
                    DBCreateCmdString = "DROP DATABASE IF EXISTS " + repositoryName + ";";
                    cmd.CommandText = DBCreateCmdString;
                    cmd.ExecuteNonQuery();

                    con.Close();

                }
            }

        }

        private static NpgsqlConnection CreateConnectionForDBCreation()
        {
            return CreateConnectionForLMCreation(defaultConnectionString);
        }

        private static NpgsqlConnection CreatetDBConnectionBasedOnCurrentDBSession()
        {
            return CreateConnectionForLMCreation(connectionStringToGetExtended + tmpRepository);
        }

        private static NpgsqlConnection CreateConnectionForLMCreation(string ConnectionString)
        {
            NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        #endregion

        #region Private SqlCe Methods
        private static int MsSqlCeCreateNewSettings(string connectionString)
        {
            using (SqlCeConnection con = new SqlCeConnection(connectionString))
            {
                con.Open();

                //1. SnoozeOptions Table
                int snoozeOptionsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"SnoozeOptions\"(cards_enabled,rights_enabled,time_enabled)	VALUES(0,0,0);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        snoozeOptionsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //2. MultipleChoiceOptions Table
                int multipleChoiceOptionsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO\"MultipleChoiceOptions\" (allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices)" +
                                      "VALUES(0, 1, 1, 3);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        multipleChoiceOptionsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //3. QueryTypes Table
                int queryTypesId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"QueryTypes\"(image_recognition, listening_comprehension, multiple_choice, sentence, word)" +
                                      "VALUES(0, 0, 1, 0, 1);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        queryTypesId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //4. TypeGradings Table
                int typeGradingsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"TypeGradings\"(all_correct, half_correct, none_correct, prompt) VALUES(0, 1, 0, 0);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        typeGradingsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //5. SynonymGradings Table
                int synonymGradingsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"SynonymGradings\"(all_known, half_known, one_known, first_known, prompt) VALUES(0, 0, 1, 0, 0);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        synonymGradingsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //6. QueryDirections Table
                int queryDirectionsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"QueryDirections\"(question2answer, answer2question, mixed)	VALUES(1, 0, 0);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        queryDirectionsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //7. CardStyles Table
                int cardStylesId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"CardStyles\"(value) VALUES(NULL);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        cardStylesId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //8. Boxes Table
                int boxesId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"Boxes\"(box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size) " +
                                      "VALUES(10, 20, 50, 100, 250, 500, 1000, 2000, 4000);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        boxesId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //9. Settings Table --> insert all foreign keys into the settings table:
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"Settings\"" +
                                      "(snooze_options, multiple_choice_options, query_types, type_gradings, synonym_gradings, query_directions, cardstyle, boxes, " +
                                      "autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, random_pool, self_assessment, " +
                                      "show_images, stripchars, auto_boxsize, pool_empty_message_shown, show_statistics, skip_correct_answers, use_lm_stylesheets)" +
                                      "VALUES(@snooze_options_id, @multiple_choice_options_id, @query_types_id, @type_gradings_id, @synonym_gradings_id, @query_directions_id, " +
                                        "@card_styles_id, @boxes_id, 1, 0, 0, 0, 0, 0, 1, 0, 1, @stripchars, 0, 0, 1, 0, 0);";
                    cmd.Parameters.Add("@snooze_options_id", snoozeOptionsId);
                    cmd.Parameters.Add("@multiple_choice_options_id", multipleChoiceOptionsId);
                    cmd.Parameters.Add("@query_types_id", queryTypesId);
                    cmd.Parameters.Add("@type_gradings_id", typeGradingsId);
                    cmd.Parameters.Add("@synonym_gradings_id", synonymGradingsId);
                    cmd.Parameters.Add("@query_directions_id", queryDirectionsId);
                    cmd.Parameters.Add("@card_styles_id", cardStylesId);
                    cmd.Parameters.Add("@boxes_id", boxesId);
                    cmd.Parameters.Add("@stripchars", "!,.?;");
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        return Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }
            }
        }

        private static int MsSqlCeCreateNewAllowedSettings(string connectionString)
        {
            using (SqlCeConnection con = new SqlCeConnection(connectionString))
            {
                con.Open();

                //1. SnoozeOptions Table
                int snoozeOptionsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"SnoozeOptions\"(cards_enabled,rights_enabled,time_enabled)	VALUES(1,1,1);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        snoozeOptionsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //2. MultipleChoiceOptions Table
                int multipleChoiceOptionsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO\"MultipleChoiceOptions\" (allow_multiple_correct_answers, allow_random_distractors)" +
                                      "VALUES(1, 1);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        multipleChoiceOptionsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //3. QueryTypes Table
                int queryTypesId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"QueryTypes\"(image_recognition, listening_comprehension, multiple_choice, sentence, word)" +
                                      "VALUES(1, 1, 1, 1, 1);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        queryTypesId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //4. TypeGradings Table
                int typeGradingsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"TypeGradings\"(all_correct, half_correct, none_correct, prompt) VALUES(1, 1, 1, 1);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        typeGradingsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //5. SynonymGradings Table
                int synonymGradingsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"SynonymGradings\"(all_known, half_known, one_known, first_known, prompt) VALUES(1, 1, 1, 1, 1);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        synonymGradingsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //6. QueryDirections Table
                int queryDirectionsId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"QueryDirections\"(question2answer, answer2question, mixed)	VALUES(1, 1, 1);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        queryDirectionsId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //7. CardStyles Table
                int cardStylesId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"CardStyles\"(value) VALUES(NULL);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        cardStylesId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //8. Boxes Table
                int boxesId;
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"Boxes\"(box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size) " +
                                      "VALUES(null, null, null, null, null, null, null, null, null);";
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        boxesId = Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }

                //9. Settings Table --> insert all foreign keys into the settings table:
                using (SqlCeCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"Settings\"" +
                                      "(snooze_options, multiple_choice_options, query_types, type_gradings, synonym_gradings, query_directions, cardstyle, boxes, " +
                                      "autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, random_pool, self_assessment, " +
                                      "show_images, stripchars, auto_boxsize, pool_empty_message_shown, show_statistics, skip_correct_answers, use_lm_stylesheets)" +
                                      "VALUES(@snooze_options_id, @multiple_choice_options_id, @query_types_id, @type_gradings_id, @synonym_gradings_id, @query_directions_id, " +
                                      "@card_styles_id, @boxes_id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);";
                    cmd.Parameters.Add("@snooze_options_id", snoozeOptionsId);
                    cmd.Parameters.Add("@multiple_choice_options_id", multipleChoiceOptionsId);
                    cmd.Parameters.Add("@query_types_id", queryTypesId);
                    cmd.Parameters.Add("@type_gradings_id", typeGradingsId);
                    cmd.Parameters.Add("@synonym_gradings_id", synonymGradingsId);
                    cmd.Parameters.Add("@query_directions_id", queryDirectionsId);
                    cmd.Parameters.Add("@card_styles_id", cardStylesId);
                    cmd.Parameters.Add("@boxes_id", boxesId);
                    cmd.ExecuteNonQuery();
                    using (SqlCeCommand cmd2 = con.CreateCommand())
                    {
                        cmd2.CommandText = "SELECT @@IDENTITY;";
                        return Convert.ToInt32(cmd2.ExecuteScalar());
                    }
                }
            }
        }

        private static string GetFullSqlCeConnectionString(string filePath)
        {
            return "Data Source=" + filePath + ";Max Database Size=4091;Persist Security Info=False;";
        }

        private static string GetFullSqlCeConnectionString(ConnectionStringStruct connectionString)
        {
            string output = GetFullSqlCeConnectionString(connectionString.ConnectionString);
            if (connectionString.Password != string.Empty)
                output += "Encrypt Database=True;Password=" + connectionString.Password + ";";

            return output;
        }

        private static string TestDicSqlCE
        {
            get
            {
                //Gets the test dic SQL CE.
                return cleanupQueue.GetTempFilePath(Helper.EmbeddedDbExtension);
            }
        }
        #endregion

        #endregion
    }

    public struct LMConnectionParameter
    {
        public TestContext TestContext;
        public string RepositoryName;
        public int LearningModuleId;
        public GetLoginInformation Callback;
        public string ConnectionType;
        public bool standAlone;
        public string Password;
        public bool IsProtected;

        public LMConnectionParameter(TestContext testContext)
        {
            this.TestContext = testContext;
            RepositoryName = string.Empty;
            LearningModuleId = -1;
            Callback = null;
            ConnectionType = string.Empty;
            standAlone = false;
            Password = string.Empty;
            IsProtected = false;
        }
    }
}
