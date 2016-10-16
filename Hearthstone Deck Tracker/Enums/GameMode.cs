namespace Hearthstone_Deck_Tracker.Enums
{
	public enum GameMode
	{
		All, //for filtering @ deck stats
		Ranked,
		Casual,
		Arena,
		Brawl,
		Friendly,
		Practice,
		Spectator,
		None
	}
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