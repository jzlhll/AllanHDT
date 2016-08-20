using Hearthstone_Deck_Tracker;
using System.Diagnostics;
namespace AllanPlugins
{
    class ChinaWebImport
    {
        public static string getSupportDemo() {
            return "检查链接是否正确？\n支持的格式:"+
                "\n类似：http://ls.duowan.com/s/decksbuilder/standard.html#i3&1uyG-aU-gAcH1dky-CEuxo-3jJ67vY&842&EEEWE&3&18"
                + "\n\n类似:http://db.178.com/hs/deck/#3$909896:2,2924878:2,3314238:2,3871714:1,7195615:1,7487906:1";
        }
        public static bool import(string deckstr, string deckname)
        {
            string converted = "";
            string className = "";
            string[] ss;
            if (deckstr == null)
            {
                deckstr = "";
            }
            if (deckstr.Contains("db.178.com"))
            {
                ss = import178(deckstr);
                if (ss == null)
                {
                    return false;
                }
                converted = ss[0]; //英文
                className = ss[2];
            }
            else if (deckstr.Contains("[hsdeck]"))
            {
                ss = import178hsdeck(deckstr);
                if (ss == null)
                {
                    return false;
                }
                converted = ss[0];
            }
            else if (deckstr.Contains("duowan"))
            {
                ss = importDuowan(deckstr);
                if (ss == null)
                {
                    return false;
                }
                converted = ss[0]; //英文
                className = ss[2];
            }
            else {
                char[] chs = deckstr.ToCharArray();
                int i0 = chs[0] - '0';
                char i1 = chs[1];
                if (i0 >= 1 && i0 <= 9 && i1 == '#')
                { //TODO allan如果添加职业需要修改
                    ss = importMyApkStr(deckstr, i0);
                    converted = ss[0];
                    className = ss[1];
                }
                else {
                    return false;
                }
            }
            
            if (deckname == null) {
                deckname = "";
            }
            var deck = Helper.ParseCardString(converted, false); //默认false
            deck.Name = deckname == "" ? "自定义" + className : deckname;
            Core.MainWindow.SetNewDeck(deck);
            Core.MainWindow.ActivateWindow();
            return true;
        }

        private static string[] importMyApkStr(string deckstr, int heroId) {
            AllanConverter converter = new AllanConverter();
            string[] convertedSS = converter.getMyApkConvertedToEngNames(deckstr, heroId);
            if (convertedSS == null)
            {
                return null;
            }
            converter.release();
            return convertedSS;
        }

        private static string[] importDuowan(string deckstr)
        {
            AllanPlugins.AllanConverter converter = new AllanPlugins.AllanConverter();
            string[] convertedSS = converter.getDuowanConvertedToEngNames(deckstr);
            if (convertedSS == null) {
                return null;
            }
            converter.release();
            return convertedSS;
        }

        private static string[] import178(string deckstr)
        {
            AllanPlugins.AllanConverter converter = new AllanPlugins.AllanConverter();
            string[] convertedSS = converter.get178ConvertedToEngNames(deckstr);
            if (convertedSS == null)
            {
                return null;
            }
            converter.release();
            return convertedSS;
        }

        private static string[] import178hsdeck(string deckstr) {
            AllanPlugins.AllanConverter converter = new AllanPlugins.AllanConverter();
            string[] convertedSS = converter.get178hsdeckConvertedToEngNames(deckstr);
            if (convertedSS == null)
            {
                return null;
            }
            converter.release();
            return convertedSS;
        }
    }
}
