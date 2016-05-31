#region

using System;
using System.Collections.Generic;
using System.Globalization;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Enums;

#endregion

namespace Hearthstone_Deck_Tracker.Hearthstone
{
	public static class HearthDbConverter
	{
		public static readonly Dictionary<int, string> SetDict = new Dictionary<int, string>
		{
			{0, null},
			{2, Core.BASIC},
			{3, Core.CLASSIC},
			{4, "Reward"},
			{5, "Missions"},
			{7, "System"},
			{8, "Debug"},
			{11, Core.PROMOTION},
			{12, Core.CURSE_OF_NAXXRAMAS},
			{13, Core.GOBLINS_VS_GNOMES},
			{14, Core.BLACKROCK_MOUNTAIN},
			{15, Core.THE_GRAND_TOURNAMENT},
			{16, "Credits"},
			{17, "Hero Skins"},
			{18, "Tavern Brawl"},
			{20, Core.LEAGUE_OF_EXPLORERS},
			{21, Core.WHISPERS_OF_THE_OLD_GODS}
		};

		public static string ConvertClass(CardClass cardClass) => (int)cardClass < 2 || (int)cardClass > 10
																	  ? null : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cardClass.ToString().ToLowerInvariant());

		public static string CardTypeConverter(CardType type) => type == CardType.HERO_POWER ? "Hero Power" : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(type.ToString().ToLowerInvariant().Replace("_", ""));


		public static string RaceConverter(Race race)
		{
			switch(race)
			{
				case Race.INVALID:
					return null;
				case Race.GOBLIN2:
					return "Goblin";
				case Race.PET:
					return "Beast";
				default:
					return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(race.ToString().ToLowerInvariant());
			}
		}

		public static string SetConverter(CardSet set)
		{
			string str;
			SetDict.TryGetValue((int)set, out str);
			return str;
		}

		public static GameMode GetGameMode(GameType gameType)
		{
			switch(gameType)
			{
				case GameType.GT_VS_AI:
					return GameMode.Practice;
				case GameType.GT_VS_FRIEND:
					return GameMode.Friendly;
				case GameType.GT_ARENA:
					return GameMode.Arena;
				case GameType.GT_RANKED:
					return GameMode.Ranked;
				case GameType.GT_UNRANKED:
					return GameMode.Casual;
				case GameType.GT_TAVERNBRAWL:
				case GameType.GT_TB_2P_COOP:
					return GameMode.Brawl;
				default:
					return GameMode.None;
			}
		}
	}
}