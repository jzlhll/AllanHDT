 namespace Hearthstone_Deck_Tracker.AllanAdd
{
    class MyUtils
    {
        public static string translateClass2CN(string s)
        {
            s = s.ToLowerInvariant();
            if (s.Equals("hunter"))
            {
                return "猎人";
            }
            else if (s.Equals("paladin"))
            {
                return "圣骑士";
            }
            else if (s.Equals("priest"))
            {
                return "牧师";
            }
            else if (s.Equals("warrior"))
            {
                return "战士";
            }
            else if (s.Equals("warlock"))
            {
                return "术士";
            }
            else if (s.Equals("druid"))
            {
                return "德鲁伊";
            }
            else if (s.Equals("mage"))
            {
                return "法师";
            }
            else if (s.Equals("shaman"))
            {
                return "萨满";
            }
            else if (s.Equals("rogue"))
            {
                return "潜行者";
            }
            return s;
        }

    }
}
