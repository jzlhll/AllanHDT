namespace Hearthstone_Deck_Tracker.Enums
{
    public enum HeroClass
    {
        Druid,
        Hunter,
        Mage,
        Paladin,
        Priest,
        Rogue,
        Shaman,
        Warlock,
        Warrior
    }

    public enum HeroClassAll
    {
        All,
        Druid,
        Hunter,
        Mage,
        Paladin,
        Priest,
        Rogue,
        Shaman,
        Warlock,
        Warrior,
        Archived
    }

    public enum HeroClassStatsFilter
    {
        All,
        Druid,
        Hunter,
        Mage,
        Paladin,
        Priest,
        Rogue,
        Shaman,
        Warlock,
        Warrior
    }

    public class HeroClassStatsFilterConverter{
        public static HeroClassStatsFilter convert(string cl)
        {
            switch (cl)
            {
                case "全部":
                    return HeroClassStatsFilter.All;
                case "德鲁伊":
                    return HeroClassStatsFilter.Druid;
                case "猎人":
                    return HeroClassStatsFilter.Hunter;
                case "法师":
                    return HeroClassStatsFilter.Mage;
                case "圣骑士":
                    return HeroClassStatsFilter.Paladin;
                case "牧师":
                    return HeroClassStatsFilter.Priest;
                case "潜行者":
                    return HeroClassStatsFilter.Rogue;
                case "萨满":
                    return HeroClassStatsFilter.Shaman;
                case "术士":
                    return HeroClassStatsFilter.Warlock;
                case "战士":
                    return HeroClassStatsFilter.Warrior;
            }
            return HeroClassStatsFilter.All;
        }
        public static string convert(HeroClassStatsFilter cl) {
            switch (cl) {
                case HeroClassStatsFilter.All:
                    return "全部";
                case HeroClassStatsFilter.Druid:
                    return "德鲁伊";
                case HeroClassStatsFilter.Hunter:
                    return "猎人";
                case HeroClassStatsFilter.Mage:
                    return "法师";
                case HeroClassStatsFilter.Paladin:
                    return "圣骑士";
                case HeroClassStatsFilter.Priest:
                    return "牧师";
                case HeroClassStatsFilter.Rogue:
                    return "潜行者";
                case HeroClassStatsFilter.Shaman:
                    return "萨满";
                case HeroClassStatsFilter.Warlock:
                    return "术士";
                case HeroClassStatsFilter.Warrior:
                    return "战士";
            }
            return "全部";
        }
    }
}