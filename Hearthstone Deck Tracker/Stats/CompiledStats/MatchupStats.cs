#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Hearthstone_Deck_Tracker.Enums;

#endregion

namespace Hearthstone_Deck_Tracker.Stats.CompiledStats
{
	public class MatchupStats
	{
		public MatchupStats(string @class, IEnumerable<GameStats> games)
		{
			Class = @class;
            CNClass = translateClass2CN(Class);
            Games = games;
		}

        private string translateClass2CN(string s)
        {
            s = s.ToLowerInvariant();
            if (s.Equals("hunter"))
            {
                return "猎人";
            }
            else if (s.Equals("paladin"))
            {
                return "圣骑士";
            }
            else if (s.Equals("priest"))
            {
                return "牧师";
            }
            else if (s.Equals("warrior"))
            {
                return "战士";
            }
            else if (s.Equals("warlock"))
            {
                return "术士";
            }
            else if (s.Equals("druid"))
            {
                return "德鲁伊";
            }
            else if (s.Equals("mage"))
            {
                return "法师";
            }
            else if (s.Equals("shaman"))
            {
                return "萨满";
            }
            else if (s.Equals("rogue"))
            {
                return "潜行者";
            }
            return s;
        }

        public IEnumerable<GameStats> Games { get; set; }

		public int Wins => Games.Count(x => x.Result == GameResult.Win);

		public int Losses => Games.Count(x => x.Result == GameResult.Loss);

		public double WinRate => (double)Wins / (Wins + Losses);

		public string Class { get; set; }
        public string CNClass { get; }

		public double WinRatePercent => Math.Round(WinRate * 100);

		public SolidColorBrush WinRateTextBrush
		{
			get
			{
				if(double.IsNaN(WinRate) || !Config.Instance.ArenaStatsTextColoring)
					return new SolidColorBrush(Config.Instance.StatsInWindow && Config.Instance.AppTheme != MetroTheme.BaseDark ? Colors.Black : Colors.White);
				return new SolidColorBrush(WinRate >= 0.5 ? Colors.Green : Colors.Red);
			}
		}

		public string Summary => Config.Instance.ConstructedStatsAsPercent ? (double.IsNaN(WinRate) ? "-" : $" {WinRatePercent}%") : $"{Wins} - {Losses}";
	}
}