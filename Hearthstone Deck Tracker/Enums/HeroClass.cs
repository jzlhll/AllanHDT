namespace Hearthstone_Deck_Tracker.Enums
{
<<<<<<< HEAD
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
    public class HeroClassConverter
    {
        public static HeroClass convert(string cl)
        {
            switch (cl)
            {
                case "德鲁伊":
                    return HeroClass.Druid;
                case "猎人":
                    return HeroClass.Hunter;
                case "法师":
                    return HeroClass.Mage;
                case "圣骑士":
                    return HeroClass.Paladin;
                case "牧师":
                    return HeroClass.Priest;
                case "潜行者":
                    return HeroClass.Rogue;
                case "萨满":
                    return HeroClass.Shaman;
                case "术士":
                    return HeroClass.Warlock;
                case "战士":
                    return HeroClass.Warrior;
            }
            return HeroClass.Druid;
        }
        public static string convert(HeroClass cl)
        {
            switch (cl)
            {
                case HeroClass.Druid:
                    return "德鲁伊";
                case HeroClass.Hunter:
                    return "猎人";
                case HeroClass.Mage:
                    return "法师";
                case HeroClass.Paladin:
                    return "圣骑士";
                case HeroClass.Priest:
                    return "牧师";
                case HeroClass.Rogue:
                    return "潜行者";
                case HeroClass.Shaman:
                    return "萨满";
                case HeroClass.Warlock:
                    return "术士";
                case HeroClass.Warrior:
                    return "战士";
            }
            return "德鲁伊";
        }
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

    public class HeroClassAllConverter
    {
        public static HeroClassAll convert(string cl)
        {
            switch (cl)
            {
                case "全部":
                    return HeroClassAll.All;
                case "德鲁伊":
                    return HeroClassAll.Druid;
                case "猎人":
                    return HeroClassAll.Hunter;
                case "法师":
                    return HeroClassAll.Mage;
                case "圣骑士":
                    return HeroClassAll.Paladin;
                case "牧师":
                    return HeroClassAll.Priest;
                case "潜行者":
                    return HeroClassAll.Rogue;
                case "萨满":
                    return HeroClassAll.Shaman;
                case "术士":
                    return HeroClassAll.Warlock;
                case "战士":
                    return HeroClassAll.Warrior;
                case "压缩/存储":
                    return HeroClassAll.Archived;
            }
            return HeroClassAll.All;
        }
        public static string convert(HeroClassAll cl)
        {
            switch (cl)
            {
                case HeroClassAll.All:
                    return "全部";
                case HeroClassAll.Druid:
                    return "德鲁伊";
                case HeroClassAll.Hunter:
                    return "猎人";
                case HeroClassAll.Mage:
                    return "法师";
                case HeroClassAll.Paladin:
                    return "圣骑士";
                case HeroClassAll.Priest:
                    return "牧师";
                case HeroClassAll.Rogue:
                    return "潜行者";
                case HeroClassAll.Shaman:
                    return "萨满";
                case HeroClassAll.Warlock:
                    return "术士";
                case HeroClassAll.Warrior:
                    return "战士";
                case HeroClassAll.Archived:
                    return "压缩/存储";
            }
            return "全部";
        }
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

    public class HeroClassStatsFilterConverter
    {
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
        public static string convert(HeroClassStatsFilter cl)
        {
            switch (cl)
            {
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
=======
	public enum HeroClass
	{
		[LocDescription("Enum_HeroClass_Druid")]
		Druid,
		[LocDescription("Enum_HeroClass_Hunter")]
		Hunter,
		[LocDescription("Enum_HeroClass_Mage")]
		Mage,
		[LocDescription("Enum_HeroClass_Paladin")]
		Paladin,
		[LocDescription("Enum_HeroClass_Priest")]
		Priest,
		[LocDescription("Enum_HeroClass_Rogue")]
		Rogue,
		[LocDescription("Enum_HeroClass_Shaman")]
		Shaman,
		[LocDescription("Enum_HeroClass_Warlock")]
		Warlock,
		[LocDescription("Enum_HeroClass_Warrior")]
		Warrior
	}

	public enum HeroClassAll
	{
		[LocDescription("Enum_HeroClass_All")]
		All,
		[LocDescription("Enum_HeroClass_Druid")]
		Druid,
		[LocDescription("Enum_HeroClass_Hunter")]
		Hunter,
		[LocDescription("Enum_HeroClass_Mage")]
		Mage,
		[LocDescription("Enum_HeroClass_Paladin")]
		Paladin,
		[LocDescription("Enum_HeroClass_Priest")]
		Priest,
		[LocDescription("Enum_HeroClass_Rogue")]
		Rogue,
		[LocDescription("Enum_HeroClass_Shaman")]
		Shaman,
		[LocDescription("Enum_HeroClass_Warlock")]
		Warlock,
		[LocDescription("Enum_HeroClass_Warrior")]
		Warrior,
		[LocDescription("Enum_HeroClass_Archived")]
		Archived
	}

	public enum HeroClassStatsFilter
	{
		[LocDescription("Enum_HeroClass_All")]
		All,
		[LocDescription("Enum_HeroClass_Druid")]
		Druid,
		[LocDescription("Enum_HeroClass_Hunter")]
		Hunter,
		[LocDescription("Enum_HeroClass_Mage")]
		Mage,
		[LocDescription("Enum_HeroClass_Paladin")]
		Paladin,
		[LocDescription("Enum_HeroClass_Priest")]
		Priest,
		[LocDescription("Enum_HeroClass_Rogue")]
		Rogue,
		[LocDescription("Enum_HeroClass_Shaman")]
		Shaman,
		[LocDescription("Enum_HeroClass_Warlock")]
		Warlock,
		[LocDescription("Enum_HeroClass_Warrior")]
		Warrior
	}
}
>>>>>>> c693a4c... update code to 0925
