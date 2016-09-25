namespace Hearthstone_Deck_Tracker.Enums
{
<<<<<<< HEAD
    public enum Format
    {
        All,
        Standard,
        Wild
    }

    public class FormatConvert
    {
        public static string convert(Format f)
        {
            switch (f)
            {
                case Format.All:
                    return "ȫ��";
                case Format.Standard:
                    return "��׼";
                case Format.Wild:
                    return "��Ұ";
            }
            return "ȫ��";
        }

        public static string convert_(Format? f)
        {
            if (f == null) return null;
            switch (f)
            {
                case Format.All:
                    return "ȫ��";
                case Format.Standard:
                    return "��׼";
                case Format.Wild:
                    return "��Ұ";
            }
            return null;
        }

        public static Format convert(string f)
        {
            switch (f)
            {
                case "ȫ��":
                    return Format.All;
                case "��׼":
                    return Format.Standard;
                case "��Ұ":
                    return Format.Wild;
            }
            return Format.All;
        }
        public static Format? convert_(string f)
        {
            if (f == null || f == "") return null;
            switch (f)
            {
                case "ȫ��":
                    return Format.All;
                case "��׼":
                    return Format.Standard;
                case "��Ұ":
                    return Format.Wild;
            }
            return null;
        }
    }
}
=======
	public enum Format
	{
		[LocDescription("Enum_Format_All")]
		All,
		[LocDescription("Enum_Format_Standard")]
		Standard,
		[LocDescription("Enum_Format_Wild")]
		Wild
	}
}
>>>>>>> c693a4c... update code to 0925
