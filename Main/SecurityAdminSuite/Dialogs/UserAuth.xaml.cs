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
using SecurityFramework;

namespace SecurityAdminSuite
{
    /// <summary>
    /// Interaction logic for EditStringWindow.xaml
    /// </summary>
    public partial class UserAuth
    {

        #region Property
        private User _User { get; set; }
        private UserAuthType _UserAuthType { get; set; }
        #endregion Property

        #region Constructors
        public UserAuth(User u)
        {
            InitializeComponent();
            this._User = u;

            if (_User.Type == UserAuthType.FormsAuthentication)
                AuthForm();
            else if (_User.Type == UserAuthType.ListAuthentication)
                AuthList();
            else if (_User.Type == UserAuthType.LocalDirectoryAuthentication)
                AuthActiveDirectory();

            DataContext = this;
            tbString.Focus();
        }

        #endregion Constructors


        #region Properties (2)

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
            _User.Type = _UserAuthType;
            DialogResult = true;
        }

        #endregion Event Handlers

        private void AuthForm()
        {
            radioButtonFormUser.IsChecked = true;
            PasswordRequired();

            radioButtonActiveDirectoryUser.IsEnabled = false;
        }
        private void AuthList()
        {
            radioButtonListUser.IsChecked = true;
            NoPasswordRequired();

            radioButtonActiveDirectoryUser.IsEnabled = false;
        }
        private void AuthActiveDirectory()
        {
            radioButtonActiveDirectoryUser.IsChecked = true;
            NoPasswordRequired();

            radioButtonListUser.IsEnabled = false;
            radioButtonFormUser.IsEnabled = false;
        }

        private void radioButtonListUser_Click(object sender, RoutedEventArgs e)
        {
            NoPasswordRequired();
            _UserAuthType = UserAuthType.ListAuthentication;
        }

        private void radioButtonFormUser_Click(object sender, RoutedEventArgs e)
        {
            PasswordRequired();
            _UserAuthType = UserAuthType.FormsAuthentication;
        }

        private void radioButtonActiveDirectoryUser_Click(object sender, RoutedEventArgs e)
        {
            NoPasswordRequired();
            _UserAuthType = UserAuthType.LocalDirectoryAuthentication;
        }

        private void NoPasswordRequired()
        {
            tbString.IsEnabled = false;
            tbString.Text = "No Password Required";
        }
        private void PasswordRequired()
        {
            tbString.IsEnabled = true;
            tbString.Text = string.Empty;
        }
    }
}
