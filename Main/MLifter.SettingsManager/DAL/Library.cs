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
using System.Linq;
using System.Text;
using MLifter.DAL.DB.MsSqlCe;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using MLifter.BusinessLayer;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using MLifter.Generics;
using MLifter.DAL.Interfaces;
using MLifter.DAL;
using System.ComponentModel;
using MLifter.Controls;
using System.Windows.Forms;

namespace MLifterSettingsManager.DAL
{
    public delegate void SetStatusMessageDelegate(string message, bool update);

    public class Publisher
    {
        public static bool Publish(string source, string destination)
        {
            if (!Publisher.CopyLM(source, destination))
                return false;
            if (!Publisher.DeleteUserProfiles(destination))
                return false;

            return true;
        }

        /// <summary>
        /// Deletes the user profiles.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev07, 2009-05-20</remarks>
        public static bool DeleteUserProfiles(string destination)
        {
            try
            {
                SqlCeConnection con = GetConnection(destination);
                con.Open();
                SqlCeCommand cmd = con.CreateCommand();

                cmd.CommandText = "DELETE FROM UserProfiles_UserGroups";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM UserProfiles_AccessControlList";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM LearnLog";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM LearningSessions";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM UserCardState";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM UserProfilesLearningModulesSettings";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM UserProfiles";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "UPDATE Settings SET use_lm_stylesheets = 1 WHERE use_lm_stylesheets = 0";
                cmd.ExecuteNonQuery();

                con.Close();
                con.Dispose();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        private static BinaryFormatter serializer = new BinaryFormatter();

        /// <summary>
        /// Copies the LM.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev07, 2009-05-20</remarks>
        public static bool CopyLM(string source, string destination)
        {
            try
            {
                string filename = destination;
                if (File.Exists(filename))
                {
                    int i = 0;
                    while (File.Exists(filename.Replace(".mlm", "_" + i + ".mlm"))) i++;
                    filename = filename.Replace(".mlm", "_" + i + ".mlm");
                }

                string sourceConString = MSSQLCEConn.GetFullConnectionString(source);
                string desConString = MSSQLCEConn.GetFullConnectionString(filename);
                SqlCeEngine engine = new SqlCeEngine(sourceConString);
                engine.Compact(desConString);
                engine.Dispose();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        private static SqlCeConnection GetConnection(string path)
        {
            return new SqlCeConnection(MSSQLCEConn.GetFullConnectionString(path));
        }
    }

    internal class ConverterFromOdx
    {
        /// <summary>
        /// Eventargs which delivers the filename of a converted file.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        internal class ConvertingEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the converted file.
            /// </summary>
            /// <value>The converted file.</value>
            /// <remarks>Documented by Dev05, 2009-03-03</remarks>
            public string ConvertedFile { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConvertingEventArgs"/> class.
            /// </summary>
            /// <param name="convertedFile">The converted file.</param>
            /// <remarks>Documented by Dev05, 2009-03-03</remarks>
            public ConvertingEventArgs(string convertedFile)
            {
                ConvertedFile = convertedFile;
            }
        }

        private LoadStatusMessage m_LoadMsg = new LoadStatusMessage(String.Empty, 100, true);
        private static bool cleanUpOdx = true;
        private static string odxFile = string.Empty;
        private static string edbFile = string.Empty;

        public string Dictionary { get; set; }
        public SetStatusMessageDelegate SetStatusDelegate { get; set; }

        private static void DataAccessError(object sender, Exception exp)
        {
            throw exp;
        }
        public ConverterFromOdx()
        {

        }
        public void Start(string filename, bool cleanUp)
        {
            System.Windows.Forms.Application.DoEvents();
            cleanUpOdx = cleanUp;
            if (filename == (string)null) throw new ArgumentNullException("filename");
            if (filename.Length == 0) throw new ArgumentException("filename");
            try
            {
                ShowStatusMessage("Converting");
                odxFile = filename;
                IUser xmlUser = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                    new ConnectionStringStruct(DatabaseType.Xml, Path.GetDirectoryName(filename), filename, true), DataAccessError, this);

                IDictionary oldDic = xmlUser.Open();

                IUser dbUser = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                    new ConnectionStringStruct(DatabaseType.Unc, Path.GetDirectoryName(filename), Path.ChangeExtension(filename, Helper.EmbeddedDbExtension), true), DataAccessError, this);

                IDictionary newDic = dbUser.List().AddNew(oldDic.Category.Id > 5 ? 3 : oldDic.Category.Id, oldDic.Title);

                ConnectionStringStruct oldCon = new ConnectionStringStruct(DatabaseType.Xml, filename);
                ConnectionStringStruct newCon = new ConnectionStringStruct(DatabaseType.MsSqlCe, newDic.Connection, newDic.Id);
                edbFile = newCon.ConnectionString;

                oldDic.Dispose();
                newDic.Dispose();

                LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
                LearnLogic.CopyLearningModule(oldCon, newCon, (GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; }, UpdateStatusMessage, DataAccessError, null);

                Dictionary = newCon.ConnectionString;
            }
            catch { HideStatusMessage(); throw; }
        }
        public event EventHandler<ConvertingEventArgs> ConvertingFinished;
        protected virtual void OnConvertedFinish(ConvertingEventArgs e)
        {
            if (ConvertingFinished != null)
                ConvertingFinished(this, e);
        }
        private void LearnLogic_CopyToFinished(object sender, EventArgs e)
        {
            LearnLogic.CopyToFinished -= new EventHandler(LearnLogic_CopyToFinished);
            HideStatusMessage();

            if (cleanUpOdx)
            {
                ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.Xml, Path.GetDirectoryName(odxFile), odxFile, true);
                IUser xmlUser = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                      css, DataAccessError, null);
                xmlUser.List().Delete(css);
            }
            else
            {
                FileInfo fi = new FileInfo(odxFile);
                if (File.Exists(odxFile + ".bak"))
                {
                    try
                    {
                        File.Delete(odxFile + ".bak");
                    }
                    catch { }
                }
                try
                {
                    fi.MoveTo(odxFile + ".bak");
                }
                catch { }
            }
            OnConvertedFinish(new ConvertingEventArgs(edbFile));
        }

