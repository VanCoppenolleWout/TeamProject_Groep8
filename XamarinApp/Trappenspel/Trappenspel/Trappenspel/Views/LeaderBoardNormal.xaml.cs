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
    public partial class LeaderBoardNormal : ContentPage
    {
        public LeaderBoardNormal()
        {
            InitializeComponent();
            LoadContentList();
        }

        private async Task LoadContentList()
        {
            List<LeaderBoard> leaderBoardList = await LeaderBoardRepo.GetLeaderBoardListAsync();

            List<LeaderBoard> normalList = new List<LeaderBoard>();

            foreach (LeaderBoard item in leaderBoardList)
            {
                if (item.difficulty == "normal")
                {
                    normalList.Add(item);
                }
            }

            ContentList.ItemsSource = normalList;
            int i = 1;
            foreach (LeaderBoard item in normalList)
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