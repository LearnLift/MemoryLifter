// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace SyncApplication
{
    partial class SyncForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonSynchornize = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabOrders = new System.Windows.Forms.TabPage();
            this.textPeer3OrdersChange = new System.Windows.Forms.TextBox();
            this.textPeer2OrdersChange = new System.Windows.Forms.TextBox();
            this.textPeer1OrdersChange = new System.Windows.Forms.TextBox();
            this.radioPeer3Orders = new System.Windows.Forms.RadioButton();
            this.buttonApplyOrdersDeletes = new System.Windows.Forms.Button();
            this.buttonApplyOrdersUpdates = new System.Windows.Forms.Button();
            this.radioPeer2Orders = new System.Windows.Forms.RadioButton();
            this.radioPeer1Orders = new System.Windows.Forms.RadioButton();
            this.buttonRefreshOrders = new System.Windows.Forms.Button();
            this.buttonApplyOrdersInserts = new System.Windows.Forms.Button();
            this.dataGridOrders = new System.Windows.Forms.DataGridView();
            this.tabOrderDetails = new System.Windows.Forms.TabPage();
            this.textPeer3OrderDetailsChange = new System.Windows.Forms.TextBox();
            this.textPeer2OrderDetailsChange = new System.Windows.Forms.TextBox();
            this.textPeer1OrderDetailsChange = new System.Windows.Forms.TextBox();
            this.radioPeer3OrderDetails = new System.Windows.Forms.RadioButton();
            this.buttonApplyOrderDetailsDeletes = new System.Windows.Forms.Button();
            this.buttonApplyOrderDetailsUpdates = new System.Windows.Forms.Button();
            this.radioPeer2OrderDetails = new System.Windows.Forms.RadioButton();
            this.radioPeer1OrderDetails = new System.Windows.Forms.RadioButton();
            this.buttonApplyOrderDetailsInserts = new System.Windows.Forms.Button();
            this.buttonRefreshOrderDetails = new System.Windows.Forms.Button();
            this.dataGridOrderDetails = new System.Windows.Forms.DataGridView();
            this.textPeer1Machine = new System.Windows.Forms.TextBox();
            this.textPeer2Machine = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ordersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.textPeer3Machine = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fromPeer3 = new System.Windows.Forms.RadioButton();
            this.fromPeer2 = new System.Windows.Forms.RadioButton();
            this.fromPeer1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.toPeer3 = new System.Windows.Forms.RadioButton();
            this.toPeer2 = new System.Windows.Forms.RadioButton();
            this.toPeer1 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOrders)).BeginInit();
            this.tabOrderDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOrderDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ordersBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSynchornize
            // 
            this.buttonSynchornize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSynchornize.Location = new System.Drawing.Point(503, 540);
            this.buttonSynchornize.Name = "buttonSynchornize";
            this.buttonSynchornize.Size = new System.Drawing.Size(188, 55);
            this.buttonSynchornize.TabIndex = 0;
            this.buttonSynchornize.Text = "&Synchronize";
            this.buttonSynchornize.UseVisualStyleBackColor = true;
            this.buttonSynchornize.Click += new System.EventHandler(this.buttonSynchronize_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(503, 601);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(188, 30);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.Text = "E&xit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabOrders);
            this.tabControl1.Controls.Add(this.tabOrderDetails);
            this.tabControl1.Location = new System.Drawing.Point(12, 78);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(679, 446);
            this.tabControl1.TabIndex = 3;
            // 
            // tabOrders
            // 
            this.tabOrders.Controls.Add(this.textPeer3OrdersChange);
            this.tabOrders.Controls.Add(this.textPeer2OrdersChange);
            this.tabOrders.Controls.Add(this.textPeer1OrdersChange);
            this.tabOrders.Controls.Add(this.radioPeer3Orders);
            this.tabOrders.Controls.Add(this.buttonApplyOrdersDeletes);
            this.tabOrders.Controls.Add(this.buttonApplyOrdersUpdates);
            this.tabOrders.Controls.Add(this.radioPeer2Orders);
            this.tabOrders.Controls.Add(this.radioPeer1Orders);
            this.tabOrders.Controls.Add(this.buttonRefreshOrders);
            this.tabOrders.Controls.Add(this.buttonApplyOrdersInserts);
            this.tabOrders.Controls.Add(this.dataGridOrders);
            this.tabOrders.Location = new System.Drawing.Point(4, 22);
            this.tabOrders.Name = "tabOrders";
            this.tabOrders.Padding = new System.Windows.Forms.Padding(3);
            this.tabOrders.Size = new System.Drawing.Size(671, 420);
            this.tabOrders.TabIndex = 0;
            this.tabOrders.Text = "Orders";
            this.tabOrders.UseVisualStyleBackColor = true;
            // 
            // textPeer3OrdersChange
            // 
            this.textPeer3OrdersChange.Location = new System.Drawing.Point(137, 360);
            this.textPeer3OrdersChange.Name = "textPeer3OrdersChange";
            this.textPeer3OrdersChange.ReadOnly = true;
            this.textPeer3OrdersChange.Size = new System.Drawing.Size(100, 20);
            this.textPeer3OrdersChange.TabIndex = 10;
            // 
            // textPeer2OrdersChange
            // 
            this.textPeer2OrdersChange.Location = new System.Drawing.Point(137, 338);
            this.textPeer2OrdersChange.Name = "textPeer2OrdersChange";
            this.textPeer2OrdersChange.ReadOnly = true;
            this.textPeer2OrdersChange.Size = new System.Drawing.Size(100, 20);
            this.textPeer2OrdersChange.TabIndex = 9;
            // 
            // textPeer1OrdersChange
            // 
            this.textPeer1OrdersChange.Location = new System.Drawing.Point(137, 315);
            this.textPeer1OrdersChange.Name = "textPeer1OrdersChange";
            this.textPeer1OrdersChange.ReadOnly = true;
            this.textPeer1OrdersChange.Size = new System.Drawing.Size(100, 20);
            this.textPeer1OrdersChange.TabIndex = 8;
            // 
            // radioPeer3Orders
            // 
            this.radioPeer3Orders.AutoSize = true;
            this.radioPeer3Orders.Location = new System.Drawing.Point(15, 361);
            this.radioPeer3Orders.Name = "radioPeer3Orders";
            this.radioPeer3Orders.Size = new System.Drawing.Size(102, 17);
            this.radioPeer3Orders.TabIndex = 7;
            this.radioPeer3Orders.TabStop = true;
            this.radioPeer3Orders.Text = "Peer3 Database";
            this.radioPeer3Orders.UseVisualStyleBackColor = true;
            this.radioPeer3Orders.CheckedChanged += new System.EventHandler(this.radioPeer3Orders_CheckedChanged);
            // 
            // buttonApplyOrdersDeletes
            // 
            this.buttonApplyOrdersDeletes.Location = new System.Drawing.Point(474, 387);
            this.buttonApplyOrdersDeletes.Name = "buttonApplyOrdersDeletes";
            this.buttonApplyOrdersDeletes.Size = new System.Drawing.Size(177, 23);
            this.buttonApplyOrdersDeletes.TabIndex = 6;
            this.buttonApplyOrdersDeletes.Text = "Make Random &Deletes";
            this.buttonApplyOrdersDeletes.UseVisualStyleBackColor = true;
            this.buttonApplyOrdersDeletes.Click += new System.EventHandler(this.buttonApplyOrdersDeletes_Click);
            // 
            // buttonApplyOrdersUpdates
            // 
            this.buttonApplyOrdersUpdates.Location = new System.Drawing.Point(474, 351);
            this.buttonApplyOrdersUpdates.Name = "buttonApplyOrdersUpdates";
            this.buttonApplyOrdersUpdates.Size = new System.Drawing.Size(177, 23);
            this.buttonApplyOrdersUpdates.TabIndex = 5;
            this.buttonApplyOrdersUpdates.Text = "Make Random &Updates";
            this.buttonApplyOrdersUpdates.UseVisualStyleBackColor = true;
            this.buttonApplyOrdersUpdates.Click += new System.EventHandler(this.buttonApplyOrdersUpdates_Click);
            // 
            // radioPeer2Orders
            // 
            this.radioPeer2Orders.AutoSize = true;
            this.radioPeer2Orders.Location = new System.Drawing.Point(15, 339);
            this.radioPeer2Orders.Name = "radioPeer2Orders";
            this.radioPeer2Orders.Size = new System.Drawing.Size(102, 17);
            this.radioPeer2Orders.TabIndex = 4;
            this.radioPeer2Orders.TabStop = true;
            this.radioPeer2Orders.Text = "Peer2 Database";
            this.radioPeer2Orders.UseVisualStyleBackColor = true;
            this.radioPeer2Orders.CheckedChanged += new System.EventHandler(this.radioPeer2Orders_CheckedChanged);
            // 
            // radioPeer1Orders
            // 
            this.radioPeer1Orders.AutoSize = true;
            this.radioPeer1Orders.Checked = true;
            this.radioPeer1Orders.Location = new System.Drawing.Point(15, 316);
            this.radioPeer1Orders.Name = "radioPeer1Orders";
            this.radioPeer1Orders.Size = new System.Drawing.Size(102, 17);
            this.radioPeer1Orders.TabIndex = 3;
            this.radioPeer1Orders.TabStop = true;
            this.radioPeer1Orders.Text = "Peer1 Database";
            this.radioPeer1Orders.UseVisualStyleBackColor = true;
            this.radioPeer1Orders.CheckedChanged += new System.EventHandler(this.radioPeer1Orders_CheckedChanged);
            // 
            // buttonRefreshOrders
            // 
            this.buttonRefreshOrders.Location = new System.Drawing.Point(15, 387);
            this.buttonRefreshOrders.Name = "buttonRefreshOrders";
            this.buttonRefreshOrders.Size = new System.Drawing.Size(222, 23);
            this.buttonRefreshOrders.TabIndex = 2;
            this.buttonRefreshOrders.Text = "&Refresh";
            this.buttonRefreshOrders.UseVisualStyleBackColor = true;
            this.buttonRefreshOrders.Click += new System.EventHandler(this.buttonRefreshOrders_Click);
            // 
            // buttonApplyOrdersInserts
            // 
            this.buttonApplyOrdersInserts.Location = new System.Drawing.Point(474, 316);
            this.buttonApplyOrdersInserts.Name = "buttonApplyOrdersInserts";
            this.buttonApplyOrdersInserts.Size = new System.Drawing.Size(177, 23);
            this.buttonApplyOrdersInserts.TabIndex = 1;
            this.buttonApplyOrdersInserts.Text = "Make Random &Inserts";
            this.buttonApplyOrdersInserts.UseVisualStyleBackColor = true;
            this.buttonApplyOrdersInserts.Click += new System.EventHandler(this.buttonApplyOrdersInserts_Click);
            // 
            // dataGridOrders
            // 
            this.dataGridOrders.AllowUserToAddRows = false;
            this.dataGridOrders.AllowUserToDeleteRows = false;
            this.dataGridOrders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridOrders.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridOrders.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dataGridOrders.Location = new System.Drawing.Point(15, 19);
            this.dataGridOrders.Name = "dataGridOrders";
            this.dataGridOrders.Size = new System.Drawing.Size(636, 286);
            this.dataGridOrders.TabIndex = 0;
            // 
            // tabOrderDetails
            // 
            this.tabOrderDetails.Controls.Add(this.textPeer3OrderDetailsChange);
            this.tabOrderDetails.Controls.Add(this.textPeer2OrderDetailsChange);
            this.tabOrderDetails.Controls.Add(this.textPeer1OrderDetailsChange);
            this.tabOrderDetails.Controls.Add(this.radioPeer3OrderDetails);
            this.tabOrderDetails.Controls.Add(this.buttonApplyOrderDetailsDeletes);
            this.tabOrderDetails.Controls.Add(this.buttonApplyOrderDetailsUpdates);
            this.tabOrderDetails.Controls.Add(this.radioPeer2OrderDetails);
            this.tabOrderDetails.Controls.Add(this.radioPeer1OrderDetails);
            this.tabOrderDetails.Controls.Add(this.buttonApplyOrderDetailsInserts);
            this.tabOrderDetails.Controls.Add(this.buttonRefreshOrderDetails);
            this.tabOrderDetails.Controls.Add(this.dataGridOrderDetails);
            this.tabOrderDetails.Location = new System.Drawing.Point(4, 22);
            this.tabOrderDetails.Name = "tabOrderDetails";
            this.tabOrderDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tabOrderDetails.Size = new System.Drawing.Size(671, 420);
            this.tabOrderDetails.TabIndex = 1;
            this.tabOrderDetails.Text = "Order Details";
            this.tabOrderDetails.UseVisualStyleBackColor = true;
            // 
            // textPeer3OrderDetailsChange
            // 
            this.textPeer3OrderDetailsChange.Location = new System.Drawing.Point(137, 360);
            this.textPeer3OrderDetailsChange.Name = "textPeer3OrderDetailsChange";
            this.textPeer3OrderDetailsChange.ReadOnly = true;
            this.textPeer3OrderDetailsChange.Size = new System.Drawing.Size(100, 20);
            this.textPeer3OrderDetailsChange.TabIndex = 12;
            // 
            // textPeer2OrderDetailsChange
            // 
            this.textPeer2OrderDetailsChange.Location = new System.Drawing.Point(137, 338);
            this.textPeer2OrderDetailsChange.Name = "textPeer2OrderDetailsChange";
            this.textPeer2OrderDetailsChange.ReadOnly = true;
            this.textPeer2OrderDetailsChange.Size = new System.Drawing.Size(100, 20);
            this.textPeer2OrderDetailsChange.TabIndex = 11;
            // 
            // textPeer1OrderDetailsChange
            // 
            this.textPeer1OrderDetailsChange.Location = new System.Drawing.Point(137, 315);
            this.textPeer1OrderDetailsChange.Name = "textPeer1OrderDetailsChange";
            this.textPeer1OrderDetailsChange.ReadOnly = true;
            this.textPeer1OrderDetailsChange.Size = new System.Drawing.Size(100, 20);
            this.textPeer1OrderDetailsChange.TabIndex = 10;
            // 
            // radioPeer3OrderDetails
            // 
            this.radioPeer3OrderDetails.AutoSize = true;
            this.radioPeer3OrderDetails.Location = new System.Drawing.Point(15, 361);
            this.radioPeer3OrderDetails.Name = "radioPeer3OrderDetails";
            this.radioPeer3OrderDetails.Size = new System.Drawing.Size(102, 17);
            this.radioPeer3OrderDetails.TabIndex = 9;
            this.radioPeer3OrderDetails.TabStop = true;
            this.radioPeer3OrderDetails.Text = "Peer3 Database";
            this.radioPeer3OrderDetails.UseVisualStyleBackColor = true;
            this.radioPeer3OrderDetails.CheckedChanged += new System.EventHandler(this.radioPeer3OrderDetails_CheckedChanged);
            // 
            // buttonApplyOrderDetailsDeletes
            // 
            this.buttonApplyOrderDetailsDeletes.Location = new System.Drawing.Point(474, 387);
            this.buttonApplyOrderDetailsDeletes.Name = "buttonApplyOrderDetailsDeletes";
            this.buttonApplyOrderDetailsDeletes.Size = new System.Drawing.Size(177, 23);
            this.buttonApplyOrderDetailsDeletes.TabIndex = 8;
            this.buttonApplyOrderDetailsDeletes.Text = "Make Random &Deletes";
            this.buttonApplyOrderDetailsDeletes.UseVisualStyleBackColor = true;
            this.buttonApplyOrderDetailsDeletes.Click += new System.EventHandler(this.buttonApplyOrderDetailsDeletes_Click);
            // 
            // buttonApplyOrderDetailsUpdates
            // 
            this.buttonApplyOrderDetailsUpdates.Location = new System.Drawing.Point(474, 351);
            this.buttonApplyOrderDetailsUpdates.Name = "buttonApplyOrderDetailsUpdates";
            this.buttonApplyOrderDetailsUpdates.Size = new System.Drawing.Size(177, 23);
            this.buttonApplyOrderDetailsUpdates.TabIndex = 7;
            this.buttonApplyOrderDetailsUpdates.Text = "Make Random &Updates";
            this.buttonApplyOrderDetailsUpdates.UseVisualStyleBackColor = true;
            this.buttonApplyOrderDetailsUpdates.Click += new System.EventHandler(this.buttonApplyOrderDetailsUpdates_Click);
            // 
            // radioPeer2OrderDetails
            // 
            this.radioPeer2OrderDetails.AutoSize = true;
            this.radioPeer2OrderDetails.Location = new System.Drawing.Point(15, 339);
            this.radioPeer2OrderDetails.Name = "radioPeer2OrderDetails";
            this.radioPeer2OrderDetails.Size = new System.Drawing.Size(102, 17);
            this.radioPeer2OrderDetails.TabIndex = 6;
            this.radioPeer2OrderDetails.TabStop = true;
            this.radioPeer2OrderDetails.Text = "Peer2 Database";
            this.radioPeer2OrderDetails.UseVisualStyleBackColor = true;
            this.radioPeer2OrderDetails.CheckedChanged += new System.EventHandler(this.radioPeer2OrderDetails_CheckedChanged);
            // 
            // radioPeer1OrderDetails
            // 
            this.radioPeer1OrderDetails.AutoSize = true;
            this.radioPeer1OrderDetails.Checked = true;
            this.radioPeer1OrderDetails.Location = new System.Drawing.Point(15, 316);
            this.radioPeer1OrderDetails.Name = "radioPeer1OrderDetails";
            this.radioPeer1OrderDetails.Size = new System.Drawing.Size(102, 17);
            this.radioPeer1OrderDetails.TabIndex = 5;
            this.radioPeer1OrderDetails.TabStop = true;
            this.radioPeer1OrderDetails.Text = "Peer1 Database";
            this.radioPeer1OrderDetails.UseVisualStyleBackColor = true;
            this.radioPeer1OrderDetails.CheckedChanged += new System.EventHandler(this.radioPeer1OrderDetails_CheckedChanged);
            // 
            // buttonApplyOrderDetailsInserts
            // 
            this.buttonApplyOrderDetailsInserts.Location = new System.Drawing.Point(474, 316);
            this.buttonApplyOrderDetailsInserts.Name = "buttonApplyOrderDetailsInserts";
            this.buttonApplyOrderDetailsInserts.Size = new System.Drawing.Size(177, 23);
            this.buttonApplyOrderDetailsInserts.TabIndex = 4;
            this.buttonApplyOrderDetailsInserts.Text = "Make Random &Inserts";
            this.buttonApplyOrderDetailsInserts.UseVisualStyleBackColor = true;
            this.buttonApplyOrderDetailsInserts.Click += new System.EventHandler(this.buttonApplyOrderDetailsInserts_Click);
            // 
            // buttonRefreshOrderDetails
            // 
            this.buttonRefreshOrderDetails.Location = new System.Drawing.Point(15, 387);
            this.buttonRefreshOrderDetails.Name = "buttonRefreshOrderDetails";
            this.buttonRefreshOrderDetails.Size = new System.Drawing.Size(222, 23);
            this.buttonRefreshOrderDetails.TabIndex = 3;
            this.buttonRefreshOrderDetails.Text = "&Refresh";
            this.buttonRefreshOrderDetails.UseVisualStyleBackColor = true;
            this.buttonRefreshOrderDetails.Click += new System.EventHandler(this.buttonRefreshOrderDetails_Click);
            // 
            // dataGridOrderDetails
            // 
            this.dataGridOrderDetails.AllowUserToAddRows = false;
            this.dataGridOrderDetails.AllowUserToDeleteRows = false;
            this.dataGridOrderDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridOrderDetails.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridOrderDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridOrderDetails.Location = new System.Drawing.Point(15, 19);
            this.dataGridOrderDetails.Name = "dataGridOrderDetails";
            this.dataGridOrderDetails.Size = new System.Drawing.Size(636, 286);
            this.dataGridOrderDetails.TabIndex = 0;
            // 
            // textPeer1Machine
            // 
            this.textPeer1Machine.Location = new System.Drawing.Point(543, 7);
            this.textPeer1Machine.Name = "textPeer1Machine";
            this.textPeer1Machine.Size = new System.Drawing.Size(148, 20);
            this.textPeer1Machine.TabIndex = 4;
            // 
            // textPeer2Machine
            // 
            this.textPeer2Machine.Location = new System.Drawing.Point(543, 33);
            this.textPeer2Machine.Name = "textPeer2Machine";
            this.textPeer2Machine.Size = new System.Drawing.Size(148, 20);
            this.textPeer2Machine.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(448, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Peer1 Machine";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(448, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Peer2 Machine";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(448, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Peer3 Machine";
            // 
            // textPeer3Machine
            // 
            this.textPeer3Machine.Location = new System.Drawing.Point(543, 59);
            this.textPeer3Machine.Name = "textPeer3Machine";
            this.textPeer3Machine.Size = new System.Drawing.Size(148, 20);
            this.textPeer3Machine.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fromPeer3);
            this.groupBox1.Controls.Add(this.fromPeer2);
            this.groupBox1.Controls.Add(this.fromPeer1);
            this.groupBox1.Location = new System.Drawing.Point(16, 536);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 95);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "From";
            // 
            // fromPeer3
            // 
            this.fromPeer3.AutoSize = true;
            this.fromPeer3.Location = new System.Drawing.Point(15, 64);
            this.fromPeer3.Name = "fromPeer3";
            this.fromPeer3.Size = new System.Drawing.Size(102, 17);
            this.fromPeer3.TabIndex = 10;
            this.fromPeer3.TabStop = true;
            this.fromPeer3.Text = "Peer3 Database";
            this.fromPeer3.UseVisualStyleBackColor = true;
            this.fromPeer3.Click += new System.EventHandler(this.fromPeer3_CheckedChanged);
            // 
            // fromPeer2
            // 
            this.fromPeer2.AutoSize = true;
            this.fromPeer2.Location = new System.Drawing.Point(15, 42);
            this.fromPeer2.Name = "fromPeer2";
            this.fromPeer2.Size = new System.Drawing.Size(102, 17);
            this.fromPeer2.TabIndex = 9;
            this.fromPeer2.TabStop = true;
            this.fromPeer2.Text = "Peer2 Database";
            this.fromPeer2.UseVisualStyleBackColor = true;
            this.fromPeer2.Click += new System.EventHandler(this.fromPeer2_CheckedChanged);
            // 
            // fromPeer1
            // 
            this.fromPeer1.AutoSize = true;
            this.fromPeer1.Checked = true;
            this.fromPeer1.Location = new System.Drawing.Point(15, 19);
            this.fromPeer1.Name = "fromPeer1";
            this.fromPeer1.Size = new System.Drawing.Size(102, 17);
            this.fromPeer1.TabIndex = 8;
            this.fromPeer1.TabStop = true;
            this.fromPeer1.Text = "Peer1 Database";
            this.fromPeer1.UseVisualStyleBackColor = true;
            this.fromPeer1.Click += new System.EventHandler(this.fromPeer1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.toPeer3);
            this.groupBox2.Controls.Add(this.toPeer2);
            this.groupBox2.Controls.Add(this.toPeer1);
            this.groupBox2.Location = new System.Drawing.Point(218, 536);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(174, 95);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "To";
            // 
            // toPeer3
            // 
            this.toPeer3.AutoSize = true;
            this.toPeer3.Location = new System.Drawing.Point(20, 64);
            this.toPeer3.Name = "toPeer3";
            this.toPeer3.Size = new System.Drawing.Size(102, 17);
            this.toPeer3.TabIndex = 10;
            this.toPeer3.Text = "Peer3 Database";
            this.toPeer3.UseVisualStyleBackColor = true;
            this.toPeer3.Click += new System.EventHandler(this.toPeer3_CheckedChanged);
            // 
            // toPeer2
            // 
            this.toPeer2.AutoSize = true;
            this.toPeer2.Checked = true;
            this.toPeer2.Location = new System.Drawing.Point(20, 42);
            this.toPeer2.Name = "toPeer2";
            this.toPeer2.Size = new System.Drawing.Size(102, 17);
            this.toPeer2.TabIndex = 9;
            this.toPeer2.TabStop = true;
            this.toPeer2.Text = "Peer2 Database";
            this.toPeer2.UseVisualStyleBackColor = true;
            this.toPeer2.Click += new System.EventHandler(this.toPeer2_CheckedChanged);
            // 
            // toPeer1
            // 
            this.toPeer1.AutoSize = true;
            this.toPeer1.Location = new System.Drawing.Point(20, 19);
            this.toPeer1.Name = "toPeer1";
            this.toPeer1.Size = new System.Drawing.Size(102, 17);
            this.toPeer1.TabIndex = 8;
            this.toPeer1.Text = "Peer1 Database";
            this.toPeer1.UseVisualStyleBackColor = true;
            this.toPeer1.Click += new System.EventHandler(this.toPeer1_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(189, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "Peer to Peer Data Sharing";
            // 
            // SyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 644);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textPeer3Machine);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textPeer2Machine);
            this.Controls.Add(this.textPeer1Machine);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonSynchornize);
            this.Name = "SyncForm";
            this.Text = "share\'em - Data Synchronization Demo Application";
            this.tabControl1.ResumeLayout(false);
            this.tabOrders.ResumeLayout(false);
            this.tabOrders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOrders)).EndInit();
            this.tabOrderDetails.ResumeLayout(false);
            this.tabOrderDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOrderDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ordersBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSynchornize;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabOrders;
        private System.Windows.Forms.TabPage tabOrderDetails;
        private System.Windows.Forms.DataGridView dataGridOrders;
        private System.Windows.Forms.DataGridView dataGridOrderDetails;
        private System.Windows.Forms.Button buttonApplyOrdersInserts;
        private System.Windows.Forms.Button buttonRefreshOrders;
        private System.Windows.Forms.RadioButton radioPeer2Orders;
        private System.Windows.Forms.RadioButton radioPeer1Orders;
        private System.Windows.Forms.RadioButton radioPeer2OrderDetails;
        private System.Windows.Forms.RadioButton radioPeer1OrderDetails;
        private System.Windows.Forms.Button buttonApplyOrderDetailsInserts;
        private System.Windows.Forms.Button buttonRefreshOrderDetails;
        private System.Windows.Forms.TextBox textPeer1Machine;
        private System.Windows.Forms.TextBox textPeer2Machine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.BindingSource ordersBindingSource;
        private System.Windows.Forms.Button buttonApplyOrderDetailsDeletes;
        private System.Windows.Forms.Button buttonApplyOrderDetailsUpdates;
        private System.Windows.Forms.Button buttonApplyOrdersDeletes;
        private System.Windows.Forms.Button buttonApplyOrdersUpdates;
        private System.Windows.Forms.RadioButton radioPeer3Orders;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textPeer3Machine;
        private System.Windows.Forms.RadioButton radioPeer3OrderDetails;
        private System.Windows.Forms.TextBox textPeer3OrdersChange;
        private System.Windows.Forms.TextBox textPeer2OrdersChange;
        private System.Windows.Forms.TextBox textPeer1OrdersChange;
        private System.Windows.Forms.TextBox textPeer2OrderDetailsChange;
        private System.Windows.Forms.TextBox textPeer1OrderDetailsChange;
        private System.Windows.Forms.TextBox textPeer3OrderDetailsChange;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton fromPeer3;
        private System.Windows.Forms.RadioButton fromPeer2;
        private System.Windows.Forms.RadioButton fromPeer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton toPeer3;
        private System.Windows.Forms.RadioButton toPeer2;
        private System.Windows.Forms.RadioButton toPeer1;
        private System.Windows.Forms.Label label4;

    }
}

