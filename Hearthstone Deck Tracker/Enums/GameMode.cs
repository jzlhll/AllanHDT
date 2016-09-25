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
                    return "ȫ��";
                case GameMode.Ranked:
                    return "����";
                case GameMode.Casual:
                    return "����";
                case GameMode.Arena:
                    return "������";
                case GameMode.Brawl:
                    return "�Ҷ�";
                case GameMode.Friendly:
                    return "����";
                case GameMode.Practice:
                    return "��ϰ";
                case GameMode.Spectator:
                    return "����";
                case GameMode.None:
                    return "��";
            }
            return "";
        }

        public static GameMode convert(string str)
        {
            switch (str)
            {
                case "ȫ��":
                    return GameMode.All;
                case "����" :
                    return GameMode.Ranked;
                case "����":
                    return GameMode.Casual;
                case "������":
                    return GameMode.Arena;
                case "�Ҷ�":
                    return GameMode.Brawl;
                case "����":
                    return GameMode.Friendly;
                case "��ϰ":
                    return GameMode.Practice;
                case "����":
                    return GameMode.Spectator;
                case "��":
                    return GameMode.None;
            }
            return GameMode.None;
        }
    }
}
=======
}
>>>>>>> c693a4c... update code to 0925
