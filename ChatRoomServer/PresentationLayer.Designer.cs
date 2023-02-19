namespace ChatRoomServer
{
    partial class PresentationLayer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.lblConnectedClients = new System.Windows.Forms.Label();
            this.lblServerIpAddress = new System.Windows.Forms.Label();
            this.lblListenOnPort = new System.Windows.Forms.Label();
            this.txtServerStatus = new System.Windows.Forms.TextBox();
            this.txtConnectedClients = new System.Windows.Forms.TextBox();
            this.txtServerIpAddress = new System.Windows.Forms.TextBox();
            this.txtListenOnPort = new System.Windows.Forms.TextBox();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.lblWarningPort = new System.Windows.Forms.Label();
            this.txtServerStatusLogger = new System.Windows.Forms.TextBox();
            this.lvAllConnectedClients = new System.Windows.Forms.ListView();
            this.colClientUsername = new System.Windows.Forms.ColumnHeader();
            this.colClientId = new System.Windows.Forms.ColumnHeader();
            this.colConnectionStatus = new System.Windows.Forms.ColumnHeader();
            this.colLocalEndPoint = new System.Windows.Forms.ColumnHeader();
            this.colRemoteEndPoint = new System.Windows.Forms.ColumnHeader();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.2426F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.7574F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtServerStatusLogger, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lvAllConnectedClients, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.72131F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.27869F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 201F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1352, 647);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.21452F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.78548F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel2.Controls.Add(this.lblServerStatus, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblConnectedClients, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblServerIpAddress, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblListenOnPort, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.txtServerStatus, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtConnectedClients, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtServerIpAddress, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtListenOnPort, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnStartServer, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnStopServer, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblWarningPort, 2, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 4);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(483, 134);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServerStatus.Location = new System.Drawing.Point(3, 0);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(130, 32);
            this.lblServerStatus.TabIndex = 0;
            this.lblServerStatus.Text = "Server Status:";
            this.lblServerStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblConnectedClients
            // 
            this.lblConnectedClients.AutoSize = true;
            this.lblConnectedClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConnectedClients.Location = new System.Drawing.Point(3, 32);
            this.lblConnectedClients.Name = "lblConnectedClients";
            this.lblConnectedClients.Size = new System.Drawing.Size(130, 32);
            this.lblConnectedClients.TabIndex = 1;
            this.lblConnectedClients.Text = "Connected Clients:";
            this.lblConnectedClients.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblServerIpAddress
            // 
            this.lblServerIpAddress.AutoSize = true;
            this.lblServerIpAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServerIpAddress.Location = new System.Drawing.Point(3, 64);
            this.lblServerIpAddress.Name = "lblServerIpAddress";
            this.lblServerIpAddress.Size = new System.Drawing.Size(130, 32);
            this.lblServerIpAddress.TabIndex = 2;
            this.lblServerIpAddress.Text = "Server IP Address:";
            this.lblServerIpAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblListenOnPort
            // 
            this.lblListenOnPort.AutoSize = true;
            this.lblListenOnPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblListenOnPort.Location = new System.Drawing.Point(3, 96);
            this.lblListenOnPort.Name = "lblListenOnPort";
            this.lblListenOnPort.Size = new System.Drawing.Size(130, 38);
            this.lblListenOnPort.TabIndex = 3;
            this.lblListenOnPort.Text = "Listen On Port:";
            this.lblListenOnPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtServerStatus
            // 
            this.txtServerStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtServerStatus.Location = new System.Drawing.Point(139, 4);
            this.txtServerStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtServerStatus.Name = "txtServerStatus";
            this.txtServerStatus.ReadOnly = true;
            this.txtServerStatus.Size = new System.Drawing.Size(160, 27);
            this.txtServerStatus.TabIndex = 4;
            this.txtServerStatus.Text = "Stopped";
            this.txtServerStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtConnectedClients
            // 
            this.txtConnectedClients.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConnectedClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConnectedClients.Location = new System.Drawing.Point(139, 36);
            this.txtConnectedClients.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConnectedClients.Name = "txtConnectedClients";
            this.txtConnectedClients.ReadOnly = true;
            this.txtConnectedClients.Size = new System.Drawing.Size(160, 27);
            this.txtConnectedClients.TabIndex = 5;
            this.txtConnectedClients.Text = "0";
            this.txtConnectedClients.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtServerIpAddress
            // 
            this.txtServerIpAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServerIpAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtServerIpAddress.Location = new System.Drawing.Point(139, 68);
            this.txtServerIpAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtServerIpAddress.Name = "txtServerIpAddress";
            this.txtServerIpAddress.ReadOnly = true;
            this.txtServerIpAddress.Size = new System.Drawing.Size(160, 27);
            this.txtServerIpAddress.TabIndex = 6;
            this.txtServerIpAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtListenOnPort
            // 
            this.txtListenOnPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtListenOnPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtListenOnPort.Location = new System.Drawing.Point(139, 100);
            this.txtListenOnPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtListenOnPort.Name = "txtListenOnPort";
            this.txtListenOnPort.Size = new System.Drawing.Size(160, 27);
            this.txtListenOnPort.TabIndex = 7;
            this.txtListenOnPort.Text = "56789";
            this.txtListenOnPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtListenOnPort.TextChanged += new System.EventHandler(this.txtListenOnPort_TextChanged);
            // 
            // btnStartServer
            // 
            this.btnStartServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartServer.Location = new System.Drawing.Point(305, 4);
            this.btnStartServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStartServer.Name = "btnStartServer";
            this.tableLayoutPanel2.SetRowSpan(this.btnStartServer, 2);
            this.btnStartServer.Size = new System.Drawing.Size(85, 56);
            this.btnStartServer.TabIndex = 8;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.BtnStartServer_ClickEvent);
            // 
            // btnStopServer
            // 
            this.btnStopServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStopServer.Location = new System.Drawing.Point(396, 4);
            this.btnStopServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStopServer.Name = "btnStopServer";
            this.tableLayoutPanel2.SetRowSpan(this.btnStopServer, 2);
            this.btnStopServer.Size = new System.Drawing.Size(84, 56);
            this.btnStopServer.TabIndex = 9;
            this.btnStopServer.Text = "Stop Server";
            this.btnStopServer.UseVisualStyleBackColor = true;
            this.btnStopServer.Click += new System.EventHandler(this.BtnStopServer_ClickEvent);
            // 
            // lblWarningPort
            // 
            this.lblWarningPort.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.lblWarningPort, 2);
            this.lblWarningPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWarningPort.Location = new System.Drawing.Point(305, 96);
            this.lblWarningPort.Name = "lblWarningPort";
            this.lblWarningPort.Size = new System.Drawing.Size(175, 38);
            this.lblWarningPort.TabIndex = 10;
            this.lblWarningPort.Text = "lvlWarning";
            // 
            // txtServerStatusLogger
            // 
            this.txtServerStatusLogger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServerStatusLogger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtServerStatusLogger.Location = new System.Drawing.Point(3, 145);
            this.txtServerStatusLogger.Multiline = true;
            this.txtServerStatusLogger.Name = "txtServerStatusLogger";
            this.txtServerStatusLogger.ReadOnly = true;
            this.tableLayoutPanel1.SetRowSpan(this.txtServerStatusLogger, 3);
            this.txtServerStatusLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtServerStatusLogger.Size = new System.Drawing.Size(483, 481);
            this.txtServerStatusLogger.TabIndex = 1;
            // 
            // lvAllConnectedClients
            // 
            this.lvAllConnectedClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colClientUsername,
            this.colClientId,
            this.colConnectionStatus,
            this.colLocalEndPoint,
            this.colRemoteEndPoint});
            this.lvAllConnectedClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAllConnectedClients.Location = new System.Drawing.Point(492, 3);
            this.lvAllConnectedClients.Name = "lvAllConnectedClients";
            this.tableLayoutPanel1.SetRowSpan(this.lvAllConnectedClients, 2);
            this.lvAllConnectedClients.Size = new System.Drawing.Size(857, 192);
            this.lvAllConnectedClients.TabIndex = 2;
            this.lvAllConnectedClients.UseCompatibleStateImageBehavior = false;
            this.lvAllConnectedClients.View = System.Windows.Forms.View.Details;
            this.lvAllConnectedClients.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvAllConnectedClients_DrawItem);
            // 
            // colClientUsername
            // 
            this.colClientUsername.Text = "Client Username";
            this.colClientUsername.Width = 120;
            // 
            // colClientId
            // 
            this.colClientId.Text = "Client ID";
            this.colClientId.Width = 300;
            // 
            // colConnectionStatus
            // 
            this.colConnectionStatus.Text = "Connected";
            this.colConnectionStatus.Width = 100;
            // 
            // colLocalEndPoint
            // 
            this.colLocalEndPoint.Text = "Local EndPoint";
            this.colLocalEndPoint.Width = 160;
            // 
            // colRemoteEndPoint
            // 
            this.colRemoteEndPoint.Text = "Remote Endpoint";
            this.colRemoteEndPoint.Width = 160;
            // 
            // PresentationLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1352, 647);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PresentationLayer";
            this.Text = "ChatServer";
            this.Load += new System.EventHandler(this.WinFormOnLoad_Event);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label lblServerStatus;
        private Label lblConnectedClients;
        private Label lblServerIpAddress;
        private Label lblListenOnPort;
        private TextBox txtServerStatus;
        private TextBox txtConnectedClients;
        private TextBox txtServerIpAddress;
        private TextBox txtListenOnPort;
        private Button btnStartServer;
        private Button btnStopServer;
        private TextBox txtServerStatusLogger;
        private Label lblWarningPort;
        private ListView lvAllConnectedClients;
        private ColumnHeader colClientUsername;
        private ColumnHeader colClientId;
        private ColumnHeader colConnectionStatus;
        private ColumnHeader colLocalEndPoint;
        private ColumnHeader colRemoteEndPoint;
    }
}