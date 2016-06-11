using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace AllanPlugins
{
    class AllanConverter
    {
        private int[] duowan_ids;
        private int[] nums;
        private int duowanheroId;
        private int _178heroId;
        private const string XeLi = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";
        private static char[] charSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        private CardTool mCardTool;

        public void release()
        {
            duowan_ids = null;
            nums = null;
            mCardTool.release();
        }

        public AllanConverter()
        {
            mCardTool = new CardTool();
        }

        public string[] get178ConvertedToEngNames(string _178web)
        {
            Console.WriteLine("_178web000=  " + _178web);
            //http://db.178.com/hs/deck/#3$909896:2,2924878:2,3314238:2,3871714:1,7195615:1,7487906:1,7586559:2,8469073:2
            _178web = _178web.Replace("http://db.178.com/hs/deck/#", "");
            Console.WriteLine("_178web111=  " + _178web);
            //3$909896:2,2924878:2,3314238:2,3871714:1,7195615:1,7487906:1,7586559:2,8469073:2
            _178heroId = int.Parse(_178web.Substring(0, 1));
            //909896:2,2924878:2,3314238:2,3871714:1,7195615:1,7487906:1,7586559:2,8469073:2
            _178web = _178web.Substring(2, _178web.Length - 2);
            Console.WriteLine("_178web222=  " + _178web);
            string[] ss = _178web.Split(',');
            string[] ret = { "", "", "-1" };
            ret[2] = CardTool.getHeroNameBy178ID(_178heroId);
            int size = ss.Length;
            for (int i = 0; i < size; i++)
            {
                string s = ss[i];
                int num = int.Parse(s.Substring(s.Length - 1, 1));
                int id = int.Parse(s.Substring(0, s.Length - 2));
                Console.WriteLine("num= " + num + " id= " + id);
                CardTool.CardSturct cs = mCardTool.getCardBy178Id(id);
                ret[0] += cs.enCard;
                ret[1] += cs.cnCard;
                if (num == 2)
                {
                    ret[0] += " x 2";
                    ret[1] += " x 2";
                }
                if (size - 1 != i)
                {
                    ret[0] += "\r\n";
                    ret[1] += "\r\n";
                }
            }
            return ret;
        }

        public string[] getDuowanConvertedToEngNames(string duowanweb)
        {
            if (duowanweb.Contains("http://ls.duowan.com/deckbuilder/index.html")
                    || duowanweb.Contains("http://ls.duowan.com/s/decksbuilder/index.html")
                    || duowanweb.Contains("ls.duowan.com/decksbuilder/standard.html")
                    || duowanweb.Contains("ls.duowan.com/s/decksbuilder/standard.html"))
            {
                if (duowanweb.Contains("&p1="))
                {
                    duowanweb = duowanweb.Substring(0, duowanweb.IndexOf("&p1="));
                }
                duowanStrToIds(duowanweb, XeLi);
            }
            return getConvertToHDT_ENG_CARDS();
        }

        /**
 * 将10进制转化为62进制
 * 
 * @param number
 * @param length
 *            转化成的62进制长度，不足length长度的话高位补0，否则不改变什么
 * @return
 */
        private string _10_to_62(int number, int length)
        {
            int rest = number;
            Stack<char> stack = new Stack<char>();
            StringBuilder result = new StringBuilder(0);
            while (rest != 0)
            {
                stack.Push(charSet[(rest - (rest / 62) * 62)]);
                rest = rest / 62;
            }
            for (; stack.Count != 0;)
            {
                result.Append(stack.Pop());
            }
            int result_length = result.Length;
            StringBuilder temp0 = new StringBuilder();
            for (int i = 0; i < length - result_length; i++)
            {
                temp0.Append('0');
            }
            return temp0.ToString() + result.ToString();
        }

        private string[] getConvertToHDT_ENG_CARDS()
        {
            string[] ret = new string[] { "", "", "-1" };
            int totalSize = duowan_ids.Length;
            for (int i = 0; i < totalSize; i++)
            {
                CardTool.CardSturct cs = mCardTool.getCardByDuowanId(duowan_ids[i]);
                //Debug.WriteLine("num " + nums[i] + " " + cs.ToStr());
                ret[0] += cs.enCard;
                if (nums[i] == 2) ret[0] += " x 2";
                if (i != totalSize - 1)
                    ret[0] += "\r\n";

                ret[1] += cs.cnCard;
                if (nums[i] == 2) ret[1] += " x 2";
                if (i != totalSize - 1)
                    ret[1] += "\r\n";
            }
            ret[2] = CardTool.getHeroNameByduowanID(duowanheroId);
            return ret;
        }

        private void duowanStrToIds(string duowan, string XL)
        {
            string[] duowans = duowan.Split('&');
            string decs = anyTo_10(duowans[1], XL);
            string[] ids = _10toAny(decs, int.Parse(duowans[2]));
            duowan_ids = new int[ids.Length];
            string[] orders = _10toAny(anyTo_10(duowans[3], XeLi),
                    int.Parse(duowans[4]));
            nums = new int[orders.Length];
            Console.WriteLine("duowan1 " + duowan);
            if (duowan.Contains("http://ls.duowan.com/deckbuilder/index.html")) {
                duowan = duowan.Replace(
                                    "http://ls.duowan.com/deckbuilder/index.html#i", "");
            } else if (duowan.Contains("http://ls.duowan.com/s/decksbuilder/index.html"))
            {
                duowan = duowan.Replace(
                                    "http://ls.duowan.com/s/decksbuilder/index.html#i", "");
            }
            if (duowan.Contains("ls.duowan.com/decksbuilder/standard.html"))
            {
                duowan = duowan.Replace(
                                    "http://ls.duowan.com/decksbuilder/standard.html#i", "");
            }
            if (duowan.Contains("ls.duowan.com/s/decksbuilder/standard.html"))
            {
                duowan = duowan.Replace(
                                    "http://ls.duowan.com/s/decksbuilder/standard.html#i", "");
            }

            Console.WriteLine("duowan2 " + duowan);
            duowanheroId = duowan.ElementAt(0) - '0';
            Console.WriteLine("duowanheroroId " + duowanheroId + duowan.ElementAt(0));
            for (int i = 0; i < ids.Length; i++)
            {
                duowan_ids[i] = int.Parse(ids[i]);
                nums[i] = int.Parse(orders[i]);
                //DuowanCard.CardSturct cs = mDuowanCard.getCardById(duowan_ids[i]);
                //Debug.WriteLine("num " + nums[i] + " " + cs.ToStr());
            }
        }

        /**
 * d是输入的必须是10进制的值.支持任意进制的返回 返回的是从高位到低位存储的数组 比如string[] = new string{"12",
 * "10", "0"}对于16进制就是ca0
 * 
 */
        public static string[] _10toAny(string d, int any)
        {
            if (d == null || d.Equals(""))
            {
                return null;
            }

            List<string> list = new List<string>();

            System.Numerics.BigInteger bint = BigInteger.Parse(d);
            BigInteger B_ANY = new BigInteger(any);

            BigInteger B0 = new BigInteger(0);

            while (bint != B0)
            {
                // System.out.print("bint= " + bint.toString());
                // System.out.println(", mod= " + bint.mod(B_ANY).toString());
                list.Add(BigInteger.ModPow(bint, 1, B_ANY).ToString());
                bint = BigInteger.Divide(bint, B_ANY);
            }

            string[] ss = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                ss[list.Count - i - 1] = list.ElementAt(i);
            }
            return ss;
        }

        /**
 * 任意进制(2~16)转10进制, 支持大数计算
 * 
 * @param in
 * @param any
 *            因为常用表示的进制只支持到2~16这些形式所以这里做了判断
 * @return
 */
        public static string anyTo_10(string inp, int any)
        {
            if (any < 2 || any > 16)
            {
                return "";
            }
            BigInteger ret = new BigInteger(0);
            BigInteger MUL = new BigInteger(any);
            BigInteger muls = new BigInteger(1);
            for (int i = inp.Length - 1; i >= 0; i--)
            {
                int d = formatting("" + inp.ElementAt(i));
                ret = BigInteger.Add(ret, BigInteger.Multiply(muls, new BigInteger(d)));
                muls = BigInteger.Multiply(muls, MUL);
            }
            return ret.ToString();
        }

        private static int formatting64(char a, char[] XLs)
        {
            for (int i = 0; i < XLs.Length; i++)
            {
                if (a == XLs[i])
                {
                    return i;
                }
            }
            return -1;
        }

        // 将十六进制中的字母转为对应的数字
        private static int formatting(string a)
        {
            int i = 0;
            for (int u = 0; u < 10; u++)
            {
                if (a.Equals("" + u))
                {
                    i = u;
                }
            }
            if (a.Equals("a"))
            {
                i = 10;
            }
            if (a.Equals("b"))
            {
                i = 11;
            }
            if (a.Equals("c"))
            {
                i = 12;
            }
            if (a.Equals("d"))
            {
                i = 13;
            }
            if (a.Equals("e"))
            {
                i = 14;
            }
            if (a.Equals("f"))
            {
                i = 15;
            }
            return i;
        }

        /**
         * 64进制转10进制
         * 
         * @param in
         * @param XL
         * @return
         */
        public static string anyTo_10(string inp, string XL)
        {
            char[] xlChs = XL.ToCharArray();
            BigInteger ret = new BigInteger(0);
            BigInteger MUL = new BigInteger(64);
            BigInteger muls = new BigInteger(1);
            for (int i = inp.Length - 1; i >= 0; i--)
            {
                int d = formatting64(inp.ElementAt(i), xlChs);
                ret = BigInteger.Add(ret, BigInteger.Multiply(muls, new BigInteger(d)));
                muls = BigInteger.Multiply(muls, MUL);
            }

            return ret.ToString();
        }

        /**
         * 二进制变成16进制,支持45000位2进制
         * 
         * @param b
         * @return
         */
        public static string _2To_16(string b)
        {
            if (b.Length > 45000)
            {
                return null;
            }
            string every4 = "";
            string ret = "";
            while (b != null
                    && b.Length > 0
                    && (every4 = b.Substring((b.Length - 4 >= 0) ? b.Length - 4
                            : 0, b.Length)) != null && every4.Length > 0)
            {
                b = (b.Length < 4) ? "" : b.Substring(0, b.Length - 4);
                string bin = Convert.ToString(Convert.ToInt32(every4, 2), 16);
                ret = bin + ret;
            }
            return ret;
        }

        /*
         * 可以直接任何位数 16进制转2进制
         */
        public static string _16To_2(string h)
        {
            if (h == null || h.Length > 10000)
            {
                return null;
            }

            char[] chs = h.ToCharArray();

            string biny = "";

            foreach (char c in chs)
            {
                string s = Convert.ToString(formatting("" + c), 2);
                if (s.Length == 1)
                {
                    s = "000" + s;
                }
                else if (s.Length == 2)
                {
                    s = "00" + s;
                }
                else if (s.Length == 3)
                {
                    s = "0" + s;
                }
                biny += s;
            }
            while (biny.StartsWith("0"))
            {
                biny = biny.Substring(1, biny.Length);
            }
            return biny;
        }

    }
}