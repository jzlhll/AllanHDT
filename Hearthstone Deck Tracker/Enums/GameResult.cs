namespace Hearthstone_Deck_Tracker.Enums
{
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