using SVG.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trappenspel.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage {
        public LoginPage() {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            BindingContext = new SVGClass();

        }

        private async void Button_Clicked(object sender, EventArgs e) {
            if (nameEntry.Text == null || String.IsNullOrWhiteSpace(nameEntry.Text)) {
                await DisplayAlert("Oops...", "Je kan geen lege naam invoeren!", "Ok");
            } else if (nameEntry.Text.Length <= 2) {
                await DisplayAlert("Oops...", "Gelieve een naam in te geven met minimaal 2 karakters!", "Ok");
            } else {

                Application.Current.Properties["name"] = nameEntry.Text;
                await Navigation.PopToRootAsync();

            }
        }
    }
}