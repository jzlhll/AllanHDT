#region

using System.ComponentModel;

#endregion

namespace Hearthstone_Deck_Tracker.Enums
{
	public enum ArenaImportingBehaviour
	{
		[Description("自动导入&保存")]
		AutoImportSave,

		[Description("询问和自动导入")]
		AutoAsk,

		[Description("主动")]
		Manual
	}
}