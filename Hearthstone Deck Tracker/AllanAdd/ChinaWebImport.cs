
namespace Hearthstone_Deck_Tracker.AllanAdd
{
    class ChinaWebImport
    {
        public static void import(string deckstr, string deckname)
        {
            string converted = "";
            string className = "";
            string[] ss;
            if (deckstr.Contains("db.178.com"))
            {
                ss = import178(deckstr);
                converted = ss[0]; //英文
                className = ss[2];
            }
            else if (deckstr.Contains("duowan"))
            {
                ss = importDuowan(deckstr);
                converted = ss[0]; //英文
                className = ss[2];
            }
            var deck = Helper.ParseCardString(converted, false); //默认false
            deck.Name = deckname == "" ? "自定义" + className : deckname;
            Core.MainWindow.SetNewDeck(deck);
            Core.MainWindow.ActivateWindow();
        }

        private static string[] importDuowan(string deckstr)
        {
            AllanPlugins.AllanConverter converter = new AllanPlugins.AllanConverter();
            string[] convertedSS = converter.getDuowanConvertedToEngNames(deckstr);
            converter.release();
            return convertedSS;
        }

        private static string[] import178(string deckstr)
        {
            AllanPlugins.AllanConverter converter = new AllanPlugins.AllanConverter();
            string[] convertedSS = converter.get178ConvertedToEngNames(deckstr);
            converter.release();
            return convertedSS;
        }
    }
}
