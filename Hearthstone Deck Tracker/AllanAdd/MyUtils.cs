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
        public static string translateClass2EN(string s)
        {
            if (s.Equals("猎人"))
            {
                return "Hunter";
            }
            else if (s.Equals("圣骑士"))
            {
                return "Paladin";
            }
            else if (s.Equals("牧师"))
            {
                return "Priest";
            }
            else if (s.Equals("战士"))
            {
                return "Warrior";
            }
            else if (s.Equals("术士"))
            {
                return "Warlock";
            }
            else if (s.Equals("德鲁伊"))
            {
                return "Druid";
            }
            else if (s.Equals("法师"))
            {
                return "Mage";
            }
            else if (s.Equals("萨满"))
            {
                return "Shaman";
            }
            else if (s.Equals("潜行者"))
            {
                return "Rogue";
            }
            return s;
        }
    }
}
