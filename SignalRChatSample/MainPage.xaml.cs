using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SignalRChatSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HubConnection connection;
        private HttpClient httpClient = new HttpClient();

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// SignalR に接続
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnConnectSignalR(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            connection = new HubConnectionBuilder()
           .WithUrl(ConnectURL.Text + "/api")
           .Build();
            connection.On<object>("newMessage", ms =>
            {
                // SignalR からの受信データを表示
                var data = JsonSerializer.Deserialize<SignalRMessage>(ms.ToString());
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ReceiveText.Text += $"{data.sender} : {data.text}" + Environment.NewLine;
                });
            });
            await connection.StartAsync();
        }

        /// <summary>
        /// SignalR に送信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSendText(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var signalr = new SignalRMessage() { sender = "UWP app", text = SendText.Text };
            var ms = JsonSerializer.Serialize(signalr);
            using (var content = new StringContent(ms, Encoding.UTF8, "application/json"))
            {
                await httpClient.PostAsync(new Uri(ConnectURL.Text + "/api/messages"), content);
            }
        }

        /// <summary>
        /// 送受信データ内容
        /// </summary>
        public class SignalRMessage
        {
            public string sender { set; get; }
            public string text { set; get; }
        }
    }
}
