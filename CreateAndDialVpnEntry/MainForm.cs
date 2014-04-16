using System;
using System.Net;
using System.Windows.Forms;
using DotRas;
using System.Diagnostics;

namespace DotRas.Samples.CreateAndDialVpnEntry
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Defines the name of the entry being used by the example.
        /// </summary>
        public const string EntryName = "VPN Connection";

        /// <summary>
        /// Holds a value containing the handle used by the connection that was dialed.
        /// </summary>
        private RasHandle handle = null;

        public MainForm()
        {
            this.InitializeComponent();

            //var device = RasDevice.GetDeviceByName("(PPPoE)", RasDeviceType.PPPoE);

            //RasEntry.CreateBroadbandEntry("Meu PPPoE", device);
        }

        /// <summary>
        /// Occurs when the user clicks the Create Entry button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void CreateEntryButton_Click(object sender, EventArgs e)
        {
            // This opens the phonebook so it can be used. Different overloads here will determine where the phonebook is opened/created.
            this.AllUsersPhoneBook.Open();

            // Create the entry that will be used by the dialer to dial the connection. Entries can be created manually, however the static methods on
            // the RasEntry class shown below contain default information matching that what is set by Windows for each platform.
            RasEntry entry = RasEntry.CreateVpnEntry(EntryName, IPAddress.Loopback.ToString(), RasVpnStrategy.Default,
                RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn));

            // Add the new entry to the phone book.
            this.AllUsersPhoneBook.Entries.Add(entry);
        }

        /// <summary>
        /// Occurs when the user clicks the Dial button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void DialButton_Click(object sender, EventArgs e)
        {
            this.StatusTextBox.Clear();

            // This button will be used to dial the connection.
            this.Dialer.EntryName = "G2 Telecom";
            this.Dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);

            try
            {
                // Set the credentials the dialer should use.
                this.Dialer.Credentials = new NetworkCredential(txtUsuario.Text, txtSenha.Text);

                // NOTE: The entry MUST be in the phone book before the connection can be dialed.
                // Begin dialing the connection; this will raise events from the dialer instance.
                this.handle = this.Dialer.DialAsync();

                // Enable the disconnect button for use later.
                this.DisconnectButton.Enabled = true;
            }
            catch (Exception ex)
            {
                this.StatusTextBox.AppendText(ex.ToString());
            }
        }

        /// <summary>
        /// Occurs when the dialer state changes during a connection attempt.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="DotRas.StateChangedEventArgs"/> containing event data.</param>
        private void Dialer_StateChanged(object sender, StateChangedEventArgs e)
        {
            string texto = "";

            switch (e.State)
            {
                case RasConnectionState.AllDevicesConnected:
                    texto = "Todos os dispositivos conectados";
                    break;

                case RasConnectionState.ApplySettings:
                    texto = "Aplicando configurações...";
                    break;

                case RasConnectionState.AuthAck:
                    texto = "Ack de autenticação...";
                    break;

                case RasConnectionState.AuthCallback:
                    texto = "Resposta de autenticação";
                    break;

                case RasConnectionState.AuthChangePassword:
                    texto = "Mudar senha";
                    break;

                case RasConnectionState.Authenticate:
                    texto = "Autenticar";
                    break;

                case RasConnectionState.Authenticated:
                    texto = "Autenticado...";
                    break;

                case RasConnectionState.AuthLinkSpeed:
                    texto = "Velocidade do link";
                    break;

                case RasConnectionState.AuthNotify:
                    texto = "Notificação de autenticação";
                    break;

                case RasConnectionState.AuthProject:
                    texto = "Autenticação de projeto";
                    break;

                case RasConnectionState.AuthRetry:
                    texto = "Nova tentativa de autenticação";
                    break;

                case RasConnectionState.CallbackComplete:
                    texto = "Callback concluído";
                    break;

                case RasConnectionState.CallbackSetByCaller:
                    texto = "Callback enviado pelo chamador";
                    break;

                case RasConnectionState.ConnectDevice:
                    texto = "Conectar dispositivo";
                    break;

                case RasConnectionState.Connected:
                    texto = "Conectado";
                    break;

                case RasConnectionState.DeviceConnected:
                    texto = "Dispositivo conectado";
                    break;

                case RasConnectionState.Disconnected:
                    texto = "Desconectado";
                    break;

                case RasConnectionState.Interactive:
                    texto = "Interativo";
                    break;

                case RasConnectionState.InvokeEapUI:
                    texto = "Invocar EAP UI";
                    break;

                case RasConnectionState.LogOnNetwork:
                    texto = "Autenticar na rede";
                    break;

                case RasConnectionState.OpenPort:
                    texto = "Abrir porta";
                    break;

                case RasConnectionState.PasswordExpired:
                    texto = "Senha expirada";
                    break;

                case RasConnectionState.PortOpened:
                    texto = "Porta aberta";
                    break;

                case RasConnectionState.PostCallbackAuthentication:
                    texto = "Autenticação pós-callback";
                    break;

                case RasConnectionState.PrepareForCallback:
                    texto = "Preparar para callback";
                    break;

                case RasConnectionState.Projected:
                    texto = "Projetado";
                    break;

                case RasConnectionState.RetryAuthentication:
                    texto = "Tentar autenticação novamente";
                    break;

                case RasConnectionState.StartAuthentication:
                    texto = "Iniciar autenticação";
                    break;

                case RasConnectionState.SubEntryConnected:
                    texto = "Sub-entrada conectada";
                    break;

                case RasConnectionState.SubEntryDisconnected:
                    texto = "Sub-entrada desconectada";
                    break;

                case RasConnectionState.WaitForCallback:
                    texto = "Aguardar callback...";
                    break;

                case RasConnectionState.WaitForModemReset:
                    texto = "Aguardar reinício do modem...";
                    break;


            }


            this.StatusTextBox.AppendText(texto + "\r\n");
        }

        /// <summary>
        /// Occurs when the dialer has completed a dialing operation.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="DotRas.DialCompletedEventArgs"/> containing event data.</param>
        private void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.StatusTextBox.AppendText("Cancelled!");
            }
            else if (e.TimedOut)
            {
                this.StatusTextBox.AppendText("Connection attempt timed out!");
            }
            else if (e.Error != null)
            {
                this.StatusTextBox.AppendText(e.Error.ToString());
            }
            else if (e.Connected)
            {
                this.StatusTextBox.AppendText("Conectado!");
                Process.Start("http://www.g2telecom.com.br/");
            }

            if (!e.Connected)
            {
                // The connection was not connected, disable the disconnect button.
                this.DisconnectButton.Enabled = false;
            }
        }

        /// <summary>
        /// Occurs when the user clicks the Disconnect button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (this.Dialer.IsBusy)
            {
                // The connection attempt has not been completed, cancel the attempt.
                this.Dialer.DialAsyncCancel();
            }
            else
            {
                // The connection attempt has completed, attempt to find the connection in the active connections.
                RasConnection connection = RasConnection.GetActiveConnectionByHandle(this.handle);
                if (connection != null)
                {
                    // The connection has been found, disconnect it.
                    connection.HangUp();
                }
            }
        }
    }
}
