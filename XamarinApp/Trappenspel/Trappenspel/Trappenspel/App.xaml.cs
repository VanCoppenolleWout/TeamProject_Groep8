using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using Trappenspel.Models;
using Trappenspel.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel {

    public partial class App : Application {
        IMqttClient client;
        string host = "13.81.105.139";
        int port = 1883;
        //string prefix = App.Current.Properties["prefix"].ToString();

        public App() {
            if (Application.Current.Properties.ContainsKey("prefix")) {
                InitializeComponent();
                MainPage = new NavigationPage(new TabbedPage1());
                checkCache();
            } else {

                App.Current.Properties.Add("prefix", "kobemarchal/groep8/");
                App.Current.SavePropertiesAsync();

                InitializeComponent();
                MainPage = new NavigationPage(new TabbedPage1());
                checkCache();
            }
        }

        void checkCache() {
            try {
                // er is al een naam opgeslagen
                string name = Xamarin.Forms.Application.Current.Properties["name"].ToString();
                //await Navigation.PushModalAsync(new TabbedPage1());
                //Application.Current.MainPage = new NavigationPage(new TabbedPage1());
                //this.Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
            } catch {
                MainPage.Navigation.PushAsync(new LoginPage());

            }
        }

        void ReceivedMessage(MqttApplicationMessage message) {
            var json = Encoding.UTF8.GetString(message.Payload);

            try {
                var myObject = JsonConvert.DeserializeObject<GameStarted>(json);
                if (message.Topic == App.Current.Properties["prefix"].ToString() + "gamestarted/answer") {
                    handleGameStarted(myObject);
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        void handleGameStarted(GameStarted myObject) {
            if (myObject.gamestarted == true) {
                var page = App.Current.MainPage;
                string currentPage = page.Navigation.NavigationStack.Last().ToString();

                if (currentPage != "Trappenspel.Views.GameStart") {
                    Device.BeginInvokeOnMainThread(async () => {
                        await MainPage.Navigation.PushAsync(new GameStart());
                    });
                }
            }
        }

        protected async override void OnStart() {
            client = await MqttClient.CreateAsync(host, port);
            await client.ConnectAsync();
            await client.SubscribeAsync(App.Current.Properties["prefix"].ToString() + "gamestarted/answer", MqttQualityOfService.AtMostOnce);
            client.MessageStream.Subscribe(ReceivedMessage);
        }

        protected override void OnSleep() {
        }

        protected async override void OnResume() {
            OnStart();
        }
    }
}
