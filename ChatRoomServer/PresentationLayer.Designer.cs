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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            lblServerStatus = new Label();
            lblConnectedClients = new Label();
            lblServerIpAddress = new Label();
            lblListenOnPort = new Label();
            txtServerStatus = new TextBox();
            txtConnectedClients = new TextBox();
            txtServerIpAddress = new TextBox();
            txtListenOnPort = new TextBox();
            btnStartServer = new Button();
            btnStopServer = new Button();
            lblWarningPort = new Label();
            txtServerStatusLogger = new TextBox();
            lvAllConnectedClients = new ListView();
            colClientUsername = new ColumnHeader();
            colClientId = new ColumnHeader();
            colConnectionStatus = new ColumnHeader();
            colLocalEndPoint = new ColumnHeader();
            colRemoteEndPoint = new ColumnHeader();
            tlpChatRoomSection = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36.2426F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 63.7574F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(txtServerStatusLogger, 0, 1);
            tableLayoutPanel1.Controls.Add(lvAllConnectedClients, 1, 0);
            tableLayoutPanel1.Controls.Add(tlpChatRoomSection, 1, 2);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 71.72131F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 28.27869F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 151F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 172F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 13F));
            tableLayoutPanel1.Size = new Size(1189, 485);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45.21452F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54.78548F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 79F));
            tableLayoutPanel2.Controls.Add(lblServerStatus, 0, 0);
            tableLayoutPanel2.Controls.Add(lblConnectedClients, 0, 1);
            tableLayoutPanel2.Controls.Add(lblServerIpAddress, 0, 2);
            tableLayoutPanel2.Controls.Add(lblListenOnPort, 0, 3);
            tableLayoutPanel2.Controls.Add(txtServerStatus, 1, 0);
            tableLayoutPanel2.Controls.Add(txtConnectedClients, 1, 1);
            tableLayoutPanel2.Controls.Add(txtServerIpAddress, 1, 2);
            tableLayoutPanel2.Controls.Add(txtListenOnPort, 1, 3);
            tableLayoutPanel2.Controls.Add(btnStartServer, 2, 0);
            tableLayoutPanel2.Controls.Add(btnStopServer, 3, 0);
            tableLayoutPanel2.Controls.Add(lblWarningPort, 2, 3);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tableLayoutPanel2.Size = new Size(424, 100);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // lblServerStatus
            // 
            lblServerStatus.AutoSize = true;
            lblServerStatus.Dock = DockStyle.Fill;
            lblServerStatus.Location = new Point(3, 0);
            lblServerStatus.Name = "lblServerStatus";
            lblServerStatus.Size = new Size(113, 24);
            lblServerStatus.TabIndex = 0;
            lblServerStatus.Text = "Server Status:";
            lblServerStatus.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblConnectedClients
            // 
            lblConnectedClients.AutoSize = true;
            lblConnectedClients.Dock = DockStyle.Fill;
            lblConnectedClients.Location = new Point(3, 24);
            lblConnectedClients.Name = "lblConnectedClients";
            lblConnectedClients.Size = new Size(113, 24);
            lblConnectedClients.TabIndex = 1;
            lblConnectedClients.Text = "Connected Clients:";
            lblConnectedClients.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblServerIpAddress
            // 
            lblServerIpAddress.AutoSize = true;
            lblServerIpAddress.Dock = DockStyle.Fill;
            lblServerIpAddress.Location = new Point(3, 48);
            lblServerIpAddress.Name = "lblServerIpAddress";
            lblServerIpAddress.Size = new Size(113, 24);
            lblServerIpAddress.TabIndex = 2;
            lblServerIpAddress.Text = "Server IP Address:";
            lblServerIpAddress.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblListenOnPort
            // 
            lblListenOnPort.AutoSize = true;
            lblListenOnPort.Dock = DockStyle.Fill;
            lblListenOnPort.Location = new Point(3, 72);
            lblListenOnPort.Name = "lblListenOnPort";
            lblListenOnPort.Size = new Size(113, 28);
            lblListenOnPort.TabIndex = 3;
            lblListenOnPort.Text = "Listen On Port:";
            lblListenOnPort.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtServerStatus
            // 
            txtServerStatus.BorderStyle = BorderStyle.FixedSingle;
            txtServerStatus.Dock = DockStyle.Fill;
            txtServerStatus.Location = new Point(122, 3);
            txtServerStatus.Name = "txtServerStatus";
            txtServerStatus.ReadOnly = true;
            txtServerStatus.Size = new Size(139, 23);
            txtServerStatus.TabIndex = 4;
            txtServerStatus.Text = "Stopped";
            txtServerStatus.TextAlign = HorizontalAlignment.Center;
            // 
            // txtConnectedClients
            // 
            txtConnectedClients.BorderStyle = BorderStyle.FixedSingle;
            txtConnectedClients.Dock = DockStyle.Fill;
            txtConnectedClients.Location = new Point(122, 27);
            txtConnectedClients.Name = "txtConnectedClients";
            txtConnectedClients.ReadOnly = true;
            txtConnectedClients.Size = new Size(139, 23);
            txtConnectedClients.TabIndex = 5;
            txtConnectedClients.Text = "0";
            txtConnectedClients.TextAlign = HorizontalAlignment.Center;
            // 
            // txtServerIpAddress
            // 
            txtServerIpAddress.BorderStyle = BorderStyle.FixedSingle;
            txtServerIpAddress.Dock = DockStyle.Fill;
            txtServerIpAddress.Location = new Point(122, 51);
            txtServerIpAddress.Name = "txtServerIpAddress";
            txtServerIpAddress.ReadOnly = true;
            txtServerIpAddress.Size = new Size(139, 23);
            txtServerIpAddress.TabIndex = 6;
            txtServerIpAddress.TextAlign = HorizontalAlignment.Center;
            // 
            // txtListenOnPort
            // 
            txtListenOnPort.BorderStyle = BorderStyle.FixedSingle;
            txtListenOnPort.Dock = DockStyle.Fill;
            txtListenOnPort.Location = new Point(122, 75);
            txtListenOnPort.Name = "txtListenOnPort";
            txtListenOnPort.Size = new Size(139, 23);
            txtListenOnPort.TabIndex = 7;
            txtListenOnPort.Text = "56789";
            txtListenOnPort.TextAlign = HorizontalAlignment.Center;
            txtListenOnPort.TextChanged += txtListenOnPort_TextChanged;
            // 
            // btnStartServer
            // 
            btnStartServer.Dock = DockStyle.Fill;
            btnStartServer.Location = new Point(267, 3);
            btnStartServer.Name = "btnStartServer";
            tableLayoutPanel2.SetRowSpan(btnStartServer, 2);
            btnStartServer.Size = new Size(74, 42);
            btnStartServer.TabIndex = 8;
            btnStartServer.Text = "Start Server";
            btnStartServer.UseVisualStyleBackColor = true;
            btnStartServer.Click += BtnStartServer_ClickEvent;
            // 
            // btnStopServer
            // 
            btnStopServer.Dock = DockStyle.Fill;
            btnStopServer.Location = new Point(347, 3);
            btnStopServer.Name = "btnStopServer";
            tableLayoutPanel2.SetRowSpan(btnStopServer, 2);
            btnStopServer.Size = new Size(74, 42);
            btnStopServer.TabIndex = 9;
            btnStopServer.Text = "Stop Server";
            btnStopServer.UseVisualStyleBackColor = true;
            btnStopServer.Click += BtnStopServer_ClickEvent;
            // 
            // lblWarningPort
            // 
            lblWarningPort.AutoSize = true;
            tableLayoutPanel2.SetColumnSpan(lblWarningPort, 2);
            lblWarningPort.Dock = DockStyle.Fill;
            lblWarningPort.Location = new Point(267, 72);
            lblWarningPort.Name = "lblWarningPort";
            lblWarningPort.Size = new Size(154, 28);
            lblWarningPort.TabIndex = 10;
            lblWarningPort.Text = "lvlWarning";
            // 
            // txtServerStatusLogger
            // 
            txtServerStatusLogger.BorderStyle = BorderStyle.FixedSingle;
            txtServerStatusLogger.Dock = DockStyle.Fill;
            txtServerStatusLogger.Location = new Point(3, 108);
            txtServerStatusLogger.Margin = new Padding(3, 2, 3, 2);
            txtServerStatusLogger.Multiline = true;
            txtServerStatusLogger.Name = "txtServerStatusLogger";
            txtServerStatusLogger.ReadOnly = true;
            tableLayoutPanel1.SetRowSpan(txtServerStatusLogger, 3);
            txtServerStatusLogger.ScrollBars = ScrollBars.Vertical;
            txtServerStatusLogger.Size = new Size(424, 361);
            txtServerStatusLogger.TabIndex = 1;
            // 
            // lvAllConnectedClients
            // 
            lvAllConnectedClients.Columns.AddRange(new ColumnHeader[] { colClientUsername, colClientId, colConnectionStatus, colLocalEndPoint, colRemoteEndPoint });
            lvAllConnectedClients.Dock = DockStyle.Fill;
            lvAllConnectedClients.Location = new Point(433, 2);
            lvAllConnectedClients.Margin = new Padding(3, 2, 3, 2);
            lvAllConnectedClients.Name = "lvAllConnectedClients";
            tableLayoutPanel1.SetRowSpan(lvAllConnectedClients, 2);
            lvAllConnectedClients.Size = new Size(753, 144);
            lvAllConnectedClients.TabIndex = 2;
            lvAllConnectedClients.UseCompatibleStateImageBehavior = false;
            lvAllConnectedClients.View = View.Details;
            lvAllConnectedClients.DrawItem += lvAllConnectedClients_DrawItem;
            // 
            // colClientUsername
            // 
            colClientUsername.Text = "Client Username";
            colClientUsername.Width = 120;
            // 
            // colClientId
            // 
            colClientId.Text = "Client ID";
            colClientId.Width = 300;
            // 
            // colConnectionStatus
            // 
            colConnectionStatus.Text = "Connected";
            colConnectionStatus.Width = 100;
            // 
            // colLocalEndPoint
            // 
            colLocalEndPoint.Text = "Local EndPoint";
            colLocalEndPoint.Width = 160;
            // 
            // colRemoteEndPoint
            // 
            colRemoteEndPoint.Text = "Remote Endpoint";
            colRemoteEndPoint.Width = 160;
            // 
            // tlpChatRoomSection
            // 
            tlpChatRoomSection.AllowDrop = true;
            tlpChatRoomSection.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tlpChatRoomSection.ColumnCount = 3;
            tlpChatRoomSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpChatRoomSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpChatRoomSection.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 246F));
            tlpChatRoomSection.Location = new Point(433, 150);
            tlpChatRoomSection.Margin = new Padding(3, 2, 3, 2);
            tlpChatRoomSection.Name = "tlpChatRoomSection";
            tlpChatRoomSection.RowCount = 2;
            tableLayoutPanel1.SetRowSpan(tlpChatRoomSection, 2);
            tlpChatRoomSection.RowStyles.Add(new RowStyle(SizeType.Percent, 8.705882F));
            tlpChatRoomSection.RowStyles.Add(new RowStyle(SizeType.Percent, 91.29412F));
            tlpChatRoomSection.Size = new Size(753, 319);
            tlpChatRoomSection.TabIndex = 3;
            // 
            // PresentationLayer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1189, 485);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PresentationLayer";
            Text = "ChatServer";
            Load += WinFormOnLoad_Event;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
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
        private TableLayoutPanel tlpChatRoomSection;
    }
}