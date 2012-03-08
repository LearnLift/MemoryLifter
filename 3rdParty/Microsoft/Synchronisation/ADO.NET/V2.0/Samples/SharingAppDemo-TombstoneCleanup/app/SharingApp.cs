// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using Microsoft.Synchronization.Data;


namespace SyncApplication
{
    static class OfflineApp
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SyncTracer.Info("Starting Application");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SyncForm());              
        }
    }
}