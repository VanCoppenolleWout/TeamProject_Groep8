using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedLeaderboard
    {
        public TabbedLeaderboard()
        {
            InitializeComponent();
            this.Children.Add(new LeaderBoardEasy() { Title = "Makkelijk"});
            this.Children.Add(new LeaderBoardNormal() { Title = "Normaal" });

            this.Children.Add(new LeaderBoardHard() { Title = "Moeilijk"});

            CurrentPage = Children[0];
        }
    }
}