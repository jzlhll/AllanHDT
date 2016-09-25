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
                    return "无";
                case GameResult.Win:
                    return "胜利";
                case GameResult.Loss:
                    return "败北";
                case GameResult.Draw:
                    return "弃局";
            }
            return "无";
        }
        public static GameResult convert(string res)
        {
            switch (res)
            {
                case "无":
                    return GameResult.None;
                case "胜利":
                    return GameResult.Win;
                case "败北":
                    return GameResult.Loss;
                case "弃局":
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
                    return "全部";
                case GameResultAll.Win:
                    return "胜利";
                case GameResultAll.Loss:
                    return "败北";
                case GameResultAll.Draw:
                    return "弃局";
            }
            return "全部";
        }
        public static GameResultAll convert(string res)
        {
            switch (res)
            {
                case "全部":
                    return GameResultAll.All;
                case "胜利" :
                    return GameResultAll.Win;
                case "败北":
                    return GameResultAll.Loss;
                case "弃局":
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
