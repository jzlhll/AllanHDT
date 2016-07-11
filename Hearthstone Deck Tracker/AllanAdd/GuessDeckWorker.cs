using AllanPlugins;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System.Collections.Generic;
using System.IO;


namespace Hearthstone_Deck_Tracker.AllanAdd
{
    class GuessDeckWorker
    {
        private const string PATH = "AllanBase";
        private const string DECK_FILE_BASE_NAME = "decksBase";
        private const string DECK_FILE_LOCAL_NAME = "decksLocal";

        public static int convertClassToHeroId(string clas) {
            if (clas.Equals("Druid"))
            {
                return 1;
            }
            else if (clas.Equals("Hunter"))
            {
                return 2;
            }
            else if (clas.Equals("Mage"))
            {
                return 3;
            }
            else if (clas.Equals("Paladin"))
            {
                return 4;
            }
            else if (clas.Equals("Priest"))
            {
                return 5;
            }
            else if (clas.Equals("Rogue"))
            {
                return 6;
            }
            else if (clas.Equals("Shaman"))
            {
                return 7;
            }
            else if (clas.Equals("Warlock"))
            {
                return 8;
            }
            else if (clas.Equals("Warrior"))
            {
                return 9;
            }
            return 0;
        }

        public static string convertHeroIdToClass(int heroId) {
            if (heroId == 1) {
                return "Druid";
            } else if (heroId == 2) {
                return "Hunter";
            }
            else if (heroId == 3)
            {
                return "Mage";
            }
            else if (heroId == 4)
            {
                return "Paladin";
            }
            else if (heroId == 5)
            {
                return "Priest";
            }
            else if (heroId == 6)
            {
                return "Rogue";
            }
            else if (heroId == 7)
            {
                return "Shaman";
            }
            else if (heroId == 8)
            {
                return "Warlock";
            }
            else if (heroId == 9)
            {
                return "Warrior";
            }
            return "";
        }

        private const string DeckVersion = "0.0.1";

        public class DeckStruct {
            public string deck { get; set; }
            public string convertedDeck { get; set; }
            public int heroId { get; set; }
            public string title { get; set; }
            public string toString() {
                return heroId + "<>" + title + "<>" + deck + "\r\nconvertedDeck=\r\n" + convertedDeck;
            }
        }

        public List<DeckStruct> mDeckList = new List<DeckStruct>() ;

        public void release() {
            mDeckList.Clear();
            mDeckList = null;
        }

        public GuessDeckWorker() {
            loadDecks(DECK_FILE_BASE_NAME);
            loadDecks(DECK_FILE_LOCAL_NAME);
        }

        public void saveALocalDeck(DeckStruct deck) {
            mDeckList.Add(deck); //先添加到了这个列表中。
            var file = Path.Combine(PATH, DECK_FILE_LOCAL_NAME);
            bool isNew = false;
            if (!File.Exists(file)) {
                File.Create(file);
                isNew = true;
            }
            //convert to my type
            string nums = "";
            string[] ss = deck.convertedDeck.Split('\n');
            CardTool ct = new CardTool();
            string bs = "";
            int i = 0;
            foreach (var s in ss) {
                if (s.Contains("x 2")) {
                    bs = s.Replace(" x 2", "");
                    int id = ct.getDuowanIdByEnName(bs);
                    nums += id + "," + id;
                } else {
                    nums += ct.getDuowanIdByEnName(bs);
                }
                if (i != ss.Length - 1) {
                    nums += ",";
                }
                i++;
            }
            ct.release();
            ct = null;
            string line = deck.heroId + "<>" + "opponent" + deck.heroId + "<>" + nums;
            Log.Info("guess write line " + line);
            using (var sink = new StreamWriter(file, true, System.Text.Encoding.UTF8))
            {
                if (isNew) {
                    sink.WriteLine("###");
                }
                sink.WriteLine(line);
                sink.Close();
                sink.Dispose();
            }
        }

        private void loadDecks(string FILENAME) {
            var file = Path.Combine(PATH, DECK_FILE_BASE_NAME);
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file,System.Text.Encoding.UTF8))
                {
                    string str;
                    string[] stringSeparators = new string[] { "<>" };
                    CardTool ct = new CardTool();
                    while ((str = sr.ReadLine()) != null)
                    {
                        if (str.Contains("##")) {
                            continue;
                        }
                        if (str.Contains("#end#")) {
                            break;
                        }
                        DeckStruct adeck = new DeckStruct();
                        string[] sArray = str.Split(stringSeparators, System.StringSplitOptions.RemoveEmptyEntries);
                        adeck.heroId = int.Parse(sArray[0]);
                        adeck.title = sArray[1];
                        adeck.deck = sArray[2];
                        sArray = null;
                        sArray = adeck.deck.Split(',');
                        int len = sArray.Length;

                        for (int i= 0;i < len; i++) {
                            if (i == len - 1)
                            {
                                adeck.convertedDeck += ct.getCardByDuowanId(int.Parse(sArray[i])).enCard;
                            }
                            else {
                                if (sArray[i].Equals(sArray[i + 1]))
                                {
                                    adeck.convertedDeck += ct.getCardByDuowanId(int.Parse(sArray[i])).enCard + " x 2";
                                    i++;
                                }
                                else {
                                    adeck.convertedDeck += ct.getCardByDuowanId(int.Parse(sArray[i])).enCard;
                                }

                                if (i != len - 2)
                                {
                                    adeck.convertedDeck += "\r\n";
                                }                                    
                            }
                        }
                        Log.Info(adeck.toString());
                        mDeckList.Add(adeck);
                    }
                    ct.release();
                    ct = null;
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

    }
}
