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
using MLifter.DAL.Interfaces;
using MLifter.BusinessLayer;
using MLifter.DAL.Preview;
using MLifter.Components;

namespace MLifterSettingsManager
{
    /// <summary>
    /// Interaction logic for PreviewCardControl.xaml
    /// </summary>
    public partial class PreviewCardControl : UserControl
    {
        #region Private Members

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the card.
        /// </summary>
        /// <value>The card.</value>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        public ICard Card
        {
            get { return (ICard)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register("Card", typeof(ICard), typeof(PreviewCardControl));

        /// <summary>
        /// Gets or sets the learning module.
        /// </summary>
        /// <value>The learning module.</value>
        /// <remarks>Documented by Dev08, 2009-07-20</remarks>
        public IDictionary LearningModule
        {
            get { return (IDictionary)GetValue(LearningModuleProperty); }
            set { SetValue(LearningModuleProperty, value); }
        }

        public static readonly DependencyProperty LearningModuleProperty = DependencyProperty.Register("LearningModule", typeof(IDictionary), typeof(PreviewCardControl));

        #endregion

        public PreviewCardControl()
        {
            InitializeComponent();
        }
    }
}
