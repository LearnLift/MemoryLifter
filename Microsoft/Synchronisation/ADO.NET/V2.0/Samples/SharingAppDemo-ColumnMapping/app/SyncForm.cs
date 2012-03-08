// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.SqlClient;

using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;


namespace SyncApplication
{
    public partial class SyncForm : Form
    {
        ProgressForm _progressForm;
        string fromPeer = string.Empty, toPeer = string.Empty;

        public SyncForm()
        {
            InitializeComponent();                                    

            if (string.IsNullOrEmpty(textPeer1Machine.Text))
            {
                textPeer1Machine.Text = Environment.MachineName;
            }

            if (string.IsNullOrEmpty(textPeer2Machine.Text))
            {
                textPeer2Machine.Text = Environment.MachineName;
            }

            if (string.IsNullOrEmpty(textPeer3Machine.Text))
            {
                textPeer3Machine.Text = Environment.MachineName;
            }

            _progressForm = null;
        }        


        DbSyncProvider SetupSyncProvider(string connectionString, DbSyncProvider peerProvider)
        {

            const int TombstoneAgingInHours = 100; 
            
            SqlConnection connection = new SqlConnection(connectionString);            
            peerProvider.Connection = connection;            

            //
            // 1. Create sync adapter for each sync table and attach it to the provider
            // Following DataAdapter style in ADO.NET, DbSyncAdapter is the equivelent for sync.
            // The code below shows how to create DbSyncAdapter objects for orders 
            // and order_details tables using stored procedures stored on the database.                         
            //

            peerProvider.ScopeName = "Sales"; 
            
            
            string ordersTableName = "orders";
            if (connection.Database == "peer1")
            {
                ordersTableName = "ordertable";
            }
            
            // orders table
            DbSyncAdapter adaptorOrders = null;

            if (connection.Database == "peer1")
            {
                adaptorOrders = new DbSyncAdapter(ordersTableName, "orders");
                DbSyncColumnMappingCollection colMap = adaptorOrders.ColumnMappings;
                colMap.Add("orderid", "order_id");
                colMap.Add("orderdate", "order_date");
                adaptorOrders.RowIdColumns.Add("orderid");
            }
            else
            {
                adaptorOrders = new DbSyncAdapter(ordersTableName);
                adaptorOrders.RowIdColumns.Add("order_id");
            }

            // select incremental changes command
            SqlCommand chgsOrdersCmd = new SqlCommand();
            chgsOrdersCmd.CommandType = CommandType.StoredProcedure;
            chgsOrdersCmd.CommandText = "sp_" + ordersTableName + "_selectchanges";
            chgsOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncMetadataOnly, SqlDbType.Int);
            chgsOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);            

            adaptorOrders.SelectIncrementalChangesCommand = chgsOrdersCmd;

            string orderidCol = null;
            string orderdateCol = null;

            if (connection.Database == "peer1")
            {
                orderidCol = "@orderid";
                orderdateCol = "@orderdate";
            }
            else
            {
                orderidCol = "@order_id";
                orderdateCol = "@order_date";
            }

            // insert row command
            SqlCommand insOrdersCmd = new SqlCommand();
            insOrdersCmd.CommandType = CommandType.StoredProcedure;
            insOrdersCmd.CommandText = "sp_" + ordersTableName + "_applyinsert";
            insOrdersCmd.Parameters.Add(orderidCol, SqlDbType.Int);
            insOrdersCmd.Parameters.Add(orderdateCol, SqlDbType.DateTime);
            insOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrders.InsertCommand = insOrdersCmd;


