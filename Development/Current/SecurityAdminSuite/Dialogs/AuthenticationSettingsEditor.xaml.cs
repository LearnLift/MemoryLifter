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

namespace SecurityAdminSuite.Dialogs
{
    /// <summary>
    /// Interaction logic for AuthenticationSettingsEditor.xaml
    /// </summary>
    public partial class AuthenticationSettingsEditor : UserControl
    {
        public enum LdapMode
        {
            None,
            ActiveDirectory,
            eDirectory
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationSettingsEditor"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        public AuthenticationSettingsEditor()
        {
            InitializeComponent();
            this.DataContext = this;

            DependencyPropertyDescriptor descriptorSelectedLdap = DependencyPropertyDescriptor.FromProperty(SelectedLdapModeProperty, typeof(AuthenticationSettingsEditor));
            if (descriptorSelectedLdap != null)
            {
                descriptorSelectedLdap.AddValueChanged(this, delegate
                {
                    Facade.SecurityFramework.DataAdapter.DatabaseInformations.LocalDirectoryAuthentication = SelectedLdapMode != LdapMode.None;
                    expanderLdapSettings.IsExpanded = SelectedLdapMode != LdapMode.None;
                    if (SelectedLdapMode != LdapMode.None)
                        Facade.SecurityFramework.DataAdapter.DatabaseInformations.LocalDirectoryType = SelectedLdapMode.ToString();
                });
            }

            DependencyPropertyDescriptor descriptorFacade = DependencyPropertyDescriptor.FromProperty(FacadeProperty, typeof(AuthenticationSettingsEditor));
            if (descriptorFacade != null)
            {
                descriptorFacade.AddValueChanged(this, delegate
                {
                    SelectedLdapMode = Facade.SecurityFramework.DataAdapter.DatabaseInformations.LocalDirectoryAuthentication ?
                        (LdapMode)Enum.Parse(typeof(LdapMode), Facade.SecurityFramework.DataAdapter.DatabaseInformations.LocalDirectoryType) : LdapMode.None;
                });
            }
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
        public static readonly DependencyProperty FacadeProperty = DependencyProperty.Register("Facade", typeof(Facade), typeof(AuthenticationSettingsEditor));

        /// <summary>
        /// Gets the LDAP modes.
        /// </summary>
        /// <value>The LDAP modes.</value>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        public List<LdapMode> LdapModes
        {
            get { return new List<LdapMode>(Enum.GetValues(typeof(LdapMode)).OfType<LdapMode>()); }
        }

        /// <summary>
        /// Gets or sets the selected LDAP mode.
        /// </summary>
        /// <value>The selected LDAP mode.</value>
        /// <remarks>Documented by Dev02, 2009-06-19</remarks>
        public LdapMode SelectedLdapMode
        {
            get { return (LdapMode)GetValue(SelectedLdapModeProperty); }
            set { SetValue(SelectedLdapModeProperty, value); }
        }

        public static readonly DependencyProperty SelectedLdapModeProperty =
            DependencyProperty.Register("SelectedLdapMode", typeof(LdapMode), typeof(AuthenticationSettingsEditor));

    }
}
