namespace Hearthstone_Deck_Tracker.Enums
{
	public enum Region
	{
<<<<<<< HEAD
        [Description("未知")]
        UNKNOWN = 0,
        [Description("美国")]
        US = 1,
        [Description("欧洲")]
        EU = 2,
        [Description("亚洲")]
        ASIA = 3,
        [Description("中国")]
        CHINA = 5
=======
		[LocDescription("Enum_Region_Unknown")]
		UNKNOWN = 0,
		[LocDescription("Enum_Region_US")]
		US = 1,
		[LocDescription("Enum_Region_EU")]
		EU = 2,
		[LocDescription("Enum_Region_Asia")]
		ASIA = 3,
		[LocDescription("Enum_Region_China")]
		CHINA = 5
>>>>>>> c693a4c... update code to 0925
	}

    public class RegionConvert {
        public static Region convert(string s) {
            switch (s) {
                case "未知":
                    return Region.UNKNOWN;
                case "美国":
                    return Region.US;
                case "欧洲":
                    return Region.EU;
                case "亚洲":
                    return Region.ASIA;
                case "中国":
                    return Region.CHINA;
            }
            return Region.CHINA;
        }
        public static string convert(Region r)
        {
            switch (r)
            {
                case Region.UNKNOWN:
                    return "未知";
                case Region.US:
                    return "美国";
                case Region.EU:
                    return "欧洲";
                case Region.ASIA:
                    return "亚洲";
                case Region.CHINA:
                    return "中国";
            }
            return "中国";
        }
    }

	public enum RegionAll
	{
<<<<<<< HEAD
		[Description("全部")]
		ALL = -1,

		[Description("未知")]
		UNKNOWN = 0,

		[Description("美国")]
		US = 1,

		[Description("欧洲")]
		EU = 2,

		[Description("亚洲")]
		ASIA = 3,

		[Description("中国")]
=======
		[LocDescription("Enum_Region_All")]
		ALL = -1,
		[LocDescription("Enum_Region_Unknown")]
		UNKNOWN = 0,
		[LocDescription("Enum_Region_US")]
		US = 1,
		[LocDescription("Enum_Region_EU")]
		EU = 2,
		[LocDescription("Enum_Region_Asia")]
		ASIA = 3,
		[LocDescription("Enum_Region_China")]
>>>>>>> c693a4c... update code to 0925
		CHINA = 5
	}
}
