using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MLifter.BusinessLayer;
using System.Runtime.Remoting.Lifetime;

namespace MLifter.Classes
{
    /// <summary>
    /// Class to remotly load a dictionary.
    /// </summary>
    /// <remarks>Documented by Dev05, 2007-11-27</remarks>
    public class GlobalDictionaryLoader : MarshalByRefObject
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Determines whether [is ML ready].
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        public bool IsMLReady()
        {
            if (MLifter.Program.MainForm == null)
                return false;

            return true;
        }

        /// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks>Documented by Dev05, 2007-11-27</remarks>
        public void LoadDictionary(string path)
        {
            if (MLifter.Program.MainForm.IsHandleCreated && !MLifter.Program.SuspendIPC)
            {
                BringToFront();
                MLifter.Program.MainForm.Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            //[ML-1126] Do not allow to open dictionaries when an edit form is still open
                            MLifter.Program.MainForm.CloseLearningModulesPage();
                            if (!MLifter.Program.MainForm.OtherFormsOpen)
                            {
                                if (MLifter.DAL.Helper.IsLearningModuleFileName(path))
                                    MLifter.Program.MainForm.DoDragAndDrop(path);
                                else
                                    MLifter.Program.MainForm.OpenConfigFile(path);
                            }
                        }
                        catch (Exception exp)
                        {
                            if (exp is FormsOpenException)
                                throw;

                            RemotingException(exp);
                        }
                    });
                BringToFront();
            }
            else
            {
                MLifter.Program.MainForm.MainformLoaded += new EventHandler(delegate
                {
                    LoadDictionary(path);
                });
            }
        }

        /// <summary>
        /// Loads the dictionary.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        public void LoadDictionary(LearningModulesIndexEntry entry)
        {
            if (MLifter.Program.MainForm.IsHandleCreated && !MLifter.Program.SuspendIPC)
            {
                MLifter.Program.MainForm.Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            //[ML-1126] Do not allow to open dictionaries when an edit form is still open
                            MLifter.Program.MainForm.CloseLearningModulesPage();
                            if (!MLifter.Program.MainForm.OtherFormsOpen)
                                MLifter.Program.MainForm.OpenEntry(entry);
                            else
                                throw new FormsOpenException();
                        }
                        catch (Exception exp)
                        {
                            if (exp is FormsOpenException)
                                throw;

                            RemotingException(exp);
                        }
                    });
            }
            else
            {
                MLifter.Program.MainForm.MainformLoaded += new EventHandler(delegate
                {
                    LoadDictionary(entry);
                });
            }
        }

        /// <summary>
        /// Selects the learning chapters.
        /// </summary>
        /// <param name="chapterIds">The chapter ids.</param>
        /// <remarks>Documented by Dev02, 2009-06-26</remarks>
        public void SelectLearningChapters(int[] chapterIds)
        {
            if (MLifter.Program.MainForm.IsHandleCreated && !MLifter.Program.SuspendIPC)
            {
                MLifter.Program.MainForm.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        MLifter.Program.MainForm.CloseLearningModulesPage();
                        if (!MLifter.Program.MainForm.OtherFormsOpen)
                            MLifter.Program.MainForm.SelectLearningChapters(chapterIds);
                        else
                            throw new FormsOpenException();
                    }
                    catch (Exception exp)
                    {
                        if (exp is FormsOpenException)
                            throw;

                        RemotingException(exp);
                    }
                });
            }
            else
            {
                MLifter.Program.MainForm.MainformLoaded += new EventHandler(delegate
                {
                    SelectLearningChapters(chapterIds);
                });
            }
        }

        /// <summary>
        /// Brings the MLifter to front.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-28</remarks>
        public void BringToFront()
        {
            MLifter.Program.MainForm.Invoke((MethodInvoker)delegate
                {
                    if (MLifter.Program.MainForm.WindowState == FormWindowState.Minimized)
                        MLifter.Program.MainForm.WindowState = FormWindowState.Normal;

                    if (MainForm.LearnLogic.SnoozeModeActive)
                        MainForm.LearnLogic.SnoozeModeActive = false;

                    SetForegroundWindow(MLifter.Program.MainForm.Handle);
                    MLifter.Program.MainForm.Activate();
                });
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">
        /// The immediate caller does not have infrastructure permission.
        /// </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public override object InitializeLifetimeService()
        {
            //is important for the clients to close the channel fast
            ILease lease = (ILease)base.InitializeLifetimeService();

            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.FromSeconds(1);
                lease.SponsorshipTimeout = TimeSpan.FromSeconds(1);
                lease.RenewOnCallTime = TimeSpan.FromSeconds(1);
            }
            return lease;

        }

        /// <summary>
        /// Reports a remoting exception.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <remarks>Documented by Dev02, 2009-07-14</remarks>
        private void RemotingException(Exception exp)
        {
            ErrorReportGenerator.ReportError(exp, false);
        }
    }

    /// <summary>
    /// Occurs when settings or the learning module could not be changed because MemoryLifter has open configuration forms.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-06-26</remarks>
    [global::System.Serializable]
    public class FormsOpenException : Exception
    {
        public FormsOpenException() { }
        public FormsOpenException(string message) : base(message) { }
        public FormsOpenException(string message, Exception inner) : base(message, inner) { }
        protected FormsOpenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
