namespace Hearthstone_Deck_Tracker.Enums
{
	public enum YesNo
	{
		Yes,
		No
	}

	public enum AllYesNo
	{
		All,
		Yes,
		No
	}

    public class AllYesNoConverter
    {
        public static string convert(AllYesNo yn)
        {
            switch (yn)
            {
                case AllYesNo.Yes:
                    return "是";
                case AllYesNo.No:
                    return "否";
                case AllYesNo.All:
                    return "全部";
            }
            return "全部";
        }

        public static AllYesNo convert(string yn)
        {
            switch (yn)
            {
                case "是":
                    return AllYesNo.Yes;
                case "否":
                    return AllYesNo.No;
                case "全部":
                    return AllYesNo.All;
            }
            return AllYesNo.All;
        }
    }

    public class YesNoConverter {
        public static string convert(YesNo yn) {
            switch (yn) {
                case YesNo.Yes:
                    return "是";
                case YesNo.No:
                    return "否";
            }
            return "是";
        }

        public static YesNo convert(string yn)
        {
            switch (yn)
            {
                case "是":
                    return YesNo.Yes;
                case "否":
                    return YesNo.No;
            }
            return YesNo.Yes;
        }
    }
}