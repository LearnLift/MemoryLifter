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

namespace SecurityAdminSuite
{
    /// <summary>
    /// Interaction logic for EditStringWindow.xaml
    /// </summary>
    public partial class EditStringWindow : Window
    {

        #region Constructors (1)

        public EditStringWindow()
        {
            InitializeComponent();
            DataContext = this;
            tbString.Focus();
        }

        #endregion Constructors

        #region Properties (2)


        public string Header
        {
            get
            {
                return tbHeader.Text;
            }
            set
            {
                tbHeader.Text = value;
            }
        }
        public string TextItem
        {
            get
            {
                return tbString.Text;
            }
            set
            {
                tbString.Text = value;
                tbString.SelectAll();
            }
        }


        #endregion Properties

        #region Event Handlers (1)

        void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextItem))
            {
                MessageBox.Show("Text must not be empty!");
                return;
            }
            DialogResult = true;
        }

        #endregion Event Handlers

    }
}
