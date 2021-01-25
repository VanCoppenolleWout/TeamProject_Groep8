using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace Trappenspel.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage1
    {
        

        public TabbedPage1()
        {
            InitializeComponent();

            this.Children.Add(new Registration() { Title = "Trappenspel", IconImageSource="stairs.png"});
            this.Children.Add(new TabbedLeaderboard() { Title = "Leaderboard", IconImageSource="trophy.png"});
            this.Children.Add(new GameSettings() { Title = "Settings", IconImageSource = "settings.png" });

            CurrentPage = Children[0];

            CurrentPageChanged += MainPage_CurrentPageChanged;
            Title = CurrentPage.Title;

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);



            NavigationPage.SetHasBackButton(this, false);
            //NavigationPage.SetHasBackButton(this, true);

        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }


        private void MainPage_CurrentPageChanged(object sender, EventArgs e)
        {
            Title = CurrentPage.Title;
        }


    }
}