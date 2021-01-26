using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trappenspel.Models;
using Trappenspel.Repository;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Trappenspel.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderBoardHard : ContentPage
    {
        public LeaderBoardHard()
        {
            InitializeComponent();
            LoadContentList();
        }

        private async Task LoadContentList()
        {
            List<LeaderBoard> leaderBoardList = await LeaderBoardRepo.GetLeaderBoardListAsync();

            List<LeaderBoard> hardList = new List<LeaderBoard>();

            foreach (LeaderBoard item in leaderBoardList)
            {
                if (item.difficulty == "hard")
                {
                    hardList.Add(item);
                }
            }

            ContentList.ItemsSource = hardList;
            int i = 1;
            foreach (LeaderBoard item in hardList)
            {
                item.rank = i;
                i++;
            }
        }

        void lvwContentRefreshing(System.Object sender, System.EventArgs e)
        {
            Refresh();
        }

        private async Task Refresh()
        {
            await LoadContentList();
            ContentList.EndRefresh();
        }
    }
}