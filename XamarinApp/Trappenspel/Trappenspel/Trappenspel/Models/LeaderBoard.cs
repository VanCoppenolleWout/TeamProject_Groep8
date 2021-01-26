using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trappenspel.Models
{
    public class LeaderBoard
    {
        [JsonProperty(PropertyName = "playerID")]
        public int playerID { get; set; }

        [JsonProperty(PropertyName = "playername")]
        public string playername { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int score { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime date { get; set; }

        [JsonProperty(PropertyName = "difficulty")]
        public string difficulty { get; set; }

        [JsonProperty(PropertyName = "steps")]
        public int steps { get; set; }

        public int rank { get; set; }

        private string myvar;

        public string Score
        {
            get { return "Score: " + score; }
            set { myvar = score.ToString(); }
        }


        //public override string ToString()
        //{
        //    return playername;
        //}

        //public override bool Equals(object obj)
        //{
        //    LeaderBoard temp = (LeaderBoard)obj;

        //    if (this.playerID == temp.playerID && this.playername == temp.playername && this.score == temp.score &&
        //        this.date == temp.date && this.difficulty == temp.difficulty && this.steps == temp.steps)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
