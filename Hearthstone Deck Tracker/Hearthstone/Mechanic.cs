namespace Hearthstone_Deck_Tracker.Hearthstone
{
	public class Mechanic
	{
		public Mechanic(string name, Deck deck)
		{
			Name = name;
			Count = deck.GetMechanicCount(name);
		}

		public string Name { get; }
		public int Count { get; }

		public string DisplayValue => $"{getDisplayValueCN(Name)}: {Count}";

        private string getDisplayValueCN(string name) {
            if (name == "Battlecry") {
                return "Õ½ºð";
            } else if (name == "Charge") {
                return "³å·æ";
            } else if (name == "Combo")
            {
                return "Á¬»÷";
            }
            else if (name == "Deathrattle")
            {
                return "ÍöÓï";
            }
            else if (name == "Divine Shield")
            {
                return "Ê¥¶Ü";
            }
            else if (name == "Freeze")
            {
                return "¶³½á";
            }
            else if (name == "Inspire")
            {
                return "¼¤Àø";
            }
            else if (name == "Secret")
            {
                return "°ÂÃØ";
            }
            else if (name == "Taunt")
            {
                return "¶³½á";
            }
            else if (name == "Spellpower")
            {
                return "·¨Ç¿";
            }
            else if (name == "Windfury")
            {
                return "·çÅ­";
            }
            return name;
        }
	}
}