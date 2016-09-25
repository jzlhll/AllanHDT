namespace Hearthstone_Deck_Tracker.Enums
{
	public enum GameMode
	{
		[LocDescription("Enum_GameMode_All")]
		All, //for filtering @ deck stats
		[LocDescription("Enum_GameMode_Ranked")]
		Ranked,
		[LocDescription("Enum_GameMode_Casual")]
		Casual,
		[LocDescription("Enum_GameMode_Arena")]
		Arena,
		[LocDescription("Enum_GameMode_Brawl")]
		Brawl,
		[LocDescription("Enum_GameMode_Friendly")]
		Friendly,
		[LocDescription("Enum_GameMode_Practice")]
		Practice,
		[LocDescription("Enum_GameMode_Spectator")]
		Spectator,
		[LocDescription("Enum_GameMode_None")]
		None
	}
<<<<<<< HEAD
    public class GameModeConverter {
        public static string convert(GameMode gm)
        {
            switch (gm)
            {
                case GameMode.All:
                    return "全部";
                case GameMode.Ranked:
                    return "天梯";
                case GameMode.Casual:
                    return "休闲";
                case GameMode.Arena:
                    return "竞技场";
                case GameMode.Brawl:
                    return "乱斗";
                case GameMode.Friendly:
                    return "友谊";
                case GameMode.Practice:
                    return "练习";
                case GameMode.Spectator:
                    return "观众";
                case GameMode.None:
                    return "无";
            }
            return "";
        }

        public static GameMode convert(string str)
        {
            switch (str)
            {
                case "全部":
                    return GameMode.All;
                case "天梯" :
                    return GameMode.Ranked;
                case "休闲":
                    return GameMode.Casual;
                case "竞技场":
                    return GameMode.Arena;
                case "乱斗":
                    return GameMode.Brawl;
                case "友谊":
                    return GameMode.Friendly;
                case "练习":
                    return GameMode.Practice;
                case "观众":
                    return GameMode.Spectator;
                case "无":
                    return GameMode.None;
            }
            return GameMode.None;
        }
    }
}
=======
}
>>>>>>> c693a4c... update code to 0925
