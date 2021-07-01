using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CstxPack
{
    class TextBlockHelper
    {
        public static bool isOnlyAsciiString(string str)
        {
            foreach (char c in str)
            {
                if (c > 127)
                    return false;
            }
            return true;
        }


        public static void CutComment(ref string text)
        {
            int idx = text.IndexOf("//");
            if (idx >= 0)
            {
                text = text.Substring(0, idx);
            }
        }

        public static bool MoveToNext(List<List<string>> strs, ref int outId, ref int inId)
        {
            if(strs == null || outId < 0)
            {
                return false;
            }
            inId += 1;
            while(outId < strs.Count)
            {
                if(inId < strs[outId].Count)
                {
                    return true;
                }
                else
                {
                    outId += 1;
                    inId = 0;
                }
            }
            return false;
        }

        public static void SeekToNextLabel(List<List<string>> strs, int OutId, int innerId,
            out int newOutId, out int newInnerId)
        {
            newOutId = -1;
            newInnerId = -1;
            if (strs == null || OutId < 0 || innerId < 0)
                return;
            for(int i=OutId; i<strs.Count; i++)
            {
                for(int k=innerId; k<strs[i].Count; k++)
                {
                    List<string> lab = GetLabels(strs[i][k]);
                    if (lab != null && lab.Count > 0)
                    {
                        newOutId = i;
                        newInnerId = k;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// return the next label text's index
        /// </summary>
        /// <param name="text"></param>
        /// <param name="curIdx"></param>
        /// <returns></returns>
        public static int SeekToNextLabel(List<string>text, int curIdx)
        {
            if (text == null)
                return -1;
            for(int i=curIdx; i<text.Count; i++)
            {
                List<string> lab = GetLabels(text[i]);
                if(lab != null && lab.Count > 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool isSelectTag(string text)
        {
            string[] arr = text.Split(Helper.spaces, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 0 && arr[0] == Helper.SelectTag)
                return true;
            return false;
        }

        public static List<string> SplitSelectString(string sels)
        {
            string[] arr = sels.Split(Helper.spaces, StringSplitOptions.RemoveEmptyEntries);
            if (arr != null && arr.Length == 3)
                return new List<string>(arr);
            return null;
        }

        public static List<string> SplitProcessedImportString(string str, List<int> lens)
        {
            List<string> rtn = new List<string>();
            int oldTotal = lens.Sum();
            int newTotal = str.Length;
            int useLen = 0;
            for(int i=0; i<lens.Count; i++)
            {
                if (useLen >= str.Length)
                {
                    rtn.Add("");
                    continue;
                }
                float rate = (float)lens[i] / oldTotal;
                int newLen = (int)Math.Round(rate * newTotal);
                newLen = Math.Max(1, newLen);
                newLen = Math.Min(str.Length - useLen, newLen);
                if(i == lens.Count-1)
                {
                    newLen = str.Length - useLen;
                }
                rtn.Add(str.Substring(useLen, newLen));
                useLen += newLen;
            }
            return rtn;
        }

        public static int TrimStart(string str, HashSet<string>removeStrs)
        {
            int startPos = 0;
            int len = -1;
            while(true)
            {
                len = getStartStr(str.Substring(startPos), removeStrs);
                if (len < 0)
                    break;
                startPos += len;
            }
            return startPos;
        }

        public static int TrimEnd(string str, HashSet<string>removeStrs)
        {
            int endpos = str.Length;
            int len = -1;
            while(true)
            {
                len = geteEndStr(str.Substring(0, endpos), removeStrs);
                if (len < 0)
                    break;
                endpos -= len;
            }
            return endpos;
        }

        public static int geteEndStr(string str, HashSet<string>ends)
        {
            foreach (string s in ends)
            {
                if (str.EndsWith(s))
                    return s.Length;
            }
            return -1;
        }

        public static int getStartStr(string str, HashSet<string>begins)
        {
            foreach (string s in begins)
            {
                if (str.StartsWith(s))
                    return s.Length;
            }
            return -1;
        }

        public static bool isMessageText(string text, out bool isName)
        {
            isName = false;
            CutComment(ref text);
            if (string.IsNullOrWhiteSpace(text))
                return false;
            string[] strs = text.Split(Helper.spaces);
            string text2 = null;
            if (strs.Length >= 2 && strs[1].Length > 0)
            {
                text2 = strs[1];
                for (int i = 2; i < strs.Length; i++)
                {
                    text2 += strs[i];
                }
            }
            if (strs[0].Length != 0)
            {
                if (text2 == null)
                {
                    isName = true;
                }
                return true;
            }
            else
            {
                if (text2 == null)
                    return false;
                char c = text2[0];
                if (c != '%')
                {
                    if (c != '\\')
                    {
                        if (!isOnlyAsciiString(text2))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

        }



        public static string CheckLabel(string str)
        {
			if (string.IsNullOrEmpty(str))
				return null;
			if (str[0] == Helper.labelHeader)
			{
				int num = str.IndexOf("//");
				int num2 = str.IndexOfAny(Helper.spaces);
				bool flag3 = num >= 0 || num2 >= 0;
				if (flag3)
				{
					num = ((num >= 0) ? num : str.Length);
					num2 = ((num2 >= 0) ? num2 : str.Length);
					int num3 = Math.Min(num, num2);
					str = str.Substring(1, num3 - 1);
				}
				else
				{
					str = str.Substring(1);
				}
				return str;
			}
			return null;
		}

		public static List<string> SplitLabels(string text)
        {
			string lab = CheckLabel(text);
			if (lab == null)
				return null;
			return new List<string>(lab.Split(new char[]
			{
				Helper.labelSplitter
			}));
		}

        public static List<string> GetLabels(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                return null;
            }
            if(text[0] == '%')
            {
                return new List<string>();
            }
            return SplitLabels(text);
        }
    }
}
