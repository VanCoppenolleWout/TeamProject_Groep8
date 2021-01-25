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

namespace Trappenspel.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameStart : ContentPage {
        GameDetails gamedetails = new GameDetails();

        IMqttClient client;
        string host = "13.81.105.139";
        int port = 1883;
        string prefix = Application.Current.Properties["prefix"].ToString();
        //string prefix = "gilianledoux/groep8/";

        string name = Application.Current.Properties["name"].ToString();
        string difficulty = Application.Current.Properties["difficulty"].ToString();
        string steps = Application.Current.Properties["steps"].ToString();

        public GameStart() {
            InitializeComponent();
            BindingContext = new SVGClass();
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override bool OnBackButtonPressed() {
            return true;
        }

        protected async override void OnAppearing() {
            client = await MqttClient.CreateAsync(host, port);
            await client.ConnectAsync();

            await client.SubscribeAsync(prefix + "game/answer", MqttQualityOfService.AtMostOnce);
            client.MessageStream.Subscribe(ReceivedMessage);
        }

        void ReceivedMessage(MqttApplicationMessage message) {
            var json = Encoding.UTF8.GetString(message.Payload);
            var myObject = JsonConvert.DeserializeObject<GameDetails>(json);

            Device.BeginInvokeOnMainThread(() => {
                handleGame(myObject);
            });
        }

        public void handleGame(GameDetails myObject) {
            if (myObject.game == true) {
                
                buttonStart.Text = "Stop";

                if (myObject.name == name) {
                    homeButton.IsEnabled = false;
                    buttonStart.IsEnabled = true;


                } else if (myObject.name != name) {
                    homeButton.IsEnabled = true;
                    buttonStart.IsEnabled = false;

                }

                if (myObject.seconds > 0) {
                    startGame.Text = "Spel wordt gestart in: " + myObject.seconds;
                    scoreGame.Text = "";
                } else if (myObject.seconds == 0) {
                    startGame.Text = "Spel gestart door: " + myObject.name;
                    scoreGame.Text = "Huidige score: " + myObject.score.ToString();
                }
            } else if (myObject.game == false) {
                startGame.Text = "Spel geëindigd door: " + myObject.name;
                scoreGame.Text = "Eindscore: " + myObject.score;
                buttonStart.Text = "Start";
                buttonStart.IsEnabled = true;
                homeButton.IsEnabled = true;
            }
        }

        private async void homeButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopToRootAsync();
        }

        private async void buttonStart_Clicked(object sender, EventArgs e) {
            if (buttonStart.Text == "Start") {
                var payload_1 = Encoding.UTF8.GetBytes("{\"steps\":" + steps + "}");
                var stairs = new MqttApplicationMessage(prefix + "quantitysteps", payload_1);
                await client.PublishAsync(stairs, MqttQualityOfService.AtMostOnce);

                var payload_2 = Encoding.UTF8.GetBytes("{\"name\":\"" + name + "\",\"difficulty\":\"" + difficulty + "\"}");
                var message = new MqttApplicationMessage(prefix + "gamestart", payload_2);
                await client.PublishAsync(message, MqttQualityOfService.AtMostOnce);
            } else if (buttonStart.Text == "Stop") {

                var payload_1 = Encoding.UTF8.GetBytes("{\"game\":" + false + "}");
                var stairs = new MqttApplicationMessage(prefix + "gamestop", payload_1);
                await client.PublishAsync(stairs, MqttQualityOfService.AtMostOnce);
            }
        }
    }
}