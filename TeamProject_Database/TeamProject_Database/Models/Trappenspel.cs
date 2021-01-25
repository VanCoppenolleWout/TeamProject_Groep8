using System;
using System.Collections.Generic;
using System.Text;

namespace TeamProject_Database.Models
{
    public class Trappenspel
    {
        public int PlayerID { get; set; }
        public string Playername { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public string Difficulty { get; set; }
        public int Steps { get; set; }
        public string Googleid { get; set; }
    }
}
