using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;
using System.Configuration;

namespace ChatRoomServer
{
    public delegate void ServerLoggerDelegate(string serverStatusLog);
    public delegate void ServerStatusDelegate(bool status);
    public delegate void ConnectedClientsCountDelegate(int activeClientsCount);
    public delegate void ConnectedClentsListDelegate(List<ClientInfo> allClients);

    public partial class PresentationLayer : Form
    {
        private IServerManager _serverManager;
        private IInputValidator _inputValidator;
        private IChatRoomManager _chatRoomManager;

        private string _serverRunning;
        private string _serverStopped;
        public PresentationLayer(IServerManager serverManager, IInputValidator inputValidator, IChatRoomManager chatRoomManager)
        {
            InitializeComponent();
            _serverManager = serverManager;
            _inputValidator = inputValidator;
            _chatRoomManager = chatRoomManager;
            _serverRunning = Enum.GetName(typeof(ServerStatus), ServerStatus.Running);
            _serverStopped = Enum.GetName(typeof(ServerStatus), ServerStatus.Stopped);


        }

        #region Event Handlers


        private void WinFormOnLoad_Event(object sender, EventArgs e)
        {
            txtServerIpAddress.Text = _serverManager.GetLocalIP();
            lblWarningPort.Text = string.Empty;
            txtServerStatus.Text = _serverStopped;
            txtServerStatus.BackColor = txtServerStatus.BackColor;
            txtServerStatus.ForeColor = Color.Red;
            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;

            ResolveChatRoomDynamicControl();
        }

        private void txtListenOnPort_TextChanged(object sender, EventArgs e)
        {
            lblWarningPort.Text = string.Empty;
            var filteredText = string.Concat(txtListenOnPort.Text.Where(char.IsDigit));
            txtListenOnPort.Text = filteredText;
        }

        private void lvAllConnectedClients_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            e.Graphics.DrawRectangle(Pens.LightGray, e.Bounds);
        }

        private void BtnStartServer_ClickEvent(object sender, EventArgs e)
        {
            if (!ResolveValidation()) return;
            ServerActivityInfo serverActivityInfo = CreateServerActivityInfo();
            _serverManager.StartServer(serverActivityInfo);
        }

        private void BtnStopServer_ClickEvent(object sender, EventArgs e)
        {
            ServerActivityInfo serverActivityInfo = CreateServerActivityInfo();
            _serverManager.StopServer(serverActivityInfo);
        }

        private void ChatRoomUpdate_Event()
        {
            //Create a table layout panel and add: 
            //ChatRoom Identifier
            //ChatRoom status
            //List of all Active Users
            //List of all invites and their statuses
            //Conversation
            while (true)
            {
                List<ChatRoom> allChatRooms = _chatRoomManager.GetAllCreatedChatRooms();
            }
        }

        #endregion Event Handlers

        #region Callbacks

        private void ServerLoggerReportCallback(string report)
        {
            Action logUpdate = () =>
            {
                txtServerStatusLogger.Text += report;
                txtServerStatusLogger.AppendText(Environment.NewLine);
                txtServerStatusLogger.Refresh();
            };
            txtServerStatusLogger.BeginInvoke(logUpdate);
        }

        private void ServerStatusReportCallback(bool isActive)
        {
            Action action = () =>
            {
                txtServerStatus.Text = (isActive) ? _serverRunning : _serverStopped;
                txtServerStatus.BackColor = txtServerStatus.BackColor;
                txtServerStatus.ForeColor = (isActive) ? Color.Blue : Color.Red;
            };
            txtServerStatus.BeginInvoke(action);

            Action actionBtnStartServer = () =>
            {
                btnStartServer.Enabled = (isActive) ? false : true;
            };
            btnStartServer.BeginInvoke(actionBtnStartServer);

            Action actionBtnStopServer = () =>
            {
                btnStopServer.Enabled = (isActive) ? true : false;
            };

            btnStopServer.BeginInvoke(actionBtnStopServer);
        }

        private void ConnectedClientsCountReportCallback(int activeClientsCount)
        {
            Action action = () =>
            {
                txtConnectedClients.Text = activeClientsCount.ToString();
                txtConnectedClients.Refresh();
            };

            txtConnectedClients.BeginInvoke(action);
        }

        private void ConnectedClientsListReportCallback(List<ClientInfo> allConnectedClients)
        {
            Action actionLvAllConnectedClients = () =>
            {
                lvAllConnectedClients.Items.Clear();
                foreach (ClientInfo clientInfo in allConnectedClients)
                {
                    string[] row = { clientInfo?.Username, clientInfo?.ServerUserID?.ToString(), clientInfo?.tcpClient?.Connected.ToString(), clientInfo?.tcpClient?.Client?.LocalEndPoint?.ToString(), clientInfo?.tcpClient?.Client?.RemoteEndPoint?.ToString() };
                    var rowListViewItem = new ListViewItem(row);
                    lvAllConnectedClients.Items.Add(rowListViewItem);

                }
                lvAllConnectedClients.Refresh();
            };

            lvAllConnectedClients.BeginInvoke(actionLvAllConnectedClients);
        }

        #endregion Callbacks

        #region Private Methods

