using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;
using Trappenspel.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Trappenspel.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameStart : ContentPage
    {
        IMqttClient client;
        string host = "13.81.105.139";
        int port = 1883;
        string prefix = "gilianledoux/groep8/";

        Game game = new Game();
        bool running = false;

        public GameStart(Game _game)
        {
            InitializeComponent();
            game = _game;
        }

        protected async override void OnAppearing()
        {
            client = await MqttClient.CreateAsync(host, port);
            await client.ConnectAsync();
            await client.SubscribeAsync(prefix + "quantitysteps", MqttQualityOfService.AtMostOnce);
            await client.SubscribeAsync(prefix + "gamestart", MqttQualityOfService.AtMostOnce);
            await client.SubscribeAsync(prefix + "gamestop" , MqttQualityOfService.AtMostOnce);
            client.MessageStream.Subscribe(ReceivedMessage);
        }

        void ReceivedMessage(MqttApplicationMessage message)
        {
            var text = Encoding.UTF8.GetString(message.Payload);

            Device.BeginInvokeOnMainThread(() => {
                //messageLabel.Text = text;
                Debug.WriteLine(text);
            });
        }

        void ReceivedScore(MqttApplicationMessage message)
        {
            string payload = Encoding.UTF8.GetString(message.Payload);
            Score score = new Score();

            score = JsonConvert.DeserializeObject<Score>(payload);

            Device.BeginInvokeOnMainThread(() => {
                scoreLabel.Text = score.score;
            });
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (running == false) {
                var payload_1 = Encoding.UTF8.GetBytes("{\"steps\":" + game.Stairs + "}");
                var stairs = new MqttApplicationMessage(prefix + "quantitysteps", payload_1);
                await client.PublishAsync(stairs, MqttQualityOfService.AtMostOnce);

                var payload_2 = Encoding.UTF8.GetBytes("{\"name\":\"" + game.Name + "\",\"difficulty\":\"" + game.Difficulty + "\"}");
                var message = new MqttApplicationMessage(prefix + "gamestart", payload_2);
                await client.PublishAsync(message, MqttQualityOfService.AtMostOnce);

                homeButton.IsEnabled = false;

                messageLabel.Text = "Stop Game";
                publishButton.Text = "Stop";
                running = true;

                await client.SubscribeAsync(prefix + "game/answer", MqttQualityOfService.AtMostOnce);
                client.MessageStream.Subscribe(ReceivedScore);

            } else if (running == true) {
                var payload_1 = Encoding.UTF8.GetBytes("{\"game\":" + false + "}");
                var stairs = new MqttApplicationMessage(prefix + "gamestop", payload_1);
                await client.PublishAsync(stairs, MqttQualityOfService.AtMostOnce);

                homeButton.IsEnabled = true;

                messageLabel.Text = "Start Game";
                publishButton.Text = "Start";
                running = false;
            }

        }

        private async void homeButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registration());
        }
    }
}