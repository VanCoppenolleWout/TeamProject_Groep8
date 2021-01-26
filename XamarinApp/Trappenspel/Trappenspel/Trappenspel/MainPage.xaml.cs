using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Trappenspel
{
    public partial class MainPage : ContentPage
    {
        IMqttClient client;
        string host = "13.81.105.139";
        int port = 1883;
        string topic = "teamproject/groep8/quantitysteps";

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            client = await MqttClient.CreateAsync(host, port);
            await client.ConnectAsync();
            await client.SubscribeAsync(topic, MqttQualityOfService.AtMostOnce);
            client.MessageStream.Subscribe(ReceivedMessage);
        }

        void ReceivedMessage(MqttApplicationMessage message)
        {
            var text = Encoding.UTF8.GetString(message.Payload);

            Device.BeginInvokeOnMainThread(() => {
                messageLabel.Text = text;
            });
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var payload = Encoding.UTF8.GetBytes(textEntry.Text);
            var message = new MqttApplicationMessage(topic, payload);
            await client.PublishAsync(message, MqttQualityOfService.AtMostOnce);
        }
    }
}
