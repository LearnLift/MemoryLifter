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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MLifter.Generics;
using System.Data.SqlServerCe;
using MLifter.DAL.DB.MsSqlCe;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Diagnostics;
using MLifter.BusinessLayer;
using Microsoft.Win32;
using System.Windows.Forms;
using MLifterSettingsManager.DAL;
using System.Threading;

namespace MLifterSettingsManager
{
    /// <summary>
    /// Interaction logic for PublisherForm.xaml
    /// </summary>
    public partial class PublisherConverterForm : Window
    {
        private string[] learningModules;
        private string sourceFolderPath = string.Empty;
        private string destinationFolderPath = string.Empty;
        private UnPacker unpacker = new UnPacker();
        private ConverterFromOdx convert = new ConverterFromOdx();
        private bool convertDzpFiles = true;
        private string fileExtension;
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherForm"/> class.
        /// </summary>
        /// <param name="learningModulePath">The learning module path.</param>
        /// <remarks>Documented by Dev05, 2009-05-22</remarks>
        public PublisherConverterForm()
        {
            InitializeComponent();

            unpacker.SetStatusDelegate = log;
            unpacker.UnpackingFinished += new EventHandler(unpacker_UnpackingFinished);

            convert.SetStatusDelegate = log;
            convert.ConvertingFinished += new EventHandler<ConverterFromOdx.ConvertingEventArgs>(convert_ConvertingFinished);
        }

        private void buttonBrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            textBoxDestinationFolder.Text = getFilePath();
            if (textBoxDestinationFolder.Text != string.Empty && textBoxSourceFolder.Text != string.Empty)
                buttonPublish.IsEnabled = true;
        }

        private void buttonBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            textBoxSourceFolder.Text = getFilePath();
            if (textBoxDestinationFolder.Text != string.Empty && textBoxSourceFolder.Text != string.Empty)
                buttonPublish.IsEnabled = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonPublish_Click(object sender, RoutedEventArgs e)
        {
            enableControls(false);

            currentLM = -1;
            learningModules = null;

            Thread batchJob = new Thread(BatchJobStart);
            batchJob.Name = "BatchJob Thread";
            batchJob.IsBackground = false;
            batchJob.Priority = ThreadPriority.Lowest;
            batchJob.Start();
        }
        private void enableControls(bool value)
        {
            textBoxDestinationFolder.IsEnabled = value;
            textBoxSourceFolder.IsEnabled = value;

            buttonBrowseDestination.IsEnabled = value;
            buttonBrowseSource.IsEnabled = value;

        }
        private void BatchJobStart()
        {

            this.Dispatcher.Invoke(new MethodInvoker(delegate()
            {
                convertDzpFiles = radioButtonSourceIsDzp.IsChecked.Value ? true : false;
                fileExtension = radioButtonSourceIsDzp.IsChecked.Value ? "*.dzp" : "*.mlm";

                sourceFolderPath = textBoxSourceFolder.Text;
                destinationFolderPath = textBoxDestinationFolder.Text;
            }));
            log("reading source folder");


            learningModules = Directory.GetFiles(sourceFolderPath, fileExtension, SearchOption.AllDirectories);
            log(learningModules.Length + " files found");

            log("start converting and publishing");

            if (convertDzpFiles)
                UnpackConvertLM();
            else
                StartToPublishLMs();
        }
        private int currentLM = -1;
        private void StartToPublishLMs()
        {
            currentLM++;
            if (currentLM == learningModules.Length ){
                log("done: converted all files in source folder");
                this.Dispatcher.Invoke(new MethodInvoker(delegate()
                {
                    enableControls(true);
                }));
                //done;
                return;
            }

            string file = learningModules[currentLM];
            string fileName = System.IO.Path.GetFileName(file);
            string fileDestination = System.IO.Path.Combine(destinationFolderPath, fileName);
            try
            {
                //File.Copy(file, fileDestination);
                Publish(file); 
            }
            catch (Exception e)
            {
                log("Could not write file to destination path" + learningModules[currentLM] + " " + e.Message.ToString());
            }
        }
        private void UnpackConvertLM()
        {
            currentLM++;
            if (currentLM == learningModules.Length){
                log("done: converted all files in source folder");
                this.Dispatcher.Invoke(new MethodInvoker(delegate()
                {
                    enableControls(true);
                }));
                //done
                return;
            }
            Unpack(learningModules[currentLM], destinationFolderPath);

        }
        private void Unpack(string lm, string destination)
        {
            log(lm + ": start converting");
            unpacker.Start(lm, destination);
        }
        void unpacker_UnpackingFinished(object sender, EventArgs e)
        {
            Convert(unpacker.Dictionary, false);
        }
        private void Convert(string lm, bool cleanUp)
        {
            convert.Start(lm, cleanUp);
        }
        void convert_ConvertingFinished(object sender, ConverterFromOdx.ConvertingEventArgs e)
        {
            Publish(e.ConvertedFile);
        }
        private void Publish(string source)
        {
            log(source + ": start publishing");

            if (!Publisher.Publish(source, System.IO.Path.Combine(destinationFolderPath, System.IO.Path.GetFileName(source))))
                throw new Exception("Error publishing " + source + "!");

            log("published...");

            if (currentLM >= learningModules.Length)
            {
                this.Dispatcher.Invoke((Action)delegate { IsEnabled = true; });
                return;
            }

            if (convertDzpFiles)
                UnpackConvertLM();
            else
                StartToPublishLMs();
        }

        private void log(string logMessage)
        {
            log(logMessage, false);
        }
        private void log(string logMessage, bool update)
        {
            this.Dispatcher.Invoke(new MethodInvoker(delegate()
            {
                if (update)
                    listBoxLog.Items[0] = DateTime.Now + ": " + logMessage;
                else
                    listBoxLog.Items.Insert(0, DateTime.Now + ": " + logMessage);
            }));

        }
        private string getFilePath()
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.Description = "Choose folder";
            ofd.SelectedPath = @"C:\";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.SelectedPath;
            }

            return string.Empty;
        }
    }
}
