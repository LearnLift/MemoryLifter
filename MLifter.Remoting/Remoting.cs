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
using System.Diagnostics;
using System.IO;
using System.Threading;
using MLifter.Classes;
using System.Runtime.Remoting;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using MLifter.DAL;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Lifetime;

namespace MLifterRemoting
{
    /// <summary>
    /// Provides services for remotely accessing MemoryLifter functions.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-06-29</remarks>
    public class Remoting : IDisposable
    {
        //IPC connection data
        private static string UniqueChannelName = string.Empty;
        private static string UniqueChannelPortName = string.Empty;
        private static string ClientURL = string.Empty;
        private static string ServiceURL = string.Empty;

        GlobalDictionaryLoader loader = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Remoting"/> class and connects to MemoryLifter.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        /// <exception cref="MLCouldNotBeStartedException">Occurs when MemoryLifter did not start or did not react to connection attempts.</exception>
        /// <exception cref="MLNotReadyException">Occurs when MemoryLifter is not ready for being remote controlled.</exception>
        public Remoting()
        {
            LifetimeServices.LeaseManagerPollTime = TimeSpan.FromSeconds(3);
            Connect();
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        /// <exception cref="MLCouldNotBeStartedException">Occurs when MemoryLifter did not start or did not react to connection attempts.</exception>
        /// <exception cref="MLNotReadyException">Occurs when MemoryLifter is not ready for being remote controlled.</exception>
        private void Connect()
        {
            Debug.WriteLine("Connecting to MemoryLifter...");
            MLifter.Program.GetIPCData(out UniqueChannelName, out UniqueChannelPortName, out ClientURL, out ServiceURL);
            loader = (GlobalDictionaryLoader)RemotingServices.Connect(typeof(GlobalDictionaryLoader), ClientURL);

            int tries;

            try
            {
                tries = 20;
                while (!loader.IsMLReady())
                {
                    Thread.Sleep(50);
                    if (tries-- < 0)
                        throw new MLNotReadyException();
                }
            }
            catch (RemotingException)
            {
                StartML();
                Thread.Sleep(200);

                try
                {
                    tries = 100;
                    while (!loader.IsMLReady())
                    {
                        Thread.Sleep(50);
                        if (tries-- < 0)
                            throw new MLNotReadyException();
                    }
                }
                catch (RemotingException)
                {
                    throw;
                }
            }

            return;
        }

        /// <summary>
        /// Opens the learning module (or schedules it for opening).
        /// </summary>
        /// <param name="configFilePath">The config file path.</param>
        /// <param name="LmId">The lm id.</param>
        /// <param name="LmName">Name of the lm.</param>
        /// <param name="UserId">The user id.</param>
        /// <param name="UserName">Name of the user.</param>
        /// <param name="UserPassword">The user password (needed for FormsAuthentication, else empty).</param>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        /// <exception cref="MLHasOpenFormsException">Occurs when settings or the learning module could not be changed because MemoryLifter has other forms/windows open.</exception>
        /// <exception cref="ConfigFileParseException">Occurs when the supplied config file could not be parsed properly or does not contain a valid connection.</exception>
        /// <exception cref="MLCouldNotBeStartedException">Occurs when MemoryLifter did not start or did not react to connection attempts.</exception>
        public void OpenLearningModule(string configFilePath, int LmId, string LmName, int UserId, string UserName, string UserPassword)
        {
            ConnectionStringHandler csHandler = new ConnectionStringHandler(configFilePath);
            if (csHandler.ConnectionStrings.Count < 1)
                throw new ConfigFileParseException();

            Debug.WriteLine(string.Format("Config file parsed ({0}), building connection...", configFilePath));

            IConnectionString connectionString = csHandler.ConnectionStrings[0];

            ConnectionStringStruct css = new ConnectionStringStruct();
            css.LmId = LmId;
            css.Typ = connectionString.ConnectionType;
            css.ConnectionString = connectionString.ConnectionString;
            if (connectionString is UncConnectionStringBuilder)
            {
                css.ConnectionString = Path.Combine(css.ConnectionString, LmName + Helper.EmbeddedDbExtension);
                css.Typ = DatabaseType.MsSqlCe;
            }

            if (connectionString is ISyncableConnectionString)
            {
                ISyncableConnectionString syncConnectionString = (ISyncableConnectionString)connectionString;
                css.LearningModuleFolder = syncConnectionString.MediaURI;
                css.ExtensionURI = syncConnectionString.ExtensionURI;
                css.SyncType = syncConnectionString.SyncType;
            }

            LearningModulesIndexEntry entry = new LearningModulesIndexEntry(css);
            entry.User = new DummyUser(UserId, UserName);
            ((DummyUser)entry.User).Password = UserPassword;
            entry.UserName = UserName;
            entry.UserId = UserId;
            entry.Connection = connectionString;
            entry.DisplayName = LmName;
            entry.SyncedPath = LmName + Helper.SyncedEmbeddedDbExtension;

            Debug.WriteLine("Opening learning module...");

            try
            {
                loader.LoadDictionary(entry);
            }
            catch (MLifter.Classes.FormsOpenException)
            {
                throw new MLHasOpenFormsException();
            }
        }

        /// <summary>
        /// Selects the learning chapters (or schedules them for being selected).
        /// </summary>
        /// <param name="chapterIds">The chapter ids.</param>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        /// <exception cref="MLHasOpenFormsException">Occurs when settings or the learning module could not be changed because MemoryLifter has other forms/windows open.</exception>
        /// <exception cref="MLCouldNotBeStartedException">Occurs when MemoryLifter did not start or did not react to connection attempts.</exception>
        public void SelectLearningChapters(int[] chapterIds)
        {
            Debug.WriteLine("Selecting learning chapters...");

            try
            {
                loader.SelectLearningChapters(chapterIds);
            }
            catch (MLifter.Classes.FormsOpenException)
            {
                throw new MLHasOpenFormsException();
            }
        }

        /// <summary>
        /// Starts the MemoryLifter process.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        /// <exception cref="MLCouldNotBeStartedException">Occurs when MemoryLifter did not start or did not react to connection attempts.</exception>
        private void StartML()
        {
            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = MLifter.Program.GetAssemblyPath(true);
            si.WorkingDirectory = Path.GetDirectoryName(si.FileName);
            si.UseShellExecute = true;
            Debug.WriteLine(string.Format("Starting MemoryLifter program ({0})", si.FileName));
            try
            {
                Process ml = Process.Start(si);
                ml.WaitForInputIdle();
                if (ml.HasExited)
                {
                    Debug.WriteLine("ML did not start successfully.");
                    throw new MLCouldNotBeStartedException("MemoryLifter exited immediately.");
                }
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new MLCouldNotBeStartedException("Error launching the MemoryLifter process.", e);
            }
            return;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public void Dispose()
        {
            if (loader != null)
                loader = null;
        }

        #endregion
    }

    /// <summary>
    /// Occurs when settings or the learning module could not be changed because MemoryLifter has other forms/windows open.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-06-26</remarks>
    [global::System.Serializable]
    public class MLHasOpenFormsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MLHasOpenFormsException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLHasOpenFormsException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLHasOpenFormsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLHasOpenFormsException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLHasOpenFormsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLHasOpenFormsException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLHasOpenFormsException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        protected MLHasOpenFormsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Occurs when the supplied config file could not be parsed properly or does not contain a valid connection.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-06-29</remarks>
    [global::System.Serializable]
    public class ConfigFileParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigFileParseException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public ConfigFileParseException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigFileParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public ConfigFileParseException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigFileParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public ConfigFileParseException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigFileParseException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        protected ConfigFileParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Occurs when MemoryLifter did not start properly.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-06-29</remarks>
    [global::System.Serializable]
    public class MLCouldNotBeStartedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MLCouldNotBeStartedException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLCouldNotBeStartedException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLCouldNotBeStartedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLCouldNotBeStartedException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLCouldNotBeStartedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLCouldNotBeStartedException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLCouldNotBeStartedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        protected MLCouldNotBeStartedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Occurs when MemoryLifter is not ready for being remote controlled.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-06-29</remarks>
    [global::System.Serializable]
    public class MLNotReadyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MLNotReadyException"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLNotReadyException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLNotReadyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLNotReadyException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLNotReadyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public MLNotReadyException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MLNotReadyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        protected MLNotReadyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
