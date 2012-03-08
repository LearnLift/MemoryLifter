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
