#region

using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Enums;

#endregion

namespace Hearthstone_Deck_Tracker.HearthStats.API
{
	public class Dictionaries
	{
		public static readonly Dictionary<int, GameResult> GameResultDict = new Dictionary<int, GameResult>
		{
			{1, GameResult.胜},
			{2, GameResult.败},
			{3, GameResult.弃}
		};

		public static readonly Dictionary<int, GameMode> GameModeDict = new Dictionary<int, GameMode>
		{
			{1, GameMode.竞技场},
			{2, GameMode.休闲},
			{3, GameMode.天梯},
			{4, GameMode.无}, //Tournament
			{5, GameMode.好友}
		};

		public static readonly Dictionary<int, string> HeroDict = new Dictionary<int, string>
		{
			{1, "Druid"},
			{2, "Hunter"},
			{3, "Mage"},
			{4, "Paladin"},
			{5, "Priest"},
			{6, "Rogue"},
			{7, "Shaman"},
			{8, "Warlock"},
			{9, "Warrior"}
		};
	}
}
