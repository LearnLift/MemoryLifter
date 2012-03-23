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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace MLifter.Classes
{
    /// <summary>
    /// Displays the splash form in a different Thread/MessageLoop, and hides it automatically when the program is idle.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-06-26</remarks>
    class SplashManager
    {
        bool hideOnIdle = true;
        bool enableFadingSplash = true;
        bool enableFadingMain = false;

        /// <summary>
        /// Gets or sets a value indicating whether to [auto hide splash on idle].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [auto hide splash on idle]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public bool AutoHideSplashOnIdle
        {
            get { return hideOnIdle; }
            set { hideOnIdle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to [enable fading] of the splash form.
        /// </summary>
        /// <value><c>true</c> if [enable fading]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public bool EnableFadingSplash
        {
            get { return enableFadingSplash; }
            set { enableFadingSplash = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable fading] of the main form.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable fading main form]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public bool EnableFadingMainForm
        {
            get { return enableFadingMain; }
            set { enableFadingMain = value; }
        }

        /// <summary>
        /// Gets or sets the initial message.
        /// </summary>
        /// <value>The initial message.</value>
        /// <remarks>Documented by Dev05, 2009-05-27</remarks>
        public string InitialMessage { get; set; }

        Type splashFormType = null;
        Form splashForm = null;
        Thread splashThread = null;
        Form mainForm = null;

        /// <summary>
        /// Gets the splash form instance (if already created).
        /// </summary>
        /// <value>The splash form.</value>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public Form SplashForm
        {
            get { return splashForm; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashManager"/> class.
        /// </summary>
        /// <param name="SplashForm">The splash form type.</param>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public SplashManager(Type SplashForm)
        {
            InitialMessage = string.Empty;
            splashFormType = SplashForm;
        }

        /// <summary>
        /// Shows the splash form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public void ShowSplash()
        {
            if (splashFormType == null)
                return;

            if (hideOnIdle)
            {
                Application.Idle += new EventHandler(Application_Idle);
                Application.EnterThreadModal += new EventHandler(Application_EnterThreadModal); //this is to detect messagesboxes
            }

            StartSplashThread(splashFormType);
        }

        /// <summary>
        /// Hides the splash form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public void HideSplash()
        {
            CloseSplash();
        }

        /// <summary>
        /// Hides the main form while the splash screen is visible.
        /// </summary>
        /// <param name="mainForm">The main form.</param>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        public void HideMainForm(Form MainForm)
        {
            if (this.splashForm != null) //only hide the mainform when the splashform was not clicked away yet
            {
                this.mainForm = MainForm;
                this.mainForm.Opacity = 0; //hide the MainForm
            }
        }

        /// <summary>
        /// Starts the splash thread.
        /// </summary>
        /// <param name="splashFormClass">The splash form class.</param>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        private void StartSplashThread(Type splashFormClass)
        {
            if (splashThread != null)
                return;

            splashThread = new Thread(delegate()
            {
                DateTime start = DateTime.Now;
                ConstructorInfo constructor = splashFormClass.GetConstructor(new Type[] { });
                splashForm = (Form)constructor.Invoke(new object[] { });
                if (InitialMessage != string.Empty && splashFormClass == typeof(Splash))
                    (splashForm as Splash).SetStatusMessage(InitialMessage);
                splashForm.Click += new EventHandler(splashForm_Click);
                splashForm.Load += new EventHandler(splashForm_Load);
                SplashForm.FormClosed += new FormClosedEventHandler(SplashForm_FormClosed);
                Application.Run(splashForm);
                Debug.WriteLine(string.Format("{0} completed, SplashScreen was visible for {1}.", Thread.CurrentThread.Name, (DateTime.Now - start).ToString()));
            });

            splashThread.IsBackground = true;
            splashThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            splashThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Name = "SplashManager Thread";
            splashThread.Start();
        }

        /// <summary>
        /// Handles the FormClosed event of the SplashForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-27</remarks>
        void SplashForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //make sure that the mainform gets visible as soon as the splashform gets closed
            if (mainForm != null)
                mainForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    mainForm.Opacity = 1;

                    //[ML-551] Main window not in foreground
                    //It is important to focus MainForm once - otherwise, the SplashScreen has focus (because it was here first). 
                    //SplashScreen closed => Windows focused the last used program
                    if (MainForm.ActiveForm != null && MainForm.ActiveForm != splashForm)
                        MainForm.ActiveForm.Activate();
                    else
                        mainForm.Activate();
                }));
        }

        /// <summary>
        /// Handles the Click event of the splashForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-27</remarks>
        void splashForm_Click(object sender, EventArgs e)
        {
            CloseSplash();
        }

        /// <summary>
        /// Handles the Load event of the splashForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        void splashForm_Load(object sender, EventArgs e)
        {
            if (enableFadingSplash)
            {
                splashForm.Opacity = 0;
                splashForm.Show();
                FadeForm(splashForm, 0.05);
            }
        }

        /// <summary>
        /// Closes the splash screen window.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        private void CloseSplash()
        {
            //remove event handlers
            Application.Idle -= new EventHandler(Application_Idle);
            Application.EnterThreadModal -= new EventHandler(Application_EnterThreadModal);

            if (splashThread == null || splashForm == null)
                return;

            //fade in mainForm
            if (mainForm != null)
            {
                mainForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    if (enableFadingMain)
                        FadeForm(mainForm, 0.05);
                    mainForm.Opacity = 1;
                }));
            }

            //fade out splashform
            splashForm.BeginInvoke(new MethodInvoker(delegate()
            {
                if (enableFadingSplash)
                    FadeForm(splashForm, -0.10);
                splashForm.Close();
                splashForm = null;
                splashThread = null;
            }));
        }

        /// <summary>
        /// Handles the Idle event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-26</remarks>
        void Application_Idle(object sender, EventArgs e)
        {
            if (hideOnIdle)
                CloseSplash();
        }

        /// <summary>
        /// Handles the EnterThreadModal event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-27</remarks>
        void Application_EnterThreadModal(object sender, EventArgs e)
        {
            if (hideOnIdle)
                CloseSplash();
        }

        /// <summary>
        /// Fades the form with the specified delta value.
        /// </summary>
        /// <param name="delta">The delta (in percent). Positive for fade in, negative for fade out.</param>
        /// <remarks>Documented by Dev02, 2008-01-22</remarks>
        private void FadeForm(Form form, double delta)
        {
            if (form == null || delta == 0)
                return;

            form.Refresh(); //ensure that the form is fully painted before beginning to fade

            for (double opacity = 0; Math.Abs(opacity) < 1; opacity += Math.Abs(delta))
            {
                form.Opacity = delta > 0 ? opacity : 1 - opacity;
                System.Threading.Thread.Sleep(10);
            }

            form.Opacity = delta > 0 ? 1 : 0;
        }
    }
}
