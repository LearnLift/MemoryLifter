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
