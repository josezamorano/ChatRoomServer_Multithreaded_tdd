using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer
{
    public delegate void ServerLoggerDelegate(string serverStatusLog);
    public delegate void ServerStatusDelegate(bool status);
    public delegate void ConnectedClientsCountDelegate(int activeClientsCount);
    public delegate void ConnectedClentsListDelegate(List<ClientInfo> allClients);
    public delegate void ChatRoomsUpdateDelegate(List<ChatRoom> allChatRooms);

    public partial class PresentationLayer : Form
    {
        private IServerManager _serverManager;
        private IInputValidator _inputValidator;
        private IChatRoomManager _chatRoomManager;

        private string _serverRunning;
        private string _serverStopped;
        TableLayoutPanel _tlpCanvas;
        public PresentationLayer(IServerManager serverManager, IInputValidator inputValidator, IChatRoomManager chatRoomManager)
        {
            InitializeComponent();
            _serverManager = serverManager;
            _inputValidator = inputValidator;
            _chatRoomManager = chatRoomManager;
            _serverRunning = Enum.GetName(typeof(ServerStatus), ServerStatus.Running);
            _serverStopped = Enum.GetName(typeof(ServerStatus), ServerStatus.Stopped);

            _tlpCanvas = new TableLayoutPanel();
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

            ChatRoomsUpdateDelegate chatRoomUpdateCallback = new ChatRoomsUpdateDelegate(ChatRoomUpdate_ThreadCallback);
            _chatRoomManager.SetChatRoomUpdateCallback(chatRoomUpdateCallback);
            //var chatRooms = GetAllChatRoomsTest();
            //ChatRoomUpdate_ThreadCallback(chatRooms);
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
                    string[] row = { clientInfo?.Username, clientInfo?.ServerUserID?.ToString(), clientInfo?.TcpClient?.Connected.ToString(), clientInfo?.TcpClient?.Client?.LocalEndPoint?.ToString(), clientInfo?.TcpClient?.Client?.RemoteEndPoint?.ToString() };
                    var rowListViewItem = new ListViewItem(row);
                    lvAllConnectedClients.Items.Add(rowListViewItem);

                }
                lvAllConnectedClients.Refresh();
            };

            lvAllConnectedClients.BeginInvoke(actionLvAllConnectedClients);
        }

        private void ChatRoomUpdate_ThreadCallback(List<ChatRoom> allChatRooms)
        {
            Thread threadChatRoomUpdateEvent = new Thread(() =>
            {
                if (allChatRooms.Count > 0)
                {
                    ResolveChatRoomDynamicControl(allChatRooms);

                }
            });
            threadChatRoomUpdateEvent.Name = "threadChatRoomUpdateEvent";
            threadChatRoomUpdateEvent.IsBackground = true;
            threadChatRoomUpdateEvent.Start();

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
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "TOM" }, InviteStatus = InviteStatus.Accepted }, }

            };
            ChatRoom chatRoom2 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc1" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "Mark" }, InviteStatus = InviteStatus.SentPendingResponse } }

            };
            ChatRoom chatRoom3 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc2" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "Pam" }, InviteStatus = InviteStatus.SentPendingResponse }, }

            };
            ChatRoom chatRoom4 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc3" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "Jack" }, InviteStatus = InviteStatus.SentPendingResponse }, }

            };
            allChatRooms.Add(chatRoom1);
            allChatRooms.Add(chatRoom2);
            allChatRooms.Add(chatRoom3);
            allChatRooms.Add(chatRoom4);
            return allChatRooms;
        }

        public void ResolveChatRoomDynamicControl(List<ChatRoom> chatRooms)
        {
            Action actionUpdate = () =>
            {
                if (_tlpCanvas.Controls.Count > 0)
                {
                    _tlpCanvas.Controls.Clear();
                }

                int canvasWidth = (this.tlpChatRoomSection.Width - 25);
                _tlpCanvas.Dock = DockStyle.Fill;
                _tlpCanvas.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                _tlpCanvas.ColumnCount = 1;
                _tlpCanvas.RowCount = 1;
                _tlpCanvas.BackColor = Color.White;
                _tlpCanvas.SetColumnSpan(_tlpCanvas, 3);

                _tlpCanvas.HorizontalScroll.Maximum = 0;
                _tlpCanvas.HorizontalScroll.Visible = false;
                _tlpCanvas.AutoScroll = false;
                _tlpCanvas.VerticalScroll.Visible = true;
                _tlpCanvas.AutoScroll = true;
                _tlpCanvas.Refresh();
                for (var a = 0; a < chatRooms.Count; a++)
                {
                    string chatRoomStatus = Enum.GetName(typeof(ChatRoomStatus), chatRooms[a].ChatRoomStatus);
                    string chatRoomIdentifier = chatRooms[a].ChatRoomIdentifierNameId;
                    string[] allServerUsers = chatRooms[a].AllActiveUsersInChatRoom.Select(a => a.Username).ToArray();
                    string[] allInvitesStatusesArray = new string[chatRooms[a].AllInvitesSentToGuestUsers.Count];
                    for (var i = 0; i < chatRooms[a].AllInvitesSentToGuestUsers.Count; i++)
                    {
                        allInvitesStatusesArray[i] = chatRooms[a].AllInvitesSentToGuestUsers[i].GuestServerUser.Username + "_" + Enum.GetName(typeof(InviteStatus), chatRooms[a].AllInvitesSentToGuestUsers[i].InviteStatus);
                    }

                    var tlpRow = new TableLayoutPanel();
                    tlpRow.Height = 150;
                    tlpRow.Width = canvasWidth;
                    tlpRow.BackColor = Color.LightGray;

                    tlpRow.HorizontalScroll.Maximum = 0;
                    tlpRow.HorizontalScroll.Visible = false;
                    tlpRow.VerticalScroll.Visible = false;
                    tlpRow.AutoScroll = false;

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
                    tlpRow.Controls.Add(new Label() { Text = allServerUsers.Length.ToString(), BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleCenter }, 0, 3);
                    tlpRow.Controls.Add(new Label() { Text = "Invites Statuses:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 4);
                    tlpRow.Controls.Add(new Label() { Text = allInvitesStatusesArray.Length.ToString(), BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleCenter }, 0, 5);

                    tlpRow.Controls.Add(new Label() { Text = chatRoomIdentifier, Enabled = false, Width = 350, BorderStyle = BorderStyle.FixedSingle }, 1, 0);
                    tlpRow.Controls.Add(new Label() { Text = chatRoomStatus, Enabled = false, Width = 350, BorderStyle = BorderStyle.FixedSingle }, 1, 1);
                    ListBox allActiveUsers = new ListBox() { Enabled = true, Width = 350, BackColor = SystemColors.Control, BorderStyle = BorderStyle.Fixed3D };
                    allActiveUsers.Items.AddRange(allServerUsers);
                    tlpRow.SetRowSpan(allActiveUsers, 2);
                    tlpRow.Controls.Add(allActiveUsers, 1, 2);
                    ListBox allInvites = new ListBox() { Enabled = true, Width = 350, BackColor = SystemColors.Control, BorderStyle = BorderStyle.Fixed3D };
                    allInvites.Items.AddRange(allInvitesStatusesArray);
                    tlpRow.SetRowSpan(allInvites, 2);
                    tlpRow.Controls.Add(allInvites, 1, 4);

                    TextBox conversation = new TextBox() { Multiline = true, Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, ScrollBars = ScrollBars.Vertical, Enabled = false };
                    conversation.ScrollBars = ScrollBars.Vertical;
                    tlpRow.SetRowSpan(conversation, 6);
                    tlpRow.Controls.Add(conversation, 3, 0);
                    _tlpCanvas.SetColumnSpan(tlpRow, 3);
                    _tlpCanvas.Controls.Add(tlpRow, 0, a);
                }
                this.tlpChatRoomSection.Controls.Add(_tlpCanvas, 0, 1);

            };
            this.tlpChatRoomSection.BeginInvoke(actionUpdate);
        }

        #endregion Dynamic Controls
    }
}