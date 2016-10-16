#region

using System.ComponentModel;

#endregion

namespace Hearthstone_Deck_Tracker.Enums
{
	public enum DisplayedStats
	{
		[Description("All")]
		All,

		[Description("已选的")]
		Selected,

		[Description("最近的")]
		Latest,

		[Description("主要的已选的")]
		SelectedMajor,

		[Description("主要的最近的")]
		LatestMajor
	}
}