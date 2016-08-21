namespace Hearthstone_Deck_Tracker.Enums
{
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
                    return "全部";
                case Format.Standard:
                    return "标准";
                case Format.Wild:
                    return "狂野";
            }
            return "全部";
        }

        public static string convert_(Format? f)
        {
            if (f == null) return null;
            switch (f)
            {
                case Format.All:
                    return "全部";
                case Format.Standard:
                    return "标准";
                case Format.Wild:
                    return "狂野";
            }
            return null;
        }

        public static Format convert(string f)
        {
            switch (f)
            {
                case "全部":
                    return Format.All;
                case "标准":
                    return Format.Standard;
                case "狂野":
                    return Format.Wild;
            }
            return Format.All;
        }
        public static Format? convert_(string f)
        {
            if (f == null || f == "") return null;
            switch (f)
            {
                case "全部":
                    return Format.All;
                case "标准":
                    return Format.Standard;
                case "狂野":
                    return Format.Wild;
            }
            return null;
        }
    }
}