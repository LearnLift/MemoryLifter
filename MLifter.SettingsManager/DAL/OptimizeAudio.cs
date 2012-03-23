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
using System.IO;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using System.Threading;
using MLifter.SettingsManager.Properties;

namespace MLifterSettingsManager.DAL
{
    public class OptimizeAudio
    {
        MLifter.DAL.Interfaces.IDictionary learningModule = null;
        Thread optimizationThread = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizeAudio"/> class.
        /// </summary>
        /// <param name="learningModule">The learning module.</param>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        public OptimizeAudio(MLifter.DAL.Interfaces.IDictionary learningModule)
        {
            this.learningModule = learningModule;
        }

        public delegate void CallbackDelegate(string status, double progress);

        /// <summary>
        /// Starts the optimization.
        /// </summary>
        /// <param name="progress">The progress.</param>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        public void StartOptimization(CallbackDelegate progress)
        {
            StopOptimization();
            optimizationThread = new Thread(new ParameterizedThreadStart(Optimize));
            optimizationThread.IsBackground = true;
            optimizationThread.Name = "Audio Optimization Thread";
            optimizationThread.Priority = ThreadPriority.BelowNormal;
            optimizationThread.Start(progress);
        }

        /// <summary>
        /// Stops the optimization.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        public void StopOptimization()
        {
            if (optimizationThread != null && optimizationThread.IsAlive)
                optimizationThread.Abort();
        }

        /// <summary>
        /// Optimizes this instance.
        /// </summary>
        /// <param name="progress">The progress.</param>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        public void Optimize(object progressCallback)
        {
            CallbackDelegate progress = null;

            if (progressCallback is CallbackDelegate)
                progress = (CallbackDelegate)progressCallback;

            string sourceFile = null, targetFile = null;

            if (progress != null)
                progress("Fetching media objects...", 0);

            //fetch all "interesting" media objects into a list
            Dictionary<int, IMedia> medias = new Dictionary<int, IMedia>();

            foreach (ICard card in learningModule.Cards.Cards)
            {
                foreach (IMedia media in card.QuestionMedia.Concat(card.AnswerMedia))
                {
                    if (media.MediaType != EMedia.Audio || media.Extension.ToLowerInvariant() != ".mp3")
                        continue;

                    if (medias.ContainsKey(media.Id))
                        continue;

                    medias[media.Id] = media;
                }
            }

            //process each media
            int mediaCounter = 0;
            foreach (KeyValuePair<int, IMedia> pair in medias)
            {
                mediaCounter++;

                IMedia media = pair.Value;

                if (progress != null)
                    progress("Processing " + media.Filename + " (" + media.MediaSize + " Bytes)", 1.0 * mediaCounter / medias.Count);

                try
                {
                    sourceFile = GetTempFile(media.Extension);

                    //write to temp file
                    using (Stream mediaStream = media.Stream)
                    {
                        using (Stream output = new FileStream(sourceFile, FileMode.Create))
                        {
                            byte[] buffer = new byte[32 * 1024];
                            int read;

                            while ((read = mediaStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                output.Write(buffer, 0, read);
                            }
                        }
                    }

                    targetFile = GetTempFile(media.Extension);

                    //convert to cbr
                    string command = String.Format(Settings.Default.VBRFixCommand, sourceFile, targetFile);
                    StartExternalExe(Settings.Default.VBRFixApplication, command, false, false, progress);

                    //write temp file back
                    media.Filename = targetFile;
                }
                finally
                {
                    if (!String.IsNullOrEmpty(sourceFile))
                        File.Delete(sourceFile);

                    if (!String.IsNullOrEmpty(targetFile))
                        File.Delete(targetFile);
                }
            }

            if (progress != null)
                progress("Cleaning up old media objects...", -1);

            learningModule.ClearUnusedMedia();

            OnOptimizationFinished();
        }

        /// <summary>
        /// Called when [optimization finished].
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        private void OnOptimizationFinished()
        {
            if (OptimizationFinished != null)
                OptimizationFinished(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when [optimization is finished].
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        public event EventHandler OptimizationFinished;

        /// <summary>
        /// Gets a temp file name.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        private string GetTempFile(string extension)
        {
            string tempFile;
            do
            {
                tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + extension);
            }
            while (File.Exists(tempFile));
            return tempFile;
        }

        /// <summary>
        /// Starts an external executable.
        /// </summary>
        /// <param name="executable">The executable.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="showwindow">if set to <c>true</c> [showwindow].</param>
        /// <param name="minimizewindow">if set to <c>true</c> [minimizewindow].</param>
        /// <param name="progress">The progress.</param>
        /// <remarks>Documented by Dev02, 2008-03-17</remarks>
        private void StartExternalExe(string executable, string parameters, bool showwindow, bool minimizewindow, CallbackDelegate progress)
        {
            FileInfo exe = GetExecutablePath(executable);
            if (exe != null)
            {
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.FileName = exe.FullName;
                pi.WorkingDirectory = exe.Directory.FullName;
                pi.Arguments = parameters;
                pi.WindowStyle = showwindow ? (minimizewindow ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal) : ProcessWindowStyle.Hidden;
                Process externalprocess = Process.Start(pi);
                externalprocess.WaitForExit();
            }
            else if (progress != null)
            {
                progress("Error: Worker Executable not found: " + executable, 1);
                StopOptimization();
            }

        }

        /// <summary>
        /// Checks the executable path.
        /// </summary>
        /// <param name="executable">The executable.</param>
        /// <returns>The FileInfo, if it is valid, else null.</returns>
        /// <remarks>Documented by Dev02, 2008-04-10</remarks>
        private FileInfo GetExecutablePath(string executable)
        {
            if (string.IsNullOrEmpty(executable))
                return null;

            FileInfo exefile;

            if (Path.IsPathRooted(executable))
                exefile = new FileInfo(executable);
            else
                exefile = new FileInfo(Path.Combine(System.Windows.Forms.Application.StartupPath, executable));

            if (!exefile.Exists)
            {
                exefile = null;
            }

            return exefile;
        }
    }
}
