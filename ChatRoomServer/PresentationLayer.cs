using ChatRoomServer.DomainLayer.Models;
using ChatRoomServer.Utils.Enumerations;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer
{
    public delegate void ServerLoggerDelegate(string serverStatusLog);
    public delegate void ServerStatusDelegate(bool status);
    public delegate void ConnectedClientsDelegate(int activeClients);

    public partial class PresentationLayer : Form
    {
        private IServerManager _serverManager;
        private IInputValidator _inputValidator;

        private string _serverRunning;
        private string _serverStopped;
        public PresentationLayer(IServerManager serverManager ,IInputValidator inputValidator)
        {
            InitializeComponent();
            _serverManager = serverManager;
            _inputValidator = inputValidator;
            _serverRunning = Enum.GetName(typeof(ServerStatus), ServerStatus.Running);
            _serverStopped = Enum.GetName(typeof(ServerStatus) , ServerStatus.Stopped);
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
        }

        private void txtListenOnPort_TextChanged(object sender, EventArgs e)
        {
            lblWarningPort.Text = string.Empty;
            var filteredText = string.Concat(txtListenOnPort.Text.Where(char.IsDigit));
            txtListenOnPort.Text = filteredText;
        }

        private void BtnStartServer_ClickEvent(object sender, EventArgs e)
        {
            if (!ResolveValidation()) return;

            ServerActivationInfo serverActivationInfo = new ServerActivationInfo() 
            { 
                Port = Int32.Parse(txtListenOnPort.Text),
                ServerLoggerCallback = new ServerLoggerDelegate(ServerLoggerReportCallback),
                ServerStatusCallback = new ServerStatusDelegate(ServerStatusReportCallback),
                ConnectedClientsCallback = new ConnectedClientsDelegate(ConnectedClientsReportCallback)
            };
            _serverManager.StartServer(serverActivationInfo);
        }

        private void BtnStopServer_ClickEvent(object sender, EventArgs e)
        {
            ServerActivationInfo serverActivationInfo = new ServerActivationInfo()
            {
                Port = Int32.Parse(txtListenOnPort.Text),
                ServerLoggerCallback = new ServerLoggerDelegate(ServerLoggerReportCallback),
                ServerStatusCallback = new ServerStatusDelegate(ServerStatusReportCallback),
                ConnectedClientsCallback = new ConnectedClientsDelegate(ConnectedClientsReportCallback)
            };
            _serverManager.StopServer(serverActivationInfo);
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
                btnStartServer.Enabled = (isActive ) ? false: true;
            };
            btnStartServer.BeginInvoke(actionBtnStartServer);

            Action actionBtnStopServer = () => 
            {
                btnStopServer.Enabled = (isActive) ? true : false;
            };

            btnStopServer.BeginInvoke(actionBtnStopServer);
        }

        private void ConnectedClientsReportCallback(int activeClientsCount)
        {
            Action action = () => 
            { 
                txtConnectedClients.Text = activeClientsCount.ToString();
                txtConnectedClients.Refresh();
            };

            txtConnectedClients.BeginInvoke(action);
        }

        #endregion Callbacks

        #region Private Methods

        private bool ResolveValidation()
        {
            string report = _inputValidator.ValidateServerInputs(txtListenOnPort.Text);
            if ( !string.IsNullOrEmpty(report))
            {
                lblWarningPort.Text = report;
                lblWarningPort.BackColor = lblWarningPort.BackColor;
                lblWarningPort.ForeColor = Color.Red;
                return false;
            }
            return true;
        }

        #endregion Private Methods
    }
}