            // update row command
            SqlCommand updOrdersCmd = new SqlCommand();
            updOrdersCmd.CommandType = CommandType.StoredProcedure;
            updOrdersCmd.CommandText = "sp_" + ordersTableName + "_applyupdate";
            updOrdersCmd.Parameters.Add(orderidCol, SqlDbType.Int);
            updOrdersCmd.Parameters.Add(orderdateCol, SqlDbType.DateTime);
            updOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);
            updOrdersCmd.Parameters.Add("@sync_force_write", SqlDbType.Int);
            updOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrders.UpdateCommand = updOrdersCmd;


            // delete row command
            SqlCommand delOrdersCmd = new SqlCommand();
            delOrdersCmd.CommandType = CommandType.StoredProcedure;
            delOrdersCmd.CommandText = "sp_" + ordersTableName + "_applydelete";
            delOrdersCmd.Parameters.Add(orderidCol, SqlDbType.Int);
            delOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);
            delOrdersCmd.Parameters.Add("@sync_force_write", SqlDbType.Int);
            delOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrders.DeleteCommand = delOrdersCmd;

            // get row command
            SqlCommand selRowOrdersCmd = new SqlCommand();
            selRowOrdersCmd.CommandType = CommandType.StoredProcedure;
            selRowOrdersCmd.CommandText = "sp_" + ordersTableName + "_selectrow";
            selRowOrdersCmd.Parameters.Add(orderidCol, SqlDbType.Int);

            adaptorOrders.SelectRowCommand = selRowOrdersCmd;


            // insert row metadata command
            SqlCommand insMetadataOrdersCmd = new SqlCommand();
            insMetadataOrdersCmd.CommandType = CommandType.StoredProcedure;
            insMetadataOrdersCmd.CommandText = "sp_" + ordersTableName + "_insertmetadata";
            insMetadataOrdersCmd.Parameters.Add(orderidCol, SqlDbType.Int);
            insMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerKey, SqlDbType.Int);
            insMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerTimestamp, SqlDbType.BigInt);
            insMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerKey, SqlDbType.Int);
            insMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerTimestamp, SqlDbType.BigInt);
            insMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowIsTombstone, SqlDbType.Int);                        
            insMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrders.InsertMetadataCommand = insMetadataOrdersCmd;


            // update row metadata command
            SqlCommand updMetadataOrdersCmd = new SqlCommand();
            updMetadataOrdersCmd.CommandType = CommandType.StoredProcedure;
            updMetadataOrdersCmd.CommandText = "sp_" + ordersTableName + "_updatemetadata";
            updMetadataOrdersCmd.Parameters.Add(orderidCol, SqlDbType.Int);
            updMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerKey, SqlDbType.Int);
            updMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerTimestamp, SqlDbType.BigInt);
            updMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerKey, SqlDbType.Int);
            updMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerTimestamp, SqlDbType.BigInt);
            updMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncCheckConcurrency, SqlDbType.Int);
            updMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowTimestamp, SqlDbType.BigInt);
            updMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrders.UpdateMetadataCommand = updMetadataOrdersCmd;

            // delete row metadata command
            SqlCommand delMetadataOrdersCmd = new SqlCommand();
            delMetadataOrdersCmd.CommandType = CommandType.StoredProcedure;
            delMetadataOrdersCmd.CommandText = "sp_" + ordersTableName + "_deletemetadata";
            delMetadataOrdersCmd.Parameters.Add(orderidCol, SqlDbType.Int);
            delMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncCheckConcurrency, SqlDbType.Int);
            delMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowTimestamp, SqlDbType.BigInt);
            delMetadataOrdersCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrders.DeleteMetadataCommand = delMetadataOrdersCmd;


            // get tombstones for cleanup
            SqlCommand selTombstonesOrdersCmd = new SqlCommand();
            selTombstonesOrdersCmd.CommandType = CommandType.StoredProcedure;
            selTombstonesOrdersCmd.CommandText = "sp_" + ordersTableName + "_selecttombstones";
            selTombstonesOrdersCmd.Parameters.Add("@tombstone_aging_in_hours", SqlDbType.Int).Value = TombstoneAgingInHours;

            adaptorOrders.SelectMetadataForCleanupCommand = selTombstonesOrdersCmd;

            peerProvider.SyncAdapters.Add(adaptorOrders);


            // order_details table
            DbSyncAdapter adaptorOrderDetails = new DbSyncAdapter("order_details");
            adaptorOrderDetails.RowIdColumns.Add("order_id");

            // select incremental inserts command
            SqlCommand chgsOrderDetailsCmd = new SqlCommand();
            chgsOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            chgsOrderDetailsCmd.CommandText = "sp_order_details_selectchanges";            
            chgsOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncMetadataOnly, SqlDbType.Int);
            chgsOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);            

            adaptorOrderDetails.SelectIncrementalChangesCommand = chgsOrderDetailsCmd;


            // insert row command
            SqlCommand insOrderDetailsCmd = new SqlCommand();
            insOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            insOrderDetailsCmd.CommandText = "sp_order_details_applyinsert";
            insOrderDetailsCmd.Parameters.Add("@order_id", SqlDbType.Int);
            insOrderDetailsCmd.Parameters.Add("@order_details_id", SqlDbType.Int);
            insOrderDetailsCmd.Parameters.Add("@product", SqlDbType.VarChar, 100);
            insOrderDetailsCmd.Parameters.Add("@quantity", SqlDbType.Int);
            insOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrderDetails.InsertCommand = insOrderDetailsCmd;


            // update row command
            SqlCommand updOrderDetailsCmd = new SqlCommand();
            updOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            updOrderDetailsCmd.CommandText = "sp_order_details_applyupdate";
            updOrderDetailsCmd.Parameters.Add("@order_id", SqlDbType.Int);
            updOrderDetailsCmd.Parameters.Add("@order_details_id", SqlDbType.Int);
            updOrderDetailsCmd.Parameters.Add("@product", SqlDbType.VarChar, 100);
            updOrderDetailsCmd.Parameters.Add("@quantity", SqlDbType.Int);
            updOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);            
            updOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrderDetails.UpdateCommand = updOrderDetailsCmd;


            // delete row command
            SqlCommand delOrderDetailsCmd = new SqlCommand();
            delOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            delOrderDetailsCmd.CommandText = "sp_order_details_applydelete";
            delOrderDetailsCmd.Parameters.Add("@order_id", SqlDbType.Int);
            delOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncMinTimestamp, SqlDbType.BigInt);            
            delOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrderDetails.DeleteCommand = delOrderDetailsCmd;

            // get row command
            SqlCommand selRowOrderDetailsCmd = new SqlCommand();
            selRowOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            selRowOrderDetailsCmd.CommandText = "sp_order_details_selectrow";
            selRowOrderDetailsCmd.Parameters.Add("@order_id", SqlDbType.Int);            

            adaptorOrderDetails.SelectRowCommand = selRowOrderDetailsCmd;


            // insert row metadata command
            SqlCommand insMetadataOrderDetailsCmd = new SqlCommand();
            insMetadataOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            insMetadataOrderDetailsCmd.CommandText = "sp_order_details_insertmetadata";
            insMetadataOrderDetailsCmd.Parameters.Add("@order_id", SqlDbType.Int);   
            insMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerKey, SqlDbType.Int);
            insMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerTimestamp, SqlDbType.BigInt);
            insMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerKey, SqlDbType.Int);
            insMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerTimestamp, SqlDbType.BigInt);
            insMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowIsTombstone, SqlDbType.Int);            
            insMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrderDetails.InsertMetadataCommand = insMetadataOrderDetailsCmd;


            // update row metadata command
            SqlCommand updMetadataOrderDetailsCmd = new SqlCommand();
            updMetadataOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            updMetadataOrderDetailsCmd.CommandText = "sp_order_details_updatemetadata";
            updMetadataOrderDetailsCmd.Parameters.Add("@order_id", SqlDbType.Int);   
            updMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerKey, SqlDbType.Int);
            updMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncCreatePeerTimestamp, SqlDbType.BigInt);
            updMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerKey, SqlDbType.Int);
            updMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncUpdatePeerTimestamp, SqlDbType.BigInt);
            updMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncCheckConcurrency, SqlDbType.Int);
            updMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowTimestamp, SqlDbType.BigInt);
            updMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrderDetails.UpdateMetadataCommand = updMetadataOrderDetailsCmd;

            // delete row metadata command
            SqlCommand delMetadataOrderDetailsCmd = new SqlCommand();
            delMetadataOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            delMetadataOrderDetailsCmd.CommandText = "sp_order_details_deletemetadata";
            delMetadataOrderDetailsCmd.Parameters.Add("@order_id", SqlDbType.Int);
            delMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncCheckConcurrency, SqlDbType.Int);
            delMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowTimestamp, SqlDbType.BigInt);
            delMetadataOrderDetailsCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output;

            adaptorOrderDetails.DeleteMetadataCommand = delMetadataOrderDetailsCmd;


            // get tombstones for cleanup
            SqlCommand selTombstonesOrderDetailsCmd = new SqlCommand();
            selTombstonesOrderDetailsCmd.CommandType = CommandType.StoredProcedure;
            selTombstonesOrderDetailsCmd.CommandText = "sp_order_details_selecttombstones";
            selTombstonesOrderDetailsCmd.Parameters.Add("@tombstone_aging_in_hours", SqlDbType.Int).Value = TombstoneAgingInHours;        

            adaptorOrderDetails.SelectMetadataForCleanupCommand = selTombstonesOrderDetailsCmd;


            peerProvider.SyncAdapters.Add(adaptorOrderDetails);


            //
            // 2. Setup provider wide commands
            // There are few commands on the provider itself and not on a table sync adapter:
            // SelectNewTimestampCommand: Returns the new high watermark for current sync
            // SelectScopeInfoCommand: Returns sync knowledge, cleanup knowledge and scope version (timestamp)
            // UpdateScopeInfoCommand: Sets the new values for sync knowledge and cleanup knowledge             
            //

            SqlCommand anchorCmd = new SqlCommand();
            anchorCmd.CommandType = CommandType.Text;
            anchorCmd.CommandText = "select @" + DbSyncSession.SyncNewTimestamp + " = @@DBTS";  // for SQL Server 2005 SP2, use "min_active_rowversion() - 1"
            anchorCmd.Parameters.Add("@" + DbSyncSession.SyncNewTimestamp, SqlDbType.BigInt).Direction = ParameterDirection.Output;

            peerProvider.SelectNewTimestampCommand = anchorCmd;

            // 
            // Select local replica info
            //
            SqlCommand selReplicaInfoCmd = new SqlCommand();
            selReplicaInfoCmd.CommandType = CommandType.Text;
            selReplicaInfoCmd.CommandText = "select " +
                                            "@" + DbSyncSession.SyncScopeId + " = scope_Id, " +
                                            "@" + DbSyncSession.SyncScopeKnowledge + " = scope_sync_knowledge, " +
                                            "@" + DbSyncSession.SyncScopeCleanupKnowledge + " = scope_tombstone_cleanup_knowledge, " +
                                            "@" + DbSyncSession.SyncScopeTimestamp + " = scope_timestamp " +
                                            "from scope_info " +
                                            "where scope_name = @" + DbSyncSession.SyncScopeName;
            selReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeName, SqlDbType.NVarChar, 100);
            selReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeId, SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;
            selReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeKnowledge, SqlDbType.VarBinary, 10000).Direction = ParameterDirection.Output;
            selReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeCleanupKnowledge, SqlDbType.VarBinary, 10000).Direction = ParameterDirection.Output;
            selReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeTimestamp, SqlDbType.BigInt).Direction = ParameterDirection.Output; 
            peerProvider.SelectScopeInfoCommand = selReplicaInfoCmd;

            // 
            // Update local replica info
            //
            SqlCommand updReplicaInfoCmd = new SqlCommand();
            updReplicaInfoCmd.CommandType = CommandType.Text;
            updReplicaInfoCmd.CommandText = "update  scope_info set " +
                                            "scope_sync_knowledge = @" + DbSyncSession.SyncScopeKnowledge + ", " +
                                            "scope_tombstone_cleanup_knowledge = @" + DbSyncSession.SyncScopeCleanupKnowledge + " " +
                                            "where scope_name = @" + DbSyncSession.SyncScopeName + " ;" +
                                            "set @" + DbSyncSession.SyncRowCount + " = @@rowcount";
            updReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeKnowledge, SqlDbType.VarBinary, 10000);
            updReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeCleanupKnowledge, SqlDbType.VarBinary, 10000);
            updReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncScopeName, SqlDbType.NVarChar, 100);
            updReplicaInfoCmd.Parameters.Add("@" + DbSyncSession.SyncRowCount, SqlDbType.Int).Direction = ParameterDirection.Output; 
            peerProvider.UpdateScopeInfoCommand = updReplicaInfoCmd;

            //
            // 3. Track sync process                
            //            
            peerProvider.SyncProgress += new EventHandler<DbSyncProgressEventArgs>(ShowProgress);
            peerProvider.ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(ShowFailures);
                       
            return peerProvider;
        }
      

        //
        // Synchronize Call
        //
        private void buttonSynchronize_Click(object sender, EventArgs e)
        {
            try
            {

                // 
                // 1. Create instance of the sync components (peer, agent, peer)
                // This demo illustrates direct connection to server database. In this scenario, 
                // all sync components reside in the sample process.                 
                //                
                DbSyncProvider localProvider = new DbSyncProvider();
                DbSyncProvider remoteProvider = new DbSyncProvider();

                localProvider = SetupSyncProvider(GetFromPeerConnectionString(), localProvider);
                localProvider.SyncProviderPosition = SyncProviderPosition.Local;                
                
                remoteProvider = SetupSyncProvider(GetToPeerConnectionString(), remoteProvider);
                remoteProvider.SyncProviderPosition = SyncProviderPosition.Remote;
                
                SyncOrchestrator syncOrchestrator = new SyncOrchestrator();
                // sync direction: local -> remote
                syncOrchestrator.LocalProvider = localProvider;
                syncOrchestrator.RemoteProvider = remoteProvider;
                syncOrchestrator.Direction = SyncDirectionOrder.Upload;
                syncOrchestrator.SessionProgress += new EventHandler<SyncStagedProgressEventArgs>(ProgressChanged);

                _progressForm = new ProgressForm();
                _progressForm.Show();
                SyncOperationStatistics syncStats = syncOrchestrator.Synchronize();

                _progressForm.ShowStatistics(syncStats);
                // Update the UI
                _progressForm.EnableClose();
                _progressForm = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (_progressForm != null)
                {
                    _progressForm.EnableClose();
                    _progressForm = null;
                }
            }
        }

        #region UI Code

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();            
        }

        public void ShowProgress(object syncOrchestrator, DbSyncProgressEventArgs args)
        {
            if (null != _progressForm)
            {
                _progressForm.Report(args);
            }
        }

        public void ShowFailures(object syncOrchestrator, DbApplyChangeFailedEventArgs args)
        {
            ConflictForm form = new ConflictForm();
            form.HandleConflict(fromPeer, toPeer, args);
            form.ShowDialog();

        }

        public void ProgressChanged(object sender, SyncStagedProgressEventArgs args)
        {
            if (null != _progressForm)
            {
                _progressForm.ProgressChanged(args);
            }
        }

        protected string GetFromPeerName()
        {            
            if (fromPeer1.Checked)
            {            
                return "peer1";
            }
            else if (fromPeer2.Checked)
            {                
                return "peer2";
            }
            else
            {
                
                return "peer3";
            }            
        }

        protected string GetFromPeerConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["integrated Security"] = true;


            if (fromPeer1.Checked)
            {
                builder["Data Source"] = textPeer1Machine.Text;
                builder["Initial Catalog"] = "peer1";
                
            }
            else if (fromPeer2.Checked)
            {
                builder["Data Source"] = textPeer2Machine.Text;
                builder["Initial Catalog"] = "peer2";
            }
            else
            {
                builder["Data Source"] = textPeer3Machine.Text;
                builder["Initial Catalog"] = "peer3";
            }

            return builder.ToString();
        }

        protected string GetToPeerName()
        {           
            if (toPeer1.Checked)
            {                
                return "peer1";
            }
            else if (toPeer2.Checked)
            {                
                return "peer2";
            }
            else
            {                
                return "peer3";
            }            
        }

        protected string GetToPeerConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["integrated Security"] = true;


            if (toPeer1.Checked)
            {
                builder["Data Source"] = textPeer1Machine.Text;
                builder["Initial Catalog"] = "peer1";
            }
            else if (toPeer2.Checked)
            {
                builder["Data Source"] = textPeer2Machine.Text;
                builder["Initial Catalog"] = "peer2";
            }
            else
            {
                builder["Data Source"] = textPeer3Machine.Text;
                builder["Initial Catalog"] = "peer3";
            }

            return builder.ToString();
        }


        protected TextBox GetFromPeerChangeTextBox()
        {
            if (dataGridOrders.Visible)
            {

                if (fromPeer1.Checked)
                {
                    return textPeer1OrdersChange;
                }
                else if (fromPeer2.Checked)
                {
                    return textPeer2OrdersChange;
                }
                else
                {
                    return textPeer3OrdersChange;
                }
            }
            else
            {
                if (fromPeer1.Checked)
                {
                    return textPeer1OrderDetailsChange;
                }
                else if (fromPeer2.Checked)
                {
                    return textPeer2OrderDetailsChange;
                }
                else
                {
                    return textPeer3OrderDetailsChange;
                }
            }
            
        }

        protected TextBox GetToPeerChangeTextBox()
        {
            if (dataGridOrders.Visible)
            {

                if (toPeer1.Checked)
                {
                    return textPeer1OrdersChange;
                }
                else if (toPeer2.Checked)
                {
                    return textPeer2OrdersChange;
                }
                else
                {
                    return textPeer3OrdersChange;
                }
            }
            else
            {
                if (toPeer1.Checked)
                {
                    return textPeer1OrderDetailsChange;
                }
                else if (toPeer2.Checked)
                {
                    return textPeer2OrderDetailsChange;
                }
                else
                {
                    return textPeer3OrderDetailsChange;
                }
            }
        }

        protected void ClearChangeText()
        {
            textPeer1OrdersChange.Text = "";
            textPeer2OrdersChange.Text = "";
            textPeer3OrdersChange.Text = "";
            textPeer1OrderDetailsChange.Text = "";
            textPeer2OrderDetailsChange.Text = "";
            textPeer3OrderDetailsChange.Text = "";

            Application.DoEvents(); 
        }


        protected string GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["integrated Security"] = true;

            if (tabOrders.Visible)
            {
                if (radioPeer1Orders.Checked)
                {
                    builder["Data Source"] = textPeer1Machine.Text;
                    builder["Initial Catalog"] = "peer1";
                }
                else if (radioPeer2Orders.Checked)
                {
                    builder["Data Source"] = textPeer2Machine.Text;
                    builder["Initial Catalog"] = "peer2";
                }
                else
                {
                    builder["Data Source"] = textPeer3Machine.Text;
                    builder["Initial Catalog"] = "peer3";
                }
            }
            else
            {
                if (radioPeer1OrderDetails.Checked)
                {
                    builder["Data Source"] = textPeer1Machine.Text;
                    builder["Initial Catalog"] = "peer1";
                }
                else if (radioPeer2OrderDetails.Checked)
                {
                    builder["Data Source"] = textPeer2Machine.Text;
                    builder["Initial Catalog"] = "peer2";
                }
                else
                {
                    builder["Data Source"] = textPeer3Machine.Text;
                    builder["Initial Catalog"] = "peer3";
                }
            }

            return builder.ToString();   
        }

        protected TextBox GetCheckedChangeTextBox()
        {
            if (tabOrders.Visible)
            {
                if (radioPeer1Orders.Checked)
                {
                    return textPeer1OrdersChange;
                }
                else if (radioPeer2Orders.Checked)
                {
                    return textPeer2OrdersChange;
                }
                else
                {
                    return textPeer3OrdersChange;
                }

            }
            else
            {
                if (radioPeer1OrderDetails.Checked)
                {
                    return textPeer1OrderDetailsChange;
                }           
                else if (radioPeer2OrderDetails.Checked)
                {
                    return textPeer2OrderDetailsChange;
                }           
                else
                {
                    return textPeer3OrderDetailsChange;
                }       
            }                
        }       


        protected void ReportRandomChanges(TextBox textBox, string description)
        {            
            textBox.Text = description;                        
            Application.DoEvents();
        }       

        private void buttonRefreshOrders_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = GetConnectionString();
                string commandString = "Select * from";

                if (connectionString.Contains("peer1"))
                    commandString += " ordertable";
                else
                    commandString += " orders";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(commandString, GetConnectionString());
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);                

                dataGridOrders.DataSource = dataTable;                                

                Application.DoEvents();
            }           
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void buttonRefreshOrderDetails_Click(object sender, EventArgs e)
        {
            try
            {                     
                string commandString = "Select * from order_details";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(commandString, GetConnectionString());
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                
                dataGridOrderDetails.DataSource = dataTable;
               
                Application.DoEvents();
            }            
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }


        private void radioPeer1Orders_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefreshOrders_Click(sender, e);
        }

        private void radioPeer2Orders_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefreshOrders_Click(sender, e);
        }

        private void radioPeer3Orders_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefreshOrders_Click(sender, e);
        }

        private void radioPeer1OrderDetails_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefreshOrderDetails_Click(sender, e);
        }

        private void radioPeer2OrderDetails_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefreshOrderDetails_Click(sender, e);
        }

        private void radioPeer3OrderDetails_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefreshOrderDetails_Click(sender, e);
        }

        private void fromPeer1_CheckedChanged(object sender, EventArgs e)
        {
            if (toPeer1.Checked)
            {
                toPeer2.Checked = true;
                Application.DoEvents();
            }
        }

        private void fromPeer2_CheckedChanged(object sender, EventArgs e)
        {
            if (toPeer2.Checked)
            {
                toPeer1.Checked = true;
                Application.DoEvents();
            }
        }

        private void fromPeer3_CheckedChanged(object sender, EventArgs e)
        {
            if (toPeer3.Checked)
            {
                toPeer1.Checked = true;
                Application.DoEvents();
            }
        }

        private void toPeer1_CheckedChanged(object sender, EventArgs e)
        {
            if (fromPeer1.Checked)
            {
                fromPeer2.Checked = true;
                Application.DoEvents();
            }
        }

        private void toPeer2_CheckedChanged(object sender, EventArgs e)
        {
            if (fromPeer2.Checked)
            {
                fromPeer1.Checked = true;
                Application.DoEvents();
            }
        }

        private void toPeer3_CheckedChanged(object sender, EventArgs e)
        {
            if (fromPeer3.Checked)
            {
                fromPeer1.Checked = true;
                Application.DoEvents();
            }
        }       


