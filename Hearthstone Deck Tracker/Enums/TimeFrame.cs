#region

using System.ComponentModel;

#endregion

namespace Hearthstone_Deck_Tracker.Enums
{
	public enum TimeFrame
	{
		[Description("今天")]
		Today,

		[Description("昨天")]
		Yesterday,

		[Description("过去24小时")]
		Last24Hours,

		[Description("该星期")]
		ThisWeek,

		[Description("上个星期")]
		PreviousWeek,

		[Description("过去7天")]
		Last7Days,

		[Description("该月")]
		ThisMonth,

		[Description("上个月")]
		PreviousMonth,

		[Description("今年")]
		ThisYear,

		[Description("去年")]
		PreviousYear,

		[Description("所有时段")]
		AllTime
	}

	public enum DisplayedTimeFrame
	{
		[Description("今天")]
		Today,

		[Description("星期")]
		ThisWeek,

		[Description("该赛季")]
		CurrentSeason,

		[Description("上赛季")]
		LastSeason,

		[Description("自定义赛季")]
		CustomSeason,

		[Description("所有时段")]
		AllTime,

		[Description("自定义")]
		Custom
	}
}