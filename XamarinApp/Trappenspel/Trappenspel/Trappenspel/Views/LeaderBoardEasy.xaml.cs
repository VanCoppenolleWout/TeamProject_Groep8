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
    public partial class LeaderBoardEasy : ContentPage
    {
        public LeaderBoardEasy()
        {
            InitializeComponent();
            LoadContentList();
        }

        private async Task LoadContentList()
        {
            List<LeaderBoard> leaderBoardList = await LeaderBoardRepo.GetLeaderBoardListAsync();

            List<LeaderBoard> easyList = new List<LeaderBoard>();

            foreach (LeaderBoard item in leaderBoardList)
            {
                if (item.difficulty == "easy")
                {
                    easyList.Add(item);
                }
            }

            ContentList.ItemsSource = easyList;
            int i = 1;
            foreach (LeaderBoard item in easyList)
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