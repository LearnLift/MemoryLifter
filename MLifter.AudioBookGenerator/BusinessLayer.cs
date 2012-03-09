using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using MLifterAudioBookGenerator.Audio;
using System.Xml.Serialization;
using MLifter.AudioTools.Codecs;
using MLifter.AudioBookGenerator.Properties;

namespace MLifterAudioBookGenerator
{
    public static class BusinessLayer
    {
        /// <summary>
        /// The main worker thread.
        /// </summary>
        public static Thread workerthread = null;

        /// <summary>
        /// Generates the audio book.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="audiobook">The target audiobook file.</param>
        /// <param name="options">The options.</param>
        /// <param name="worker">The worker.</param>
        /// <param name="codecs">The codecs.</param>
        /// <remarks>Documented by Dev02, 2008-03-10</remarks>
        public static void GenerateAudioBook(IDictionary dictionary, FileInfo audiobook, AudioBookOptions options, Codecs codecs)
        {
            if (workerthread != null && workerthread.IsAlive)
            {
                AddLog("Error: Another operation is still active.");
                return;
            }

            workerthread = new Thread(delegate()
            {
                try
                {
                    Dictionary<string, Codec> encodeCodecs = codecs.encodeCodecs;
                    Dictionary<string, Codec> decodeCodecs = codecs.decodeCodecs;

                    if (audiobook.Extension.ToLowerInvariant() != Resources.AUDIO_WAVE_EXTENSION.ToLowerInvariant()
                        && !encodeCodecs.ContainsKey(audiobook.Extension.ToLowerInvariant()))
                    {
                        AddLog(string.Format("Specified extension ({0}) is not available => Check encoder settings", audiobook.Extension.ToLowerInvariant()));
                        return;
                    }

                    List<MediaFieldFile> sourcefiles = new List<MediaFieldFile>(); //source files from dictionary

                    Environment.CurrentDirectory = new FileInfo(dictionary.Connection).Directory.FullName;

                    //fetch source audio files
                    AddLog("Fetching source audio files");
                    int count = dictionary.Cards.Cards.Count;
                    int index = 0;
                    foreach (ICard card in dictionary.Cards.Cards)
                    {
                        sourcefiles.AddRange(GetCardMediaFiles(card, options.MediaFields));
                        ReportProgress(index++, count);
                    }

                    //search and generate a new temp folder
                    DirectoryInfo tempfolder;
                    int tempfolderindex = 0;
                    do
                    {
                        tempfolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Settings.Default.TempFolderPath + tempfolderindex.ToString()));
                        tempfolderindex++;
                    } while (tempfolder.Exists);
                    tempfolder.Create();

                    try
                    {
                        //convert all source files to wave where necessary
                        AddLog("Converting source files");
                        bool foundfile = false;
                        count = sourcefiles.Count;
                        index = 0;
                        for (int i = 0; i < sourcefiles.Count; i++)
                        {
                            if (sourcefiles[i].ContainsFile && decodeCodecs.ContainsKey(sourcefiles[i].Extension))
                            {
                                //decode file
                                string filename = sourcefiles[i].file.Name;
                                sourcefiles[i].file = decodeCodecs[sourcefiles[i].Extension].Decode(sourcefiles[i].file, tempfolder, 
									Settings.Default.ShowDecodingWindow, Settings.Default.MimimizeWindows);
                                if (sourcefiles[i].ContainsFile)
                                    foundfile = true;
                                else
                                    AddLog(string.Format("Decoding ({0}) did not produce a file => Check decoder settings", filename));
                            }
                            else if (sourcefiles[i].ContainsFile && sourcefiles[i].Extension == Resources.AUDIO_WAVE_EXTENSION.ToLowerInvariant())
                                foundfile = true;
                            else if (sourcefiles[i].ContainsFile)
                                AddLog(string.Format("Extension {0} not supported ({1}) => Check decoder settings", sourcefiles[i].Extension, sourcefiles[i].file.Name));
                            ReportProgress(index++, count);
                        }

                        if (!foundfile)
                        {
                            AddLog("No supported audio files found in the selected fields of the learning module");
                            ReportProgress(0, 0); //disable progress reporting
                        }
                        else
                        {
                            //concatenate all wave files
                            AddLog("Joining audio files");
                            FileInfo audiobookwave = new FileInfo(Path.Combine(tempfolder.FullName, Resources.AUDIOBOOK_DEFAULTNAME + Resources.AUDIO_WAVE_EXTENSION));
                            ABWaveCat wavecat = new ABWaveCat();
                            wavecat.Concatenate(sourcefiles, audiobookwave, options.Stereo);

                            ReportProgress(0, 0); //disable progress reporting

                            bool changeToWaveExtension = true; //fix for [MLA-1272] error message for file path without extension

                            if (audiobook.Extension.ToLowerInvariant() != Resources.AUDIO_WAVE_EXTENSION.ToLowerInvariant())
                            {
                                if (encodeCodecs.ContainsKey(audiobook.Extension.ToLowerInvariant()))
                                {
                                    //convert audiobook to specified format
                                    AddLog("Converting audiobook");
                                    FileInfo encodedfile = encodeCodecs[audiobook.Extension.ToLowerInvariant()].Encode(audiobookwave, tempfolder, 
										Settings.Default.ShowEncodingWindow, Settings.Default.MimimizeWindows);
                                    if (encodedfile != null)
                                    {
                                        try
                                        {
                                            if (audiobook.Exists)
                                                audiobook.Delete();
                                            encodedfile.MoveTo(audiobook.FullName);
                                            changeToWaveExtension = false;
                                        }
                                        catch (Exception e)
                                        {
                                            AddLog("Could not replace audio file: " + e.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        AddLog(string.Format("Encoding ({0}) did not produce a file => Check encoder settings", audiobook.Name));
                                    }
                                }
                            }

                            if (changeToWaveExtension) //change file to wave extension and force wave generation
                                audiobook = new FileInfo(Path.ChangeExtension(audiobook.FullName, Resources.AUDIO_WAVE_EXTENSION));

                            if (audiobook.Extension.ToLowerInvariant() == Resources.AUDIO_WAVE_EXTENSION.ToLowerInvariant())
                            {
                                try
                                {
                                    if (audiobook.Exists)
                                        audiobook.Delete();
                                    audiobookwave.MoveTo(audiobook.FullName);
                                }
                                catch (Exception e)
                                {
                                    AddLog("Could not replace audio file: " + e.Message);
                                    return;
                                }
                            }

                            AddLog("Finished: " + audiobook.FullName);
                        }
                    }
                    finally
                    {
                        //try to delete the temp directory (try multiple times in case it is still locked)
                        Thread cleanthread = new Thread(delegate()
                        {
                            int tries = 0;
                            while (tries++ < 10)
                            {
                                System.Threading.Thread.Sleep(1000);
                                try
                                {
                                    tempfolder.Delete(true);
                                    TempFiles.ForEach(f => f.Delete());
                                    break;
                                }
                                catch
                                { }
                            }
                        });
                        cleanthread.IsBackground = true;
                        cleanthread.Start();
                    }
                }
                catch (Exception exp)
                {
                    ReportProgress(0, 0); //disable progressbar
                    if (exp is ThreadAbortException)
                    {
                        AddLog("Operation cancelled");
                    }
                    else
                    {
                        AddLog("GenerateAudioBook Thread Exception: " + exp.ToString());
                    }
                }
                finally
                {
                    //fire the working thread finished event in every case
                    OnWorkingThreadFinished(EventArgs.Empty);
                }
            });
            workerthread.IsBackground = true;
            workerthread.Start();

            return;
        }

