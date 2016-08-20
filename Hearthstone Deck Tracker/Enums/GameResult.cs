namespace Hearthstone_Deck_Tracker.Enums
{
    public enum GameResult
    {
        None,
        Win,
        Loss,
        Draw
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
                    return "失败";
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
                case "失败":
                    return GameResultAll.Loss;
                case "弃局":
                    return GameResultAll.Draw;
            }
            return GameResultAll.All;
        }
    }
}