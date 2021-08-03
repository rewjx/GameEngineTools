using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AtxImage
{
    class AtxProcessor
    {
        public static void ExportAtxDir(string atxPath, string savePath, bool recursive=false)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(atxPath);
            Parallel.ForEach(dirInfo.GetFiles("*.atx"), file =>
			{


				AtxImageBase atxImg = new AtxImageBase();
				atxImg.Load(file.FullName);
                ExportAtx exp = new ExportAtx(atxImg, savePath);
                exp.Export();
                GC.Collect();

            });
            if (recursive)
            {
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    string newSavePath = Path.Combine(savePath, dir.Name);
                    Directory.CreateDirectory(newSavePath);
                    ExportAtxDir(dir.FullName, newSavePath, recursive);
                }
            }
        }

        public static string[] GetDiffString(string name)
        {
			string directoryName = Path.GetDirectoryName(name);
			string extension = Path.GetExtension(name);
			string text = Path.GetFileName(name).ToLower();
			string[] array = text.Split(new char[]
			{
				','
			});
			bool flag = array.Length == 0;
			string[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = array.Length == 1;
				if (flag2)
				{
					result = array;
				}
				else
				{
					int num = 0;
					for (int i = 1; i < array.Length; i++)
					{
						bool flag3 = array[i] != "0";
						if (flag3)
						{
							num++;
						}
					}
					bool flag4 = num == 0;
					if (flag4)
					{
						result = null;
					}
					else
					{
						List<string> list = new List<string>();
						int num2 = array[0].Length + 1;
						int j = 1;
						int num3 = 0;
						while (j < array.Length)
						{
							bool flag5 = array[j] != "0";
							if (flag5)
							{
								bool flag6 = j == 1;
								if (flag6)
								{
									string tname = array[0] + array[1];
									list.Add(tname);
									list.Add(string.Format("{0}_{1}", j - 1, array[j]));
								}
								else
								{
									bool flag7 = array[j].Length > 0;
									if (flag7)
									{
										list.Add(string.Format("{0}_{1}", j - 1, array[j]));
									}
								}
								num3++;
							}
							j++;
						}
						result = list.ToArray();
					}
				}
			}
			return result;
		}


    }
}
