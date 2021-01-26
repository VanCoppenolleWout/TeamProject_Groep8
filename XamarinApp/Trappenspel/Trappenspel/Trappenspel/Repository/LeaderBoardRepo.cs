using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Trappenspel.Models;

namespace Trappenspel.Repository
{
    public class LeaderBoardRepo
    {
        private static async Task<HttpClient> GetClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            return httpClient;
        }

        public static async Task<List<LeaderBoard>> GetLeaderBoardListAsync()
        {
            using (HttpClient client = await GetClient())
            {
                string url = "https://trappenspel-api.azurewebsites.net/api/leaderboard";
                string json = await client.GetStringAsync(url);

                if (json != null)
                {
                    List<LeaderBoard> leaderboardList = new List<LeaderBoard>();
                    leaderboardList = JsonConvert.DeserializeObject<List<LeaderBoard>>(json);
                    return leaderboardList;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