#endregion

        #region Random Inserts, Updates and Delets to client DB


        private void InsertOrder(string connString, int key, TextBox changeBox)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand();
            if (connection.Database == "peer1")
                command.CommandText = "INSERT INTO ordertable(orderid, orderdate) values(@order_id, @order_date)";
            else
                command.CommandText = "INSERT INTO orders(order_id, order_date) values(@order_id, @order_date)";
            command.Parameters.AddWithValue("@order_id", key);
            command.Parameters.AddWithValue("@order_date", DateTime.Now);
            command.Connection = connection;

            try
            {
                connection.Open();
                int count = command.ExecuteNonQuery();
                count /= 2;
                ReportRandomChanges(changeBox, count.ToString() + " rows inserted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Item Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void InsertOrderDetails(string connString, int key, TextBox changeBox)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand();
            command.CommandText = "INSERT INTO order_details(order_id, order_details_id, product, quantity) values(@order_id, @order_details_id, @product, @quantity)";
            command.Parameters.AddWithValue("@order_id", key);
            command.Parameters.AddWithValue("@order_details_id", key % 1000);
            command.Parameters.AddWithValue("@product", "NEW");
            command.Parameters.AddWithValue("@quantity", key % 1000000);
            command.Connection = connection;

            try
            {
                connection.Open();
                int count = command.ExecuteNonQuery();
                count /= 2;
                ReportRandomChanges(changeBox, count.ToString() + " rows inserted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Item Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void UpdateOrder(string connString, int key, TextBox changeBox)
        {
            Random rand = new Random();

            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand();
            if (connection.Database == "peer1")
                command.CommandText = "UPDATE ordertable SET orderdate = @order_date where orderid % @factor < 10";
            else
                command.CommandText = "UPDATE orders SET order_date = @order_date where order_id % @factor < 10";
            command.Parameters.AddWithValue("@factor", key);
            command.Parameters.AddWithValue("@order_date", DateTime.Now);
            command.Connection = connection;

            try
            {
                connection.Open();
                int count = command.ExecuteNonQuery();
                count /= 2;
                ReportRandomChanges(changeBox, count.ToString() + " rows updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Item Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void UpdateOrderDetails(string connString, int key, TextBox changeBox)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE order_details SET quantity = @quantity, product = @product where order_id % @factor < 10";
            command.Parameters.AddWithValue("@factor", key);
            command.Parameters.AddWithValue("@product", "UPD");
            command.Parameters.AddWithValue("@quantity", key % 1000000);
            command.Connection = connection;

            try
            {
                connection.Open();
                int count = command.ExecuteNonQuery();
                count /= 2;
                ReportRandomChanges(changeBox, count.ToString() + " rows updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Item Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void DeleteOrder(string connString, int key, TextBox changeBox)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand();
            if (connection.Database == "peer1")
                command.CommandText = "DELETE ordertable WHERE orderid % @factor < 5";
            else
                command.CommandText = "DELETE orders WHERE order_id % @factor < 5";
            command.Parameters.AddWithValue("@factor", key);
            command.Connection = connection;

            try
            {
                connection.Open();
                int count = command.ExecuteNonQuery();
                count /= 2;
                ReportRandomChanges(changeBox, count.ToString() + " rows deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Item Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }            
        }

        private void DeleteOrderDetails(string connString, int key, TextBox changeBox)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand command = new SqlCommand();
            command.CommandText = "DELETE order_details WHERE order_id % @factor < 10";
            command.Parameters.AddWithValue("@factor", key);
            command.Connection = connection;

            try
            {
                connection.Open();
                int count = command.ExecuteNonQuery();
                count /= 2;
                ReportRandomChanges(changeBox, count.ToString() + " rows deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Item Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        private void buttonApplyOrdersInserts_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            ClearChangeText();
            InsertOrder(GetConnectionString(), rand.Next((int)(DateTime.Now.ToFileTime() % 10000)), GetCheckedChangeTextBox());                        
        }

        private void buttonApplyOrderDetailsInserts_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            ClearChangeText();
            InsertOrderDetails(GetConnectionString(), rand.Next((int)(DateTime.Now.ToFileTime() % 10000)), GetCheckedChangeTextBox());            
        }

        private void buttonApplyOrdersUpdates_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            ClearChangeText();
            UpdateOrder(GetConnectionString(), rand.Next((int)(DateTime.Now.ToFileTime() % 1000)), GetCheckedChangeTextBox());                        
        }

        private void buttonApplyOrderDetailsUpdates_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            ClearChangeText();
            UpdateOrderDetails(GetConnectionString(), rand.Next((int)(DateTime.Now.ToFileTime() % 1000)), GetCheckedChangeTextBox());            
        }

        private void buttonApplyOrdersDeletes_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            ClearChangeText();
            DeleteOrder(GetConnectionString(), rand.Next((int)(DateTime.Now.ToFileTime()% 1000)), GetCheckedChangeTextBox());                        
        }

        private void buttonApplyOrderDetailsDeletes_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            ClearChangeText();
            DeleteOrderDetails(GetConnectionString(), rand.Next((int)(DateTime.Now.ToFileTime() % 1000)), GetCheckedChangeTextBox());            
        }
                        
        private void buttonInsInsConflict_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int key = rand.Next((int)(DateTime.Now.ToFileTime() % 10000));
            ClearChangeText();

            if (dataGridOrders.Visible)
            {
                // from peer
                InsertOrder(GetFromPeerConnectionString(), key, GetFromPeerChangeTextBox());

                // to peer
                InsertOrder(GetToPeerConnectionString(), key, GetToPeerChangeTextBox());
            }
            else
            {
                // from peer
                InsertOrderDetails(GetFromPeerConnectionString(), key, GetFromPeerChangeTextBox());

                // to peer
                InsertOrderDetails(GetToPeerConnectionString(), key, GetToPeerChangeTextBox());
            }
        }

        private void buttonUpdUpdConflict_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int key = rand.Next((int)(DateTime.Now.ToFileTime() % 10000));
            ClearChangeText();

            if (dataGridOrders.Visible)
            {
                // from peer
                UpdateOrder(GetFromPeerConnectionString(), key, GetFromPeerChangeTextBox());

                // to peer
                UpdateOrder(GetToPeerConnectionString(), key, GetToPeerChangeTextBox());
            }
            else
            {
                // from peer
                UpdateOrderDetails(GetFromPeerConnectionString(), key, GetFromPeerChangeTextBox());

                // to peer
                UpdateOrderDetails(GetToPeerConnectionString(), key, GetToPeerChangeTextBox());
            }
        }

        private void buttonUpdDelConflict_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int key = rand.Next((int)(DateTime.Now.ToFileTime() % 100));
            ClearChangeText();

            if (dataGridOrders.Visible)
            {
                // from peer
                UpdateOrder(GetFromPeerConnectionString(), key, GetFromPeerChangeTextBox());

                // to peer
                DeleteOrder(GetToPeerConnectionString(), key, GetToPeerChangeTextBox());
            }
            else
            {
                // from peer
                UpdateOrderDetails(GetFromPeerConnectionString(), key, GetFromPeerChangeTextBox());

                // to peer
                DeleteOrderDetails(GetToPeerConnectionString(), key, GetToPeerChangeTextBox());
            }
        }

        #endregion

        private void buttonCleanupOrdersMetadata_Click(object sender, EventArgs e)
        {
            try
            {
                DbSyncProvider provider = new DbSyncProvider();
                
                provider = SetupSyncProvider(GetConnectionString(), provider);

                if (provider.CleanupMetadata() == false)
                {
                    MessageBox.Show("Metadata cleanup failed.  Please retry");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (_progressForm != null)
                {
                    _progressForm.EnableClose();
                    _progressForm = null;
                }
            }
        }
    }
}
