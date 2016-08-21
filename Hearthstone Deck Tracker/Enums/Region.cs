#region

using System.ComponentModel;

#endregion

namespace Hearthstone_Deck_Tracker.Enums
{
	public enum Region
	{
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
		CHINA = 5
	}
}