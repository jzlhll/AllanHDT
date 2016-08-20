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