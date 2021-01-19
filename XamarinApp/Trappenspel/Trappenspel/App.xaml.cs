using System;
using Trappenspel.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Registration());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
