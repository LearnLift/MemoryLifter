using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MLifter.Controls;

namespace SecurityAdminSuite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                System.Windows.Forms.Application.EnableVisualStyles();

                EventManager.RegisterClassHandler(typeof(TextBox),
                    TextBox.GotFocusEvent,
                    new RoutedEventHandler(TextBox_GotFocus));

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                TaskDialog.MessageBox("Error during startup", "An error occured while initializing the AdminSuite ", string.Empty, ex.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                System.Windows.Forms.Application.EnableVisualStyles();

                Uri uri = new Uri("PresentationFramework.Aero;V3.0.0.0;31bf3856ad364e35;component\\themes/aero.normalcolor.xaml", UriKind.Relative);

                Resources.MergedDictionaries.Add(Application.LoadComponent(uri) as ResourceDictionary);
            }
            catch { }
        }
    }
}
