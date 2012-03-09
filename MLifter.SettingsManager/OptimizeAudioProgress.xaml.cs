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
using System.Windows.Shapes;

namespace MLifterSettingsManager
{
    /// <summary>
    /// Interaction logic for OptimizeAudioProgress.xaml
    /// </summary>
    public partial class OptimizeAudioProgress : Window
    {
        public OptimizeAudioProgress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the current progress.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="progress">The progress.</param>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        public void ProgressCallback(string status, double progress)
        {
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                this.Dispatcher.Invoke((Action)delegate() { ProgressCallback(status, progress); });
            else
            {
                listBox.Items.Insert(0, status);
                if (progress >= 0)
                    progressBar.Value = progress * 100;
            }
        }

        /// <summary>
        /// Manually closes a <see cref="T:System.Windows.Window"/>.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-04</remarks>
        public new void Close()
        {
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                this.Dispatcher.Invoke((Action)delegate() { base.Close(); });
            else
                base.Close();
        }
    }
}
