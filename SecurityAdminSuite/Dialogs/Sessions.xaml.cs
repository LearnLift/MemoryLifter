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
using System.ComponentModel;
using SecurityFramework;
using System.Collections.ObjectModel;

namespace SecurityAdminSuite.Dialogs
{
    /// <summary>
    /// Interaction logic for Sessions.xaml
    /// </summary>
    public partial class Sessions : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sessions"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        public Sessions()
        {
            InitializeComponent();
            this.DataContext = this;

            DependencyPropertyDescriptor descriptorFacade = DependencyPropertyDescriptor.FromProperty(FacadeProperty, typeof(Sessions));
            if (descriptorFacade != null)
            {
                descriptorFacade.AddValueChanged(this, delegate
                {
                    UpdateOpenSessions();
                });
            }

            OpenSessions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OpenSessions_CollectionChanged);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the OpenSessions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        void OpenSessions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            buttonEndAllSessions.IsEnabled = OpenSessions.Count > 0;
        }

        /// <summary>
        /// Gets or sets the facade.
        /// </summary>
        /// <value>The facade.</value>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        public Facade Facade
        {
            get { return (Facade)GetValue(FacadeProperty); }
            set { SetValue(FacadeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Facade.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FacadeProperty =
            DependencyProperty.Register("Facade", typeof(Facade), typeof(Sessions));

        ObservableCollection<ISession> openSessions = new ObservableCollection<ISession>();

        /// <summary>
        /// Gets the open sessions.
        /// </summary>
        /// <value>The open sessions.</value>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        public ObservableCollection<ISession> OpenSessions
        {
            get { return openSessions; }
        }

        /// <summary>
        /// Updates the open sessions.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        private void UpdateOpenSessions()
        {
            openSessions.Clear();
            Facade.SecurityFramework.DataAdapter.GetOpenSessions().ForEach(s => openSessions.Add(s));
        }

        /// <summary>
        /// Handles the Click event of the buttonEndAllSessions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        private void buttonEndAllSessions_Click(object sender, RoutedEventArgs e)
        {
            EndAllSessions();
        }

        /// <summary>
        /// Handles the Click event of the buttonRefreshList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        private void buttonRefreshList_Click(object sender, RoutedEventArgs e)
        {
            UpdateOpenSessions();
        }

        /// <summary>
        /// Ends all sessions.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        private void EndAllSessions()
        {
            if (OpenSessions.Count < 1)
                return;

            if (MessageBox.Show(String.Format("Are you sure to log out {0} user(s)?", OpenSessions.Count), "End All Sessions",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                Facade.SecurityFramework.DataAdapter.CloseAllSessions();
                UpdateOpenSessions();
            }
        }
    }
}
