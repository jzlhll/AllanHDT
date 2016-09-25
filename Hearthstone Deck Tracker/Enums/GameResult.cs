namespace Hearthstone_Deck_Tracker.Enums
{
<<<<<<< HEAD
    public enum GameResult
    {
        None,
        Win,
        Loss,
        Draw
    }

    public class GameResultConvert {
        public static string convert(GameResult res)
        {
            switch (res)
            {
                case GameResult.None:
                    return "��";
                case GameResult.Win:
                    return "ʤ��";
                case GameResult.Loss:
                    return "�ܱ�";
                case GameResult.Draw:
                    return "����";
            }
            return "��";
        }
        public static GameResult convert(string res)
        {
            switch (res)
            {
                case "��":
                    return GameResult.None;
                case "ʤ��":
                    return GameResult.Win;
                case "�ܱ�":
                    return GameResult.Loss;
                case "����":
                    return GameResult.Draw;
            }
            return GameResult.None;
        }
    }

    public enum GameResultAll
    {
        All,
        Win,
        Loss,
        Draw
    }

    public class GameResultAllConverter {
        public static string convert(GameResultAll res) {
            switch (res) {
                case GameResultAll.All:
                    return "ȫ��";
                case GameResultAll.Win:
                    return "ʤ��";
                case GameResultAll.Loss:
                    return "�ܱ�";
                case GameResultAll.Draw:
                    return "����";
            }
            return "ȫ��";
        }
        public static GameResultAll convert(string res)
        {
            switch (res)
            {
                case "ȫ��":
                    return GameResultAll.All;
                case "ʤ��" :
                    return GameResultAll.Win;
                case "�ܱ�":
                    return GameResultAll.Loss;
                case "����":
                    return GameResultAll.Draw;
            }
            return GameResultAll.All;
        }
    }
}
=======
	public enum GameResult
	{
		[LocDescription("Enum_GameResult_None")]
		None,
		[LocDescription("Enum_GameResult_Win")]
		Win,
		[LocDescription("Enum_GameResult_Loss")]
		Loss,
		[LocDescription("Enum_GameResult_Draw")]
		Draw
	}

	public enum GameResultAll
	{
		[LocDescription("Enum_GameResult_All")]
		All,
		[LocDescription("Enum_GameResult_Win")]
		Win,
		[LocDescription("Enum_GameResult_Loss")]
		Loss,
		[LocDescription("Enum_GameResult_Draw")]
		Draw
	}
}
>>>>>>> c693a4c... update code to 0925
