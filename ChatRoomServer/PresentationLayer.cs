using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Extensions;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer
{
    public delegate void ServerLoggerDelegate(string serverStatusLog);
    public delegate void ServerStatusDelegate(bool status);
    public delegate void ConnectedClientsCountDelegate(int activeClientsCount);
    public delegate void ConnectedClentsListDelegate(List<ClientInfo> allClients);
    public delegate void ChatRoomsUpdateDelegate(List<ControlChatRoom> allChatRooms);

    public partial class PresentationLayer : Form
    {
        private IServerManager _serverManager;
        private IInputValidator _inputValidator;
        private IChatRoomManager _chatRoomManager;

        private string _serverRunning;
        private string _serverStopped;
        TableLayoutPanel _tlpChatRoomCanvas;
        public PresentationLayer(IServerManager serverManager, IInputValidator inputValidator, IChatRoomManager chatRoomManager)
        {
            InitializeComponent();
            _serverManager = serverManager;
            _inputValidator = inputValidator;
            _chatRoomManager = chatRoomManager;
            _serverRunning = Enum.GetName(typeof(ServerStatus), ServerStatus.Running);
            _serverStopped = Enum.GetName(typeof(ServerStatus), ServerStatus.Stopped);

            _tlpChatRoomCanvas = new TableLayoutPanel();
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

            CreateChatroomCanvasDynamicControl();
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

        private void ChatRoomUpdate_ThreadCallback(List<ControlChatRoom> allChatRooms)
        {
            Thread threadChatRoomUpdateEvent = new Thread(() =>
            {
                if (allChatRooms.Count >= 0)
                {
                    //We modify the controls Create, update, Delete
                    List<ControlChatRoom> allControlChatRoomPendingResolution = allChatRooms.Where(a => a.ControlActionType != ControlActionType.Read).ToList();
                    foreach (ControlChatRoom controlChatRoom in allControlChatRoomPendingResolution)
                    {
                        ResolveChatRoomDynamicControl(controlChatRoom);
                    }
                    //we delete items form the actual list of items
                    List<ControlChatRoom> allChatRoomsForDeletion = allChatRooms.Where(a => a.ControlActionType == ControlActionType.Delete).ToList();
                    allChatRooms.RemoveAllExtension(allChatRoomsForDeletion);

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
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "Mark" }, InviteStatus = InviteStatus.SentAndPendingResponse } }

            };
            ChatRoom chatRoom3 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc2" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "Pam" }, InviteStatus = InviteStatus.SentAndPendingResponse }, }

            };
            ChatRoom chatRoom4 = new ChatRoom()
            {
                ChatRoomIdentifierNameId = "abc-111",
                ChatRoomStatus = ChatRoomStatus.OpenActive,
                AllActiveUsersInChatRoom = new List<ServerUser> { new ServerUser() { Username = "abc3" }, new ServerUser() { Username = "mno" } },
                AllInvitesSentToGuestUsers = new List<Invite> { new Invite() { GuestServerUser = new ServerUser() { Username = "Jack" }, InviteStatus = InviteStatus.SentAndPendingResponse }, }

            };
            allChatRooms.Add(chatRoom1);
            allChatRooms.Add(chatRoom2);
            allChatRooms.Add(chatRoom3);
            allChatRooms.Add(chatRoom4);
            return allChatRooms;
        }

        private void CreateChatroomCanvasDynamicControl()
        {
            _tlpChatRoomCanvas.Dock = DockStyle.Fill;
            _tlpChatRoomCanvas.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _tlpChatRoomCanvas.ColumnCount = 1;
            _tlpChatRoomCanvas.RowCount = 1;
            _tlpChatRoomCanvas.BackColor = Color.White;
            _tlpChatRoomCanvas.SetColumnSpan(_tlpChatRoomCanvas, 3);

            _tlpChatRoomCanvas.HorizontalScroll.Maximum = 0;
            _tlpChatRoomCanvas.HorizontalScroll.Visible = false;
            _tlpChatRoomCanvas.AutoScroll = false;
            _tlpChatRoomCanvas.VerticalScroll.Visible = true;
            _tlpChatRoomCanvas.AutoScroll = true;
            this.tlpChatRoomSection.Controls.Add(_tlpChatRoomCanvas, 0, 1);

        }
        public void ResolveChatRoomDynamicControl(ControlChatRoom controlChatRoom)
        {
            Action actionUpdate = () =>
            {
                string chatRoomId = controlChatRoom.ChatRoomObject.ChatRoomId.ToString();
                string chatRoomStatus = Enum.GetName(typeof(ChatRoomStatus), controlChatRoom.ChatRoomObject.ChatRoomStatus);
                string chatRoomIdentifier = controlChatRoom.ChatRoomObject.ChatRoomIdentifierNameId;
                string[] allServerUsers = controlChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Select(a => a.Username).ToArray();
                string conversationRecord = controlChatRoom.ChatRoomObject.ConversationRecord;
                string[] allInvitesStatusesArray = new string[controlChatRoom.ChatRoomObject.AllInvitesSentToGuestUsers.Count];

                for (var i = 0; i < controlChatRoom.ChatRoomObject.AllInvitesSentToGuestUsers.Count; i++)
                {
                    allInvitesStatusesArray[i] = controlChatRoom.ChatRoomObject.AllInvitesSentToGuestUsers[i].GuestServerUser.Username + "_" + Enum.GetName(typeof(InviteStatus), controlChatRoom.ChatRoomObject.AllInvitesSentToGuestUsers[i].InviteStatus);
                }
                int canvasWidth = (this.tlpChatRoomSection.Width - 25);

                string lblAllServerUsersCount = "lblAllServerUsersCount";
                string lablAllInvitesStatusesCount = "lablAllInvitesStatusesCount";
                string lblChatRoomStatus = "lblChatRoomStatus";
                string listBoxAllActiveUsers = "listBoxAllActiveUsers";
                string listBoxAllInvites = "listBoxAllInvites";
                string textBoxConversation = "textBoxConversation";

                switch (controlChatRoom.ControlActionType)
                {
                    case ControlActionType.Create:

                        var tlpNewRow = new TableLayoutPanel();
                        tlpNewRow.Name = chatRoomId;
                        tlpNewRow.Height = 150;
                        tlpNewRow.Width = canvasWidth;
                        tlpNewRow.BackColor = Color.LightGray;

                        tlpNewRow.HorizontalScroll.Maximum = 0;
                        tlpNewRow.HorizontalScroll.Visible = false;
                        tlpNewRow.VerticalScroll.Visible = false;
                        tlpNewRow.AutoScroll = false;

                        tlpNewRow.ColumnCount = 3;
                        tlpNewRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 158F));
                        tlpNewRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
                        tlpNewRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));

                        tlpNewRow.RowCount = 6;
                        tlpNewRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                        tlpNewRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                        tlpNewRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                        tlpNewRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                        tlpNewRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                        tlpNewRow.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));

                        tlpNewRow.Controls.Add(new Label() { Text = "Chat Room Identifier:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
                        tlpNewRow.Controls.Add(new Label() { Text = "Chat Room Status:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
                        tlpNewRow.Controls.Add(new Label() { Text = "Active Users:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 2);
                        tlpNewRow.Controls.Add(new Label() { Text = allServerUsers.Length.ToString(), Name = lblAllServerUsersCount, BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleCenter }, 0, 3);
                        tlpNewRow.Controls.Add(new Label() { Text = "Invites Statuses:", BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleRight }, 0, 4);
                        tlpNewRow.Controls.Add(new Label() { Text = allInvitesStatusesArray.Length.ToString(), Name = lablAllInvitesStatusesCount, BorderStyle = BorderStyle.None, Width = 150, TextAlign = ContentAlignment.MiddleCenter }, 0, 5);

                        tlpNewRow.Controls.Add(new Label() { Text = chatRoomIdentifier, Enabled = false, Width = 350, BorderStyle = BorderStyle.FixedSingle }, 1, 0);
                        tlpNewRow.Controls.Add(new Label() { Text = chatRoomStatus, Name = lblChatRoomStatus, Enabled = false, Width = 350, BorderStyle = BorderStyle.FixedSingle }, 1, 1);

                        ListBox allActiveUsers = new ListBox() { Enabled = true, Width = 350, BackColor = SystemColors.Control, BorderStyle = BorderStyle.Fixed3D };
                        allActiveUsers.Name = listBoxAllActiveUsers;
                        allActiveUsers.Items.AddRange(allServerUsers);
                        tlpNewRow.SetRowSpan(allActiveUsers, 2);
                        tlpNewRow.Controls.Add(allActiveUsers, 1, 2);

                        ListBox allInvites = new ListBox() { Enabled = true, Width = 350, BackColor = SystemColors.Control, BorderStyle = BorderStyle.Fixed3D };
                        allInvites.Name = listBoxAllInvites;
                        allInvites.Items.AddRange(allInvitesStatusesArray);
                        tlpNewRow.SetRowSpan(allInvites, 2);
                        tlpNewRow.Controls.Add(allInvites, 1, 4);

                        TextBox conversation = new TextBox() { Text = conversationRecord, Multiline = true, Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, ScrollBars = ScrollBars.Vertical, Enabled = false };
                        conversation.Name = textBoxConversation;
                        conversation.ScrollBars = ScrollBars.Vertical;
                        tlpNewRow.SetRowSpan(conversation, 6);
                        tlpNewRow.Controls.Add(conversation, 3, 0);
                        _tlpChatRoomCanvas.SetColumnSpan(tlpNewRow, 3);

                        int lastRowIndex = _tlpChatRoomCanvas.Controls.Count;
                        _tlpChatRoomCanvas.Controls.Add(tlpNewRow, 0, lastRowIndex);
                        controlChatRoom.ControlActionType = ControlActionType.Read;

                        break;

                    case ControlActionType.Update:

                        string controlChatRoomIdForUpdate = controlChatRoom.ChatRoomObject.ChatRoomId.ToString();
                        string allActiveUsersCount = controlChatRoom.ChatRoomObject.AllActiveUsersInChatRoom.Count.ToString();
                        TableLayoutPanel tlpControlForUpdate = null;
                        foreach (var control in this._tlpChatRoomCanvas.Controls)
                        {
                            if (control is TableLayoutPanel)
                            {
                                TableLayoutPanel tlpControl = (TableLayoutPanel)control;
                                string tlpControlId = tlpControl.Name;
                                if (tlpControlId == controlChatRoomIdForUpdate)
                                {
                                    tlpControlForUpdate = tlpControl;
                                    break;
                                }
                            }
                        }

                        if (tlpControlForUpdate != null)
                        {
                            foreach (var control in tlpControlForUpdate.Controls)
                            {
                                if (control is Label)
                                {
                                    Label lblControl = (Label)control;
                                    if (lblControl.Name == lblAllServerUsersCount)
                                    {
                                        lblControl.Text = allServerUsers.Length.ToString();
                                    }

                                    if (lblControl.Name == lablAllInvitesStatusesCount)
                                    {
                                        lblControl.Text = allInvitesStatusesArray.Length.ToString();
                                    }

                                    if (lblControl.Name == lblChatRoomStatus)
                                    {
                                        lblControl.Text = chatRoomStatus;
                                    }
                                }

                                if (control is ListBox)
                                {
                                    ListBox lbControl = (ListBox)control;
                                    if (lbControl.Name == listBoxAllActiveUsers)
                                    {
                                        lbControl.Items.Clear();
                                        lbControl.Items.AddRange(allServerUsers);
                                    }

                                    if (lbControl.Name == listBoxAllInvites)
                                    {
                                        lbControl.Items.Clear();
                                        lbControl.Items.AddRange(allInvitesStatusesArray);
                                    }
                                }

                                if (control is TextBox)
                                {
                                    TextBox txtControl = (TextBox)control;
                                    if (txtControl.Name == textBoxConversation)
                                    {
                                        txtControl.Text = conversationRecord;
                                    }
                                }
                            }
                        }
                        controlChatRoom.ControlActionType = ControlActionType.Read;
                        break;

                    case ControlActionType.Delete:
                        string controlChatRoomIdForDeletion = controlChatRoom.ChatRoomObject.ChatRoomId.ToString();
                        TableLayoutPanel tlpControlForRemoval = null;
                        foreach (var control in this._tlpChatRoomCanvas.Controls)
                        {
                            if (control is TableLayoutPanel)
                            {
                                TableLayoutPanel tlpControl = (TableLayoutPanel)control;
                                string tlpControlId = tlpControl.Name;
                                if (tlpControlId == controlChatRoomIdForDeletion)
                                {
                                    tlpControlForRemoval = tlpControl;
                                    break;
                                }
                            }
                        }
                        if (tlpControlForRemoval != null)
                        {
                            this._tlpChatRoomCanvas.Controls.Remove(tlpControlForRemoval);
                        }

                        break;

                }

            };
            this.tlpChatRoomSection.BeginInvoke(actionUpdate);
        }

        #endregion Dynamic Controls
    }
}