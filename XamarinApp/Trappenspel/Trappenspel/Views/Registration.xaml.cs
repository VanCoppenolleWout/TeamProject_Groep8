using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trappenspel.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registration : ContentPage
    {
        public Registration()
        {
            InitializeComponent();
        }

        private async void publishButton_Clicked(object sender, EventArgs e)
        {
            if (nameEntry.Text == null)
            {
                DisplayAlert("Alert", "please enter name", "ok");
            } else
            {
                var game = new Game
                {
                    Name = nameEntry.Text
                };
                await Navigation.PushAsync(new GameSettings(game));
            }
        }

        private async void publishButton_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LeaderBoardView());
        }
    }
}