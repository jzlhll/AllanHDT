#region

using System.ComponentModel;

#endregion

namespace Hearthstone_Deck_Tracker.Enums
{
	public enum TimeFrame
	{
<<<<<<< HEAD
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
=======
		[LocDescription("Enum_TimeFrame_Today")]
		Today,

		[LocDescription("Enum_TimeFrame_Yesterday")]
		Yesterday,

		[LocDescription("Enum_TimeFrame_Last24Hours")]
		Last24Hours,

		[LocDescription("Enum_TimeFrame_ThisWeek")]
		ThisWeek,

		[LocDescription("Enum_TimeFrame_PreviousWeek")]
		PreviousWeek,

		[LocDescription("Enum_TimeFrame_Last7Days")]
		Last7Days,

		[LocDescription("Enum_TimeFrame_ThisMonth")]
		ThisMonth,

		[LocDescription("Enum_TimeFrame_PreviousMonth")]
		PreviousMonth,

		[LocDescription("Enum_TimeFrame_ThisYear")]
		ThisYear,

		[LocDescription("Enum_TimeFrame_PreviousYear")]
		PreviousYear,

		[LocDescription("Enum_TimeFrame_AllTime")]
>>>>>>> c693a4c... update code to 0925
		AllTime
	}

	public enum DisplayedTimeFrame
	{
<<<<<<< HEAD
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
=======
		[LocDescription("Enum_DisplayedTimeFrame_Today")]
		Today,

		[LocDescription("Enum_DisplayedTimeFrame_ThisWeek")]
		ThisWeek,

		[LocDescription("Enum_DisplayedTimeFrame_CurrentSeason")]
		CurrentSeason,

		[LocDescription("Enum_DisplayedTimeFrame_LastSeason")]
		LastSeason,

		[LocDescription("Enum_DisplayedTimeFrame_CustomSeason")]
		CustomSeason,

		[LocDescription("Enum_DisplayedTimeFrame_AllTime")]
		AllTime,

		[LocDescription("Enum_DisplayedTimeFrame_Custom")]
>>>>>>> c693a4c... update code to 0925
		Custom
	}
}
