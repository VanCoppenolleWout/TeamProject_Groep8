using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Trappenspel.Models;
using Trappenspel.Repository;
using System.Diagnostics;

namespace Trappenspel.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderBoardView : ContentPage
    {
        public LeaderBoardView()
        {
            InitializeComponent();
            LoadContentList();
        }

        private async Task LoadContentList()
        {
            List<LeaderBoard> leaderBoardList = await LeaderBoardRepo.GetLeaderBoardListAsync();

            List<LeaderBoard> easyList = new List<LeaderBoard>();
            List<LeaderBoard> normalList = new List<LeaderBoard>();
            List<LeaderBoard> hardList = new List<LeaderBoard>();

            foreach(LeaderBoard item in leaderBoardList)
            {
                if(item.difficulty == "easy")
                {
                    easyList.Add(item);
                } else if(item.difficulty == "normal")
                {
                    normalList.Add(item);
                } else if(item.difficulty == "hard")
                {
                    hardList.Add(item);
                } else
                {
                    continue;
                }
                //switch (item.difficulty)
                //{
                //    case "easy":
                //        easyList.Add(item);
                //        break;
                //    case "normal":
                //        normalList.Add(item);
                //        break;
                //    case "hard":
                //        hardList.Add(item);
                //        break;
                //    default:
                //        break;
                //}
            }
            
            ContentList.ItemsSource = hardList;
            int i = 1;
            foreach(LeaderBoard item in hardList)
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