        private void ShowStatusMessage(string msg)
        {
            if (SetStatusDelegate != null)
            {
                SetStatusDelegate.Invoke(msg, false);
                return;
            }

            if (m_LoadMsg.InvokeRequired)
                m_LoadMsg.Invoke((MethodInvoker)delegate
                {
                    m_LoadMsg.InfoMessage = msg;
                    m_LoadMsg.SetProgress(0);
                    Cursor.Current = Cursors.WaitCursor;
                    m_LoadMsg.Show();
                });

        }
        private void HideStatusMessage()
        {
            if (SetStatusDelegate != null)
                return;

            if (m_LoadMsg.InvokeRequired)
                m_LoadMsg.Invoke((MethodInvoker)delegate
                {
                    m_LoadMsg.Hide();
                });
        }
        private void UpdateStatusMessage(string statusMessage, double currentPercentage)
        {
            if (SetStatusDelegate != null)
            {
                SetStatusDelegate.Invoke(statusMessage, true);
                return;
            }

            if (m_LoadMsg.InvokeRequired)
                m_LoadMsg.Invoke((MethodInvoker)delegate
                {
                    m_LoadMsg.InfoMessage = statusMessage;
                    m_LoadMsg.SetProgress(System.Convert.ToInt32(currentPercentage));
                    System.Windows.Forms.Application.DoEvents();
                });
            else
            {
                m_LoadMsg.InfoMessage = statusMessage;
                m_LoadMsg.SetProgress(System.Convert.ToInt32(currentPercentage));
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }

    public class UnPacker
    {
        public event EventHandler UnpackingFinished;
        protected virtual void OnUnpackingFinished(EventArgs e)
        {
            if (UnpackingFinished != null)
                UnpackingFinished(this, e);
        }

        private LoadStatusMessage m_LoadMsg = new LoadStatusMessage(String.Empty, 100, true);

        public string Dictionary { get; private set; }
        public SetStatusMessageDelegate SetStatusDelegate { get; set; }

        public UnPacker()
        {
        }
        public void Start(string source, string destination)
        {
            try
            {
                //unpacking
                Dictionary = UnpackDic(source);
                OnUnpackingFinished(EventArgs.Empty);
            }
            catch (Exception e)
            {
                throw new Exception("Error Unpacking: " + e.Message);
            }
        }
        private string GenerateDictionaryPath(string filename)
        {
            string dictionaryPath = Path.Combine(Path.GetDirectoryName(filename), "unzipped");
            dictionaryPath = Path.Combine(dictionaryPath, Path.GetFileNameWithoutExtension(filename));
            return dictionaryPath;
        }
        private string UnpackDic(string filename)
        {
            bool ignoreInvalidHeader = false;
            string dictionaryPath = String.Empty;
            MLifter.DAL.Interfaces.IDictionary dictionary = null;
            string fileNameWithoutExtension = String.Empty;

            if (!MLifter.DAL.Pack.Packer.IsValidArchive(filename))
            {
                throw new Exception("Invalid Archiv");
            }

            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            dictionaryPath = GenerateDictionaryPath(filename);

            try
            {
                if (!Directory.Exists(dictionaryPath))
                {
                    Directory.CreateDirectory(dictionaryPath);
                }
            }
            catch
            {
                throw new Exception("Cannot Create Dictionary Folder for Unpacking");
            }

            bool pathExists = false;
            do
            {
                if (File.Exists(Path.Combine(dictionaryPath, fileNameWithoutExtension + MLifter.DAL.Helper.OdxExtension)))
                {
                    pathExists = true;
                }
                else if (Directory.GetFiles(dictionaryPath, "*" + MLifter.DAL.Helper.OdxExtension).Length > 0)
                {
                    pathExists = true;
                }
                else
                {
                    pathExists = false;
                }
            }
            while (pathExists);

            ShowStatusMessage("Unpacking");


            try
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(Unzip_ProgressChanged);
                MLifter.DAL.Pack.Packer packer = new MLifter.DAL.Pack.Packer(backgroundWorker);
                packer.TemporaryFolder = dictionaryPath;
                packer.LoginCallback = (MLifter.DAL.GetLoginInformation)LoginForm.OpenLoginForm;
                dictionary = packer.Unpack(filename, ignoreInvalidHeader);
                // dictionaryPath = Path.Combine(dictionaryPath, fileNameWithoutExtension + DAL.DictionaryFactory.OdxExtension);
                dictionaryPath = dictionary.Connection;
            }
            catch (MLifter.DAL.NoValidArchiveHeaderException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                return String.Empty;
            }
            catch (MLifter.DAL.NoValidArchiveException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                return String.Empty;
            }
            catch (System.Xml.XmlException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                return String.Empty;
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                return String.Empty;
            }
            catch (MLifter.DAL.DictionaryFormatNotSupported ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                return String.Empty;
            }
            catch (MLifter.DAL.DictionaryNotDecryptedException ex)
            {
                Debug.WriteLine("Pack.Unpack() - " + filename + " - " + ex.Message);
                return String.Empty;
            }
            catch
            {
                if (dictionary != null) dictionary.Dispose();
                return String.Empty;
            }
            finally
            {
                if (dictionary != null) dictionary.Dispose();
                Cursor.Current = Cursors.Default;
                m_LoadMsg.Hide();
            }

            return dictionaryPath;
        }

        private void ShowStatusMessage(string msg)
        {
            if (SetStatusDelegate != null)
            {
                SetStatusDelegate.Invoke(msg, false);
                return;
            }

            m_LoadMsg.InfoMessage = "Unpacking";
            m_LoadMsg.EnableProgressbar = true;
            m_LoadMsg.SetProgress(0);
            Cursor.Current = Cursors.WaitCursor;
            m_LoadMsg.Show();
        }
        private void HideStatusMessage()
        {
            if (SetStatusDelegate != null)
                return;

            m_LoadMsg.Invoke((MethodInvoker)delegate
            {
                m_LoadMsg.Hide();
            });
        }
        private void Unzip_ProgressChanged(object sender, ProgressChangedEventArgs progressArgs)
        {
            if (SetStatusDelegate != null)
            {
                SetStatusDelegate.Invoke(string.Format("Unpacking - {0}%", progressArgs.ProgressPercentage), true);
                return;
            }

            m_LoadMsg.SetProgress(progressArgs.ProgressPercentage);
        }
    }

}
