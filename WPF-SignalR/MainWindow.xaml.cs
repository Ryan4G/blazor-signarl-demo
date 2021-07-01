using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_SignalR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string userName;
        private string group;

        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:9080/ChatHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            group = $"GROUP{new Random().Next(0, 2)}";
            userName = $"User:{new Random().Next(1000, 5000)}({group})";
            txtUser.Text = userName;

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}\r\n";
                    rtxMessage.AppendText(newMessage);
                    rtxMessage.ScrollToEnd();
                });
            });
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.StartAsync();
                rtxMessage.AppendText("Connection started \r\n");

                await connection.InvokeAsync("AddToGroup",
                    userName, group);

                btnConnect.IsEnabled = false;
                btnDisConnect.IsEnabled =
                btnSendMsg.IsEnabled =
                btnSendToSystem.IsEnabled =
                btnSendToGroup.IsEnabled = true;
            }
            catch (Exception ex)
            {
                rtxMessage.AppendText(ex.Message);
            }

            rtxMessage.ScrollToEnd();
        }

        private async void btnSendMsg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendMessage",
                    userName, txtSendMsg.Text);
            }
            catch (Exception ex)
            {
                rtxMessage.AppendText(ex.Message + "\r\n");
            }

            rtxMessage.ScrollToEnd();
        }

        private async void btnDisConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("RemoveFromGroup",
                    userName, group);

                await connection.StopAsync();
                rtxMessage.AppendText("Connection stoped \r\n");
                btnConnect.IsEnabled = true;
                btnDisConnect.IsEnabled = 
                btnSendMsg.IsEnabled =
                btnSendToSystem.IsEnabled =
                btnSendToGroup.IsEnabled = false;
            }
            catch (Exception ex)
            {
                rtxMessage.AppendText(ex.Message);
            }

            rtxMessage.ScrollToEnd();
        }

        private async void btnSendToGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendToGroupMessage",
                    userName, txtSendMsg.Text, group);
            }
            catch (Exception ex)
            {
                rtxMessage.AppendText(ex.Message + "\r\n");
            }

            rtxMessage.ScrollToEnd();
        }

        private async void btnSendToSystem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendToCallerMessage",
                    userName, txtSendMsg.Text);
            }
            catch (Exception ex)
            {
                rtxMessage.AppendText(ex.Message + "\r\n");
            }

            rtxMessage.ScrollToEnd();
        }
    }
}