        private bool ResolveValidation()
        {
            string report = _inputValidator.ValidateServerInputs(txtListenOnPort.Text);
            if (!string.IsNullOrEmpty(report))
            {
                lblWarningPort.Text = report;
                lblWarningPort.BackColor = lblWarningPort.BackColor;
                lblWarningPort.ForeColor = Color.Red;
                return false;
            }
            return true;
        }


        private ServerActivityInfo CreateServerActivityInfo()
        {

            ServerActivityInfo serverActivityInfo = new ServerActivityInfo()
            {
                Port = Int32.Parse(txtListenOnPort.Text),
                ServerLoggerCallback = new ServerLoggerDelegate(ServerLoggerReportCallback),
                ServerStatusCallback = new ServerStatusDelegate(ServerStatusReportCallback),
                ConnectedClientsCountCallback = new ConnectedClientsCountDelegate(ConnectedClientsCountReportCallback),
                ConnectedClientsListCallback = new ConnectedClentsListDelegate(ConnectedClientsListReportCallback)
            };

            return serverActivityInfo;
        }
        #endregion Private Methods


        #region Dynamic Controls

        private List<ChatRoom> GetAllChatRoomsTest()
        {
            List<ChatRoom> allChatRooms = new List<ChatRoom>();
            ChatRoom chatRoom1 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc" }, new ServerUser() { Username = "mno" }, new ServerUser() { Username = "Jonathan" }, new ServerUser() { Username = "Surex" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "TOM" } }, }

            };
            ChatRoom chatRoom2 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc1" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "TOM" } }, }

            };
            ChatRoom chatRoom3 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc2" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "TOM" } }, }

            };
            ChatRoom chatRoom4 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc3" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "TOM" } }, }

            };
            allChatRooms.Add(chatRoom1);
            allChatRooms.Add(chatRoom2);
            allChatRooms.Add(chatRoom3);
            allChatRooms.Add(chatRoom4);
            return allChatRooms;
        }

        public void ResolveChatRoomDynamicControl()
        {

            var chatRooms = GetAllChatRoomsTest();
            TableLayoutPanel tlpCanvas = new TableLayoutPanel();
            tlpCanvas.Dock = DockStyle.Fill;
            tlpCanvas.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tlpCanvas.AutoScroll = true;  //adds horizontal ScrollBar
            tlpCanvas.VerticalScroll.Visible = true;
            tlpCanvas.ColumnCount = 1;
            tlpCanvas.RowCount = 1;
            tlpCanvas.BackColor = Color.White;
            tlpCanvas.SetColumnSpan(tlpCanvas, 3);

            for (var a = 0; a < chatRooms.Count; a++)
            {
                string chatRoomStatus = Enum.GetName(typeof(ChatRoomStatus), chatRooms[a].ChatRoomStatus);
                string chatRoomIdentifier = chatRooms[a].ChatRoomIdentifierNameId;
                string[] allServerUsers = chatRooms[a].AllActiveUsersInChatRoom.Select(a => a.Username).ToArray();
                var tlpRow = new TableLayoutPanel();
                tlpRow.Height = 150;
                tlpRow.BackColor = Color.LightGray;
                tlpRow.Dock = DockStyle.Fill;
                tlpRow.ColumnCount = 3;
                tlpRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 158F));
                tlpRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
                tlpRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));

                tlpRow.RowCount = 6;
                tlpRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                tlpRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                tlpRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                tlpRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                tlpRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                tlpRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));

                tlpRow.Controls.Add(new Label() { Text = "Chat Room Identifier:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
                tlpRow.Controls.Add(new Label() { Text = "Chat Room Status:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
                tlpRow.Controls.Add(new Label() { Text = "Active Users:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 2);
                tlpRow.Controls.Add(new Label() { Text = "", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 3);
                tlpRow.Controls.Add(new Label() { Text = "Invites Statuses:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 4);
                tlpRow.Controls.Add(new Label() { Text = "", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 5);

                tlpRow.Controls.Add(new Label() { Text = chatRoomIdentifier, Enabled = false, Width = 350, BorderStyle = BorderStyle.FixedSingle }, 1, 0);
                tlpRow.Controls.Add(new Label() { Text = chatRoomStatus, Enabled = false, Width = 350, BorderStyle = BorderStyle.FixedSingle }, 1, 1);
                ListBox allActiveUsers = new ListBox() { Enabled = true, Width = 350, BackColor = SystemColors.Control, BorderStyle = BorderStyle.Fixed3D };
                allActiveUsers.Items.AddRange(allServerUsers);
                tlpRow.SetRowSpan(allActiveUsers, 2);
                tlpRow.Controls.Add(allActiveUsers, 1, 2);
                ListBox allInvites = new ListBox() { Enabled = true, Width = 350, BackColor = SystemColors.Control, BorderStyle = BorderStyle.Fixed3D };
                tlpRow.SetRowSpan(allInvites, 2);
                tlpRow.Controls.Add(allInvites, 1, 4);



                tlpCanvas.Controls.Add(tlpRow, 0, a);
            }



            this.tlpChatRoomSection.Controls.Add(tlpCanvas, 0, 1);

        }

        #endregion Dynamic Controls
    }
}