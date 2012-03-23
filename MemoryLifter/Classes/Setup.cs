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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Xml;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using MLifter.Properties;
using System.Reflection;
using System.Configuration;
using MLifter.BusinessLayer;
using System.Text.RegularExpressions;
using MLifter.Generics;
using MLifter.DAL.Interfaces;
using MLifter.DAL;

namespace MLifter
{
    /// <summary>
    /// Class for setting up the environment.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-11-15</remarks>
    public static class Setup
    {
        /// <summary>
        /// Gets the global config path.
        /// When on stick the global config path is: "%AppData%\MemoryLifter\2.x\Config"
        /// When local the global config path is: "%ProgramFiles%\MemoryLifter\Config"
        /// </summary>
        /// <value>The global config path.</value>
        /// <remarks>Documented by Dev03, 2009-04-17</remarks>
        public static string GlobalConfigPath
        {
            get
            {
                //[ML-1773] Imported connection file is not saved on stick
                if (RunningFromStick())
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine(Properties.Settings.Default.AppDataFolder, Properties.Settings.Default.ConfigurationFolder));
                else
                    return Path.Combine(Application.StartupPath, Properties.Settings.Default.ConfigurationFolder);
            }
        }
        /// <summary>
        /// Gets the user config path.
        /// When on stick the user config path is: "G:\MemoryLifter\Config"
        /// When local the user config path is: "%AppData%\MemoryLifter\2.x\Config"
        /// </summary>
        /// <value>The user config path.</value>
        /// <remarks>Documented by Dev03, 2009-04-17</remarks>
        public static string UserConfigPath
        {
            get
            {
                //[ML-1773] Imported connection file is not saved on stick
                if (RunningFromStick())
                    return Path.Combine(Application.StartupPath, Properties.Settings.Default.ConfigurationFolder);
                else
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine(Properties.Settings.Default.AppDataFolder, Properties.Settings.Default.ConfigurationFolder));
            }
        }
        /// <summary>
        /// Gets the synced modules path.
        /// </summary>
        /// <value>The synced modules path.</value>
        /// <remarks>Documented by Dev03, 2009-04-17</remarks>
        public static string SyncedModulesPath
        {
            get
            {
                if (RunningFromStick())
                    return Path.Combine(Application.StartupPath, Properties.Settings.Default.SyncedLMFolder);
                else
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine(Properties.Settings.Default.AppDataFolder, Properties.Settings.Default.SyncedLMFolder));
            }
        }

        /// <summary>
        /// Sets the show start page config.
        /// </summary>
        /// <param name="show">if set to <c>true</c> [show].</param>
        /// <remarks>Documented by Dev03, 2009-06-03</remarks>
        public static void SetShowStartPage(bool show)
        {
            Settings.Default.ShowStartPage = show;
            Settings.Default.Save();
        }

        /// <summary>
        /// Gets the recent learning modules path.
        /// </summary>
        /// <value>The recent learning modules path.</value>
        /// <remarks>Documented by Dev03, 2009-04-26</remarks>
        public static string RecentLearningModulesPath
        {
            get
            {
                if (RunningFromStick())
                    return Path.Combine(Application.StartupPath, Resources.RECENT_LEARNING_MODULES_FILE);
                else
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine(Properties.Settings.Default.AppDataFolder, Resources.RECENT_LEARNING_MODULES_FILE));
            }
        }

        /// <summary>
        /// Gets the installed extensions file path.
        /// </summary>
        /// <value>The installed extensions file path.</value>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        public static string InstalledExtensionsFilePath
        {
            get
            {
                if (RunningFromStick())
                    return Path.Combine(Application.StartupPath, Resources.INSTALLED_EXTENSIONS_FILE);
                else
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.Combine(Properties.Settings.Default.AppDataFolder, Resources.INSTALLED_EXTENSIONS_FILE));
            }
        }

        /// <summary>
        /// Gets the file path for the new error reports.
        /// </summary>
        /// <value>The error reports file path.</value>
        /// <remarks>Documented by Dev02, 2009-07-06</remarks>
        public static string ErrorReportsFilePath
        {
            get
            {
                if (RunningFromStick())
                    return Path.Combine(Application.StartupPath, Properties.Settings.Default.AppDataFolderErrorReportsNew);
                else
                    return Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Path.Combine(Properties.Settings.Default.AppDataFolder, Properties.Settings.Default.AppDataFolderErrorReportsNew));
            }
        }

        /// <summary>
        /// Gets the user profile foot print path. Used for error reporting only.
        /// </summary>
        /// <value>The user profile foot print path.</value>
        /// <remarks>Documented by Dev03, 2009-07-15</remarks>
        public static string UserProfileFootPrintPath
        {
            get
            {
                if (Setup.RunningFromStick())
                    return Application.StartupPath;
                else
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.AppDataFolder);
            }
        }


        /// <summary>
        /// Initializes the profile environment.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-15</remarks>
        public static void InitializeProfile(bool copyDemoDictionary, string dictionaryParentPath)
        {
            MLifter.Controls.LoadStatusMessage progress = new MLifter.Controls.LoadStatusMessage(Resources.UNPACK_TLOADMSG, 100, true);

            try
            {
                if (!Directory.Exists(dictionaryParentPath))
                    Directory.CreateDirectory(dictionaryParentPath);

                if (!copyDemoDictionary)
                    return;

                string zipFilePath = Path.Combine(Application.StartupPath, Resources.SETUP_STARTUP_FOLDERS);

                if (!File.Exists(zipFilePath))
                    return;

                progress.SetProgress(0);
                Cursor.Current = Cursors.WaitCursor;
                progress.Show();

                long zCount = 0;
                ZipFile zFile = null;
                try
                {
                    zFile = new ZipFile(zipFilePath);
                    zCount = zFile.Count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (zFile != null)
                        zFile.Close();
                }
                if (zCount == 0)
                    return;

                using (ZipInputStream zipFile = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    ZipEntry entry;

                    int zCounter = 1;
                    while ((entry = zipFile.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(entry.Name);
                        string fileName = Path.GetFileName(entry.Name);

                        if (directoryName.Length > 0)
                            Directory.CreateDirectory(Path.Combine(dictionaryParentPath, directoryName));

                        if (fileName.Length > 0)
                        {
                            using (FileStream stream = File.Open(Path.Combine(Path.Combine(dictionaryParentPath, directoryName), fileName), FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                int bufferSize = 1024;
                                byte[] buffer = new byte[bufferSize];

                                while (bufferSize > 0)
                                {
                                    bufferSize = zipFile.Read(buffer, 0, buffer.Length);
                                    if (bufferSize > 0)
                                        stream.Write(buffer, 0, bufferSize);
                                }
                            }
                        }

                        if ((Path.GetExtension(fileName) == MLifter.DAL.Helper.OdxExtension)
                            || (Path.GetExtension(fileName) == MLifter.DAL.Helper.EmbeddedDbExtension))
                        {
                            string dicPath = Path.Combine(Path.Combine(dictionaryParentPath, directoryName), fileName);
                            LearningModulesIndexEntry indexEntry = new LearningModulesIndexEntry();
                            indexEntry.DisplayName = "StartUp";
                            indexEntry.ConnectionString = new MLifter.DAL.Interfaces.ConnectionStringStruct(MLifter.DAL.DatabaseType.MsSqlCe, dicPath, false);
                            Setup.AddRecentLearningModule(indexEntry);
                        }

                        progress.SetProgress((int)Math.Floor(100.0 * (zCounter++) / zCount));
                    }
                }
                Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.INITIALIZE_ENVIRONMENT_ERROR_TEXT, Properties.Resources.INITIALIZE_ENVIRONMENT_ERROR_CAPTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progress.Dispose();
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Returns the path of the "Parent folder of ML Dictionary"
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2007-12-12</remarks>
        public static string DictionaryParentPath
        {
            get
            {
                string dictionaryParentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (RunningFromStick())
                {
                    if (new DirectoryInfo(Application.StartupPath).Parent != null)
                        dictionaryParentPath = new DirectoryInfo(Application.StartupPath).Parent.FullName;
                    else
                        dictionaryParentPath = new DirectoryInfo(Application.StartupPath).FullName;
                }
                return dictionaryParentPath;
            }
        }

        /// <summary>
        /// Upgrades the settings from an earlier version.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-14</remarks>
        public static void UpgradeFromEarlierVersion()
        {
            if (!Settings.Default.Upgraded)
            {
                try //[ML-397] ML sometimes throws an exception when trying to update old user seetings
                {
                    Settings.Default.Upgrade();
                }
                catch { }
                Settings.Default.Upgraded = true;
                Settings.Default.Save();

                //try to import recent files
                List<string> recent = GetRecentFilesFromStick(); ;
                if (recent.Count > 0)
                {
                    recent.Reverse();

                    recent.ForEach(delegate(String file)
                    {
                        try
                        {
                            if (!File.Exists(file))
                                return;
                        }
                        catch { return; }

                        LearningModulesIndexEntry entry = FolderIndexEntry.CreateNewOdxLearningModuleEntryStatic(file);
                        RecentLearningModules.Add(entry);

                    });

                    RecentLearningModules.Dump(RecentLearningModulesPath);
                    Settings.Default.RecentFiles = string.Empty;
                }

                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Upgrades the settings from registry (V2.0.1).
        /// </summary>
        /// <returns>[true] if values were loaded from registry</returns>
        /// <remarks>Documented by Dev05, 2007-11-14</remarks>
        [Obsolete()]
        public static bool UpgradeFromRegistry()
        {
            bool fromRegistry = false;
            if (!Settings.Default.MLifter_RegistryLoaded)
            {
                fromRegistry = true;
                Settings.Default.MLifter_RegistryLoaded = true;

                try
                {
                    Microsoft.Win32.RegistryKey Registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\LearnLift\\MLifter20");
                    if (Registry != null)
                    {
                        Settings.Default.CopyMediaPromt = (((string)Registry.GetValue("CopyMediaPrompt", "OK")).ToLower() == "ok");
                        //Settings.Default.CurrentLanguage = (string)Registry.GetValue("CurrentLanguage", "en");
                        //Settings.Default.CurrentStyle = (string)Registry.GetValue("CurrentStyle", "default");
                        Settings.Default.DicDir = (string)Registry.GetValue("DicDir");
                        //Settings.Default.Registered = true;  // (((string)Registry.GetValue("MLReg", "Yes")).ToLower() == "yes");
                        //Controls.HelperClass.NewsDate = (string)Registry.GetValue("NewsDate");
                        string rf = (string)Registry.GetValue("RecentFiles");
                        if (!string.IsNullOrEmpty(rf))
                            Settings.Default.RecentFiles = rf;
                        //Settings.Default.RecentFiles = MLifter.DAL.WordList.Create(rf, new string[] {",", "\""}).QuotedCommaText;
                        Controls.HelperClass.ShowNews = (((string)Registry.GetValue("ShowNews", "yes")).ToLower() == "yes");
                        try
                        {
                            Settings.Default.Statistics = Convert.ToInt32(Registry.GetValue("Statistics", "0"));
                        }
                        catch
                        {
                            Settings.Default.Statistics = 0;
                        }
                        Settings.Default.SynonymPromt = (((string)Registry.GetValue("SynonymPrompt", "ok")).ToLower() == "ok");
                    }
                }
                catch { }

                Settings.Default.Save();
            }
            return fromRegistry;
        }

        /// <summary>
        /// Checks the dic dir.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        public static void CheckDicDir()
        {
            IConnectionString cs = ConnectionStringHandler.GetDefaultConnectionString(Setup.UserConfigPath);

            string dicDir = Settings.Default.DicDir;
            if (cs == null)
            {
                Regex regexAppData = new Regex(@"\%appdata\%", RegexOptions.IgnoreCase);
                if (regexAppData.Match(dicDir).Success)
                    dicDir = regexAppData.Replace(dicDir, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }

            // check for value of dictionary directory
            FolderBrowserDialog DirDialog = new FolderBrowserDialog();

            while (string.IsNullOrEmpty(dicDir) || !Directory.Exists(dicDir) || !Path.IsPathRooted(dicDir))
            {
                MessageBox.Show(Resources.PERSONAL_FOLDER_TEXT, Resources.PERSONAL_FOLDER_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (DirDialog.ShowDialog() == DialogResult.OK)
                    dicDir = DirDialog.SelectedPath;
            }

            if (dicDir != Settings.Default.DicDir || ConnectionStringHandler.GetDefaultConnectionString(Setup.UserConfigPath) == null)
            {
                Setup.CreateDefaultConfig(dicDir);
            }
        }

        /// <summary>
        /// Creates the default config.
        /// </summary>
        /// <param name="defaultLerningModulesPath">The default lerning modules path.</param>
        /// <remarks>Documented by Dev03, 2009-05-07</remarks>
        public static void CreateDefaultConfig(string defaultLerningModulesPath)
        {
            if (Directory.Exists(defaultLerningModulesPath))
            {
                Settings.Default.DicDir = defaultLerningModulesPath;
                Settings.Default.Save();

                string configPath = Setup.UserConfigPath;

                //Get the only connection from 
                IConnectionString defaultConnection = ConnectionStringHandler.GetDefaultConnectionString(configPath);
                if (defaultConnection as UncConnectionStringBuilder == null)
                    ConnectionStringHandler.CreateUncConnection(Resources.DEFAULT_CONNECTION_NAME, defaultLerningModulesPath, configPath, Resources.DEFAULT_CONNECTION_FILE, true, Setup.IsPathOnStick(defaultLerningModulesPath));
                else
                {
                    UncConnectionStringBuilder builder = new UncConnectionStringBuilder(defaultLerningModulesPath, true, Setup.IsPathOnStick(defaultLerningModulesPath));
                    builder.Name = defaultConnection.Name;

                    ConnectionStringHandler.CreateUncConnection(builder, Path.GetDirectoryName(defaultConnection.ConfigFileName), Path.GetFileName(defaultConnection.ConfigFileName));
                }
            }
        }

        /// <summary>
        /// Checks if the application is running from a removable Media (usb stick).
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2007-12-06</remarks>
        public static bool RunningFromStick()
        {
            try
            {
                if (Properties.Settings.Default.ForceOnStickMode)
                    return true;
            }
            catch
            { }

            return Generics.Methods.IsRunningFromStick();
        }

        /// <summary>
        /// Loads the settings from stick.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-12-06</remarks>
        public static void LoadSettingsFromStick()
        {
            SettingsOnStick(SettingsOnStickDirection.Load);
        }

        /// <summary>
        /// Saves the settings to stick.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-12-06</remarks>
        public static void SaveSettingsToStick()
        {
            SettingsOnStick(SettingsOnStickDirection.Save);
        }

        /// <summary>
        /// The Direction in which the settings should be transfered.
        /// </summary>
        enum SettingsOnStickDirection
        {
            Load,
            Save
        };

        /// <summary>
        /// Transfers settings from/to the stick according to the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <remarks>Documented by Dev02, 2007-12-06</remarks>
        static void SettingsOnStick(SettingsOnStickDirection direction)
        {
            if (!RunningFromStick())
                return;

            if (direction == SettingsOnStickDirection.Save)
            {
                //replace settings with placeholders
                Properties.Settings.Default.DicDir = ReplacePath(Properties.Settings.Default.DicDir, SettingsOnStickReplaceDirection.PathToPlaceholder);
                Properties.Settings.Default.RecentFiles = ReplacePath(Properties.Settings.Default.RecentFiles, SettingsOnStickReplaceDirection.PathToPlaceholder);
                //save current properties before copying to stick
                Properties.Settings.Default.Save();
            }

            System.Configuration.Configuration localconfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            FileInfo sourcefile, destfile;
            string userconfigfileformat = Properties.Resources.SETUP_STICKMODE_CONFIGFILE;

            switch (direction)
            {
                case SettingsOnStickDirection.Load:
                    sourcefile = new FileInfo(string.Format(userconfigfileformat, config.FilePath));
                    destfile = new FileInfo(localconfig.FilePath);
                    break;
                case SettingsOnStickDirection.Save:
                    sourcefile = new FileInfo(localconfig.FilePath);
                    destfile = new FileInfo(string.Format(userconfigfileformat, config.FilePath));
                    break;
                default:
                    return;
            }

            try
            {
                sourcefile.Directory.Create();
                destfile.Directory.Create();
                File.Copy(sourcefile.FullName, destfile.FullName, true);
            }
            catch
            {
                return;
            }

            //load current properties after loading from stick
            try
            {
                Properties.Settings.Default.Reload();
                Properties.Settings.Default.DicDir = ReplacePath(Properties.Settings.Default.DicDir, SettingsOnStickReplaceDirection.PlaceholderToPath);
                Properties.Settings.Default.RecentFiles = ReplacePath(Properties.Settings.Default.RecentFiles, SettingsOnStickReplaceDirection.PlaceholderToPath);
                Properties.Settings.Default.Save();
            }
            catch (System.Configuration.ConfigurationErrorsException exp)
            {
                bool errorfixed = false;
                if (exp.InnerException != null && exp.InnerException is System.Configuration.ConfigurationErrorsException)
                {
                    System.Configuration.ConfigurationErrorsException innerexception = exp.InnerException as System.Configuration.ConfigurationErrorsException;
                    if (System.IO.File.Exists(innerexception.Filename))
                    {
                        try
                        {
                            System.IO.File.Delete(innerexception.Filename);
                            errorfixed = true;
                        }
                        catch { }
                    }
                }
                if (System.IO.File.Exists(exp.Filename))
                {
                    try
                    {
                        System.IO.File.Delete(exp.Filename);
                    }
                    catch { }
                }
                if (System.IO.File.Exists(sourcefile.FullName))
                {
                    try
                    {
                        System.IO.File.Delete(sourcefile.FullName);
                        errorfixed = true;
                    }
                    catch { }
                }
                if (errorfixed)
                {
                    Program.MainForm.BringToFront();
                    Program.MainForm.TopMost = true;
                    Application.DoEvents();

                    MessageBox.Show(Properties.Resources.INITIALIZE_SETTINGS_ERROR, Properties.Resources.INITIALIZE_SETTINGS_CAPTION);
                    Environment.Exit(-1);
                }
                else
                {
                    throw exp;
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        /// <summary>
        /// The direction to replace the path with placeholders.
        /// </summary>
        enum SettingsOnStickReplaceDirection
        {
            PathToPlaceholder,
            PlaceholderToPath
        };

        /// <summary>
        /// Replaces the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2007-12-06</remarks>
        static string ReplacePath(string sourcePath, SettingsOnStickReplaceDirection replaceDirection)
        {
            if (!RunningFromStick())
                return sourcePath;

            string replacement = Properties.Resources.SETUP_STICKMODE_DRIVELETTERREPLACEMENT;

            DirectoryInfo stickRoot = new DirectoryInfo(Application.StartupPath).Root;

            try
            {
                if (replaceDirection == SettingsOnStickReplaceDirection.PathToPlaceholder)
                {
                    return sourcePath.Replace(stickRoot.FullName, replacement);
                    //if (new FileInfo(sourcePath).Directory.Root.FullName == stickRoot.FullName)
                }
                else if (replaceDirection == SettingsOnStickReplaceDirection.PlaceholderToPath)
                {
                    return sourcePath.Replace(replacement, stickRoot.FullName);
                }
            }
            catch { }

            return sourcePath;
        }

        /// <summary>
        /// Determines whether [is path on stick] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is path on stick] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-05-22</remarks>
        public static bool IsPathOnStick(string path)
        {
            if (!RunningFromStick())
                return false;

            try
            {
                DirectoryInfo stickRoot = new DirectoryInfo(Application.StartupPath).Root;
                DirectoryInfo pathRoot = new DirectoryInfo(path).Root;
                return stickRoot.FullName == pathRoot.FullName;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is path on any stick] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is path on any stick] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-05-22</remarks>
        public static bool IsPathOnAnyStick(string path)
        {
            return Methods.IsOnMLifterStick(path);
        }

        /// <summary>
        /// Gets the application root.
        /// </summary>
        /// <value>The application root.</value>
        /// <remarks>Documented by Dev02, 2007-12-11</remarks>
        public static string ApplicationRoot
        {
            get { return new DirectoryInfo(Application.StartupPath).Root.FullName; }
        }

        /// <summary>
        /// Adds the recent learning module.
        /// </summary>
        /// <param name="learningModule">The learning module.</param>
        /// <remarks>Documented by Dev03, 2009-05-11</remarks>
        public static void AddRecentLearningModule(LearningModulesIndexEntry learningModule)
        {
            RecentLearningModules.Add(learningModule);
            RecentLearningModules.Dump(Setup.RecentLearningModulesPath);

            if (RunningFromStick())
                Setup.AddRecentFileToStick(learningModule);
        }

        /// <summary>
        /// Adds the recent file to the stick config.
        /// </summary>
        /// <param name="learningModule">The learning module.</param>
        /// <remarks>Documented by Dev03, 2009-05-12</remarks>
        private static void AddRecentFileToStick(LearningModulesIndexEntry learningModule)
        {
            if (learningModule.ConnectionString.Typ == MLifter.DAL.DatabaseType.MsSqlCe)
            {
                List<string> recentFiles = Setup.GetRecentFilesFromStick();
                if (recentFiles.Count > 0)
                {
                    recentFiles.Reverse();
                    if (recentFiles.Exists(r => r == learningModule.ConnectionString.ConnectionString))
                        recentFiles.Remove(learningModule.ConnectionString.ConnectionString);
                    recentFiles.Add(learningModule.ConnectionString.ConnectionString);

                    if (recentFiles.Count > Settings.Default.RecentFilesCount && Settings.Default.RecentFilesCount > 0)
                        recentFiles.Remove(recentFiles[0]);
                    recentFiles.Reverse();

                    Settings.Default.Reload();
                    Settings.Default.RecentFiles = "\"" + String.Join("\",\"", recentFiles.ToArray()) + "\"";
                }
                else
                {
                    Settings.Default.Reload();
                    Settings.Default.RecentFiles = "\"" + learningModule.ConnectionString.ConnectionString + "\"";
                }
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Gets the recent files from stick.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-05-12</remarks>
        private static List<string> GetRecentFilesFromStick()
        {
            string[] words;
            Settings.Default.Reload();
            string wordList = Settings.Default.RecentFiles.Trim();
            if (wordList.Length > 0)
            {
                words = wordList.Split(new string[] { "\",\"", "\", \"" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Length; i++)
                {
                    words[i] = words[i].Trim();
                }
                //remove trailing and leading '"' which is used to enclose synonym text
                if (words.Length > 0)
                {
                    if (words[0].StartsWith("\"")) words[0] = words[0].Substring(1);
                    if (words[words.Length - 1].EndsWith("\"")) words[words.Length - 1] = words[words.Length - 1].Substring(0, words[words.Length - 1].Length - 1);
                }
                //decode some protected chars ('"' and ',') 
                for (int i = 0; i < words.Length; i++)
                {
                    words[i] = words[i].Replace("\"\"", "\"").Replace("\\,", ",");
                }
            }
            else
            {
                words = new string[] { };
            }
            return new List<string>(words);
        }
    }
}
