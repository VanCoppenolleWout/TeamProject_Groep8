using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trappenspel.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameSettings : ContentPage {
        public GameSettings() {
            InitializeComponent();
        }

        private async void publishButton_Clicked(object sender, EventArgs e) {
            try {
                Application.Current.Properties["prefix"] = prefixEntry.Text;
                prefixEntry.Text = Application.Current.Properties["prefix"].ToString();

                await Application.Current.SavePropertiesAsync();
                await DisplayAlert("Alert", "Succesfully Saved", "ok");
            } catch {
                await DisplayAlert("Alert", "Save Failed", "ok");
            }

        }
    }
}