using RankingApp.Other;

namespace RankingApp.Models
{
    internal class Structure
    {
        public int Rank { get; set; }
        public string Team { get; set; }
        public string Country { get; set; }
        public int Points { get; set; }
        public Change Change { get; set; }
        public int PositionChange { get; set; }
        public string Panel { get; set; }
    }
}
