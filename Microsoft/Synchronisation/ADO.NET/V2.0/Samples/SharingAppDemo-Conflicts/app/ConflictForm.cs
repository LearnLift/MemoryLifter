// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Synchronization.Data;


namespace SyncApplication
{
    public partial class ConflictForm : Form
    {        
        DbApplyChangeFailedEventArgs _conflictArgs;       

        public ConflictForm()
        {
            InitializeComponent();
        }

        public void HandleConflict(string fromPeer, string toPeer, DbApplyChangeFailedEventArgs args)
        {                        
            _conflictArgs = args;

            this.Text = "Conflict Detected on " + toPeer;
            richTextBoxHelp.Text = "Conflict Resolution Guide: \n" +
                "Continue: Ignore the remote change [local change wins] \n " +
                "Force Write: Overwrite existing row [remote change wins]. For this option to work the sync commands need to use @sync_force_write parameter \n" +
                "Retry Next Sync: Record the conflict in the sync metadata and fetch it again in the next sync \n" +
                "Retry: Disabled, only used when custom code is written to make changes to the database to fix the conflict (i.e. constraint conflict) \n";
            buttonRetry.Enabled = false;                                        

            textBoxSyncStage.Text = _conflictArgs.Conflict.Stage.ToString();
            textBoxConflictType.Text = _conflictArgs.Conflict.Type.ToString();                        
            textBoxError.Text = _conflictArgs.Conflict.ErrorMessage;

            groupBoxRemoteChange.Text = "Remote Change from " + fromPeer;
            groupBoxLocalChange.Text = "Local Change on " + toPeer;

            if (_conflictArgs.Conflict.RemoteChange != null)
            {                
                dataGridServerChange.DataSource = _conflictArgs.Conflict.RemoteChange;
            }

            if (_conflictArgs.Conflict.LocalChange != null)
            {                
                dataGridClientChange.DataSource = _conflictArgs.Conflict.LocalChange;
            }

            Application.DoEvents();
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            _conflictArgs.Action = ApplyAction.RetryApplyingRow;
            Close();
        }

        private void buttonForceWrite_Click(object sender, EventArgs e)
        {
            _conflictArgs.Action = ApplyAction.RetryWithForceWrite;
            Close();
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            _conflictArgs.Action = ApplyAction.Continue;
            Close();
        }

        private void buttonRetryNextSync_Click(object sender, EventArgs e)
        {
            _conflictArgs.Action = ApplyAction.RetryNextSync;
            Close();
        }
    }
}
