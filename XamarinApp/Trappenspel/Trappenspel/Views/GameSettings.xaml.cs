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
    public partial class GameSettings : ContentPage
    {
        Game game = new Game();
        public GameSettings(Game _game)
        {
            InitializeComponent();
            game = _game;
            nameLabel.Text = "Welcome " + game.Name;
        }

        private async void publishButton_Clicked(object sender, EventArgs e)
        {
            if (stairEntry.Text == null)
            {
                DisplayAlert("Alert", "please enter stair amount", "ok");
            }
            else if (picker.SelectedItem == null)
            {
                DisplayAlert("Alert", "please choose a difficulty", "ok");
            } else
            {
                game.Stairs = int.Parse(stairEntry.Text);
                game.Difficulty = picker.SelectedItem.ToString().ToLower();

                await Navigation.PushAsync(new GameStart(game));
            }
        }
    }
}