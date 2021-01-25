using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;
using Trappenspel.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registration : ContentPage {

        IMqttClient client;
        string host = "13.81.105.139";
        int port = 1883;
        string prefix = Application.Current.Properties["prefix"].ToString();

        public Registration() {
            InitializeComponent();
            BindingContext = new SVGClass();
        }

        protected async override void OnAppearing() {
            client = await MqttClient.CreateAsync(host, port);
            await client.ConnectAsync();
        }

        private async void publishButton_Clicked(object sender, EventArgs e) {
            if (stairEntry.Text == null || String.IsNullOrWhiteSpace(stairEntry.Text) || stairEntry.Text.Contains(",")) {
                await DisplayAlert("Oops...", "Gelieve een geldig getal in te vullen", "Ok");
            } else if (Int32.Parse(stairEntry.Text) < 6 || Int32.Parse(stairEntry.Text) % 2 != 0 || Int32.Parse(stairEntry.Text) > 10) {
                await DisplayAlert("Oops...", "Gelieve een getal tussen 6 en 10 in te vullen dat deelbaar is door 2", "Ok");
            } else if (picker.SelectedItem == null) {
                await DisplayAlert("Oops...", "Gelieve een moeilijkheidsgraad in te stellen", "ok");
            } else {

                if (picker.SelectedItem == "Makkelijk") {
                    Application.Current.Properties["difficulty"] = "easy";

                } else if (picker.SelectedItem == "Gemiddeld") {
                    Application.Current.Properties["difficulty"] = "normal";

                } else if (picker.SelectedItem == "Moeilijk") {
                    Application.Current.Properties["difficulty"] = "hard";

                }

                Application.Current.Properties["steps"] = int.Parse(stairEntry.Text);

                var payload_1 = Encoding.UTF8.GetBytes("{\"steps\":" + int.Parse(stairEntry.Text) + "}");
                var stairs = new MqttApplicationMessage(prefix + "quantitysteps", payload_1);
                await client.PublishAsync(stairs, MqttQualityOfService.AtMostOnce);

                await Navigation.PushAsync(new GameStart());
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e) {
            //Application.Current.Properties.Clear();

            Application.Current.Properties["name"] = null;
            Application.Current.Properties["difficulty"] = null;
            Application.Current.Properties["steps"] = null;

            Navigation.PushAsync(new LoginPage());
        }
    }
}