        /// <summary>
        /// Occurs when [working thread finished].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        public static event EventHandler WorkingThreadFinished;

        /// <summary>
        /// Raises the <see cref="E:WorkingThreadFinished"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        public static void OnWorkingThreadFinished(EventArgs e)
        {
            EventHandler handler = WorkingThreadFinished;
            if (handler != null)
            {
                foreach (EventHandler caster in handler.GetInvocationList())
                {
                    ISynchronizeInvoke SyncInvoke = caster.Target as ISynchronizeInvoke;
                    try
                    {
                        if (SyncInvoke != null && SyncInvoke.InvokeRequired)
                            SyncInvoke.Invoke(handler, new object[] { null, e });
                        else
                            caster(null, e);
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [working thread active].
        /// </summary>
        /// <value><c>true</c> if [working thread active]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-04-03</remarks>
        public static bool WorkingThreadActive
        {
            get
            {
                if (workerthread == null) return false;
                return workerthread.IsAlive;
            }
        }


        /// <summary>
        /// Aborts the working thread.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        internal static void AbortWorkingThread()
        {
            if (WorkingThreadActive)
                workerthread.Abort();

            //try to delete the temp directory (try multiple times in case it is still locked)
            Thread cleanthread = new Thread(delegate()
            {
                int tries = 0;
                while (tries++ < 10)
                {
                    System.Threading.Thread.Sleep(1000);
                    try
                    {
                        TempFiles.ForEach(f => f.Delete());
                        break;
                    }
                    catch
                    { }
                }
            });
            cleanthread.IsBackground = true;
            cleanthread.Start();
        }

        public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);

        /// <summary>
        /// Occurs when [progress changed].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        public static event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Raises the <see cref="E:ProgressChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MLifterAudioBookGenerator.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-20</remarks>
        public static void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;
            if (handler != null)
            {
                foreach (ProgressChangedEventHandler caster in handler.GetInvocationList())
                {
                    ISynchronizeInvoke SyncInvoke = caster.Target as ISynchronizeInvoke;
                    try
                    {
                        if (SyncInvoke != null && SyncInvoke.InvokeRequired)
                            SyncInvoke.Invoke(handler, new object[] { null, e });
                        else
                            caster(null, e);
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// Reports the progress.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="worker">The worker.</param>
        /// <remarks>Documented by Dev02, 2008-03-31</remarks>
        public static void ReportProgress(int index, int count)
        {
            double percent = count == 0 ? -1 : 1.0 * index * 100 / count;
            ProgressChangedEventArgs args = new ProgressChangedEventArgs();
            args.percentProgress = percent;
            args.enableProgressReporting = percent >= 0;
            OnProgressChanged(args);
        }

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>Documented by Dev02, 2008-03-21</remarks>
        public static void AddLog(string message)
        {
            string logtext = string.Format("{0} {1}", System.DateTime.Now.ToString(), message);
            if (logtext.Length > 0 && !logtext.EndsWith("."))
                logtext += ".";
            System.Diagnostics.Trace.WriteLine(logtext);
        }

        /// <summary>
        /// Gets the card media files.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="mediafield">The mediafield.</param>
        /// <returns>The found media files.</returns>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        private static IList<MediaFieldFile> GetCardMediaFiles(ICard card, List<MediaField> mediafields)
        {
            IList<MediaFieldFile> foundfiles = new List<MediaFieldFile>();
            foreach (MediaField mediafield in mediafields)
            {
                if (mediafield.Type == MediaField.TypeEnum.AudioField)
                {
                    IList<IMedia> medialist = null;
                    switch (mediafield.Side)
                    {
                        case MediaField.SideEnum.Question:
                            medialist = card.QuestionMedia;
                            break;
                        case MediaField.SideEnum.Answer:
                            medialist = card.AnswerMedia;
                            break;
                    }
                    if (medialist != null)
                    {
                        foreach (IMedia media in medialist)
                        {
                            if (media.MediaType == EMedia.Audio && ((IAudio)media).Example == mediafield.Example)
                            {
                                string sourceFile = GetTempFile(media.Extension);

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

                                FileInfo mediafile = new FileInfo(sourceFile);
                                if (mediafile.Exists)
                                {
                                    TempFiles.Add(mediafile);
                                    foundfiles.Add(new MediaFieldFile(mediafield, mediafile));
                                }
                            }
                        }
                    }
                }
                else if (mediafield.Type == MediaField.TypeEnum.Silence)
                {
                    foundfiles.Add(new MediaFieldFile(mediafield));
                }
            }

            return foundfiles;
        }
        private static List<FileInfo> TempFiles = new List<FileInfo>();
        /// <summary>
        /// Gets the temp file.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-06-05</remarks>
        private static string GetTempFile(string extension)
        {
            string tempFile;
            do
            {
                tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + extension);
            }
            while (File.Exists(tempFile));
            return tempFile;
        }

    }

    /// <summary>
    /// EventArgs for the ProgressChanged event.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-05-20</remarks>
    public class ProgressChangedEventArgs : EventArgs
    {
        public bool enableProgressReporting = false;
        public double percentProgress = 0;
    }
}
