using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AtxImage
{
    class ExportAtx
    {
        private Dictionary<UInt64, HashSet<int>> groups = null;

        private AtxImageBase atxImage = null;

        private string savepath;

        private List<LayoutInfo.MeshInfo> allmeshs = null;

        public ExportAtx(AtxImageBase atxImage, string savepath)
        {
            this.atxImage = atxImage;
            this.savepath = savepath;
        }



        public void GetAllGroups()
        {
            if (this.atxImage == null)
                return;
            this.groups = new Dictionary<ulong, HashSet<int>>();
            this.allmeshs = new List<LayoutInfo.MeshInfo>();
            for (int i = 0; i < atxImage.layoutInfo.Block.Count; i++)
            {
                foreach (LayoutInfo.MeshInfo item in atxImage.layoutInfo.Block[i].Mesh)
                {
                    allmeshs.Add(item);
                    ulong k = GetKey((int)Math.Round(item.srcOffsetX), (int)Math.Round(item.srcOffsetY),
                       (int)Math.Round(item.width), (int)Math.Round(item.height));
                    if(groups.ContainsKey(k))
                    {
                        groups[k].Add(allmeshs.Count - 1);
                    }
                    else
                    {
                        HashSet<int> one = new HashSet<int>();
                        one.Add(allmeshs.Count - 1);
                        groups.Add(k, one);
                    }
                }
            }
            Console.WriteLine("groups: " + groups.Count.ToString());
            foreach (var item in groups.Keys)
            {
                Console.WriteLine(item.ToString() + " : " + groups[item].Count.ToString());
            }
          
        }

        public void SaveSplitImages()
        {
            if (this.atxImage == null)
                return;
            for (int i = 0; i < atxImage.layoutInfo.Block.Count; i++)
            {
                string curpath = Path.Combine(this.savepath, "block" + i.ToString());
                Directory.CreateDirectory(curpath);
                string filename = atxImage.layoutInfo.Block[i].filename;
                for (int m=0; m< atxImage.layoutInfo.Block[i].Mesh.Count; m++)
                {
                    LayoutInfo.MeshInfo mesh = atxImage.layoutInfo.Block[i].Mesh[m];
                    Bitmap curimg = atxImage.textures[mesh.texNo];
                    string saveName = Path.Combine(curpath,filename + "_" +  m.ToString() + ".png");
                    Bitmap saveimg = ImageOp.GetSubBitmap(curimg,
                        new Rectangle((int)mesh.viewX, (int)mesh.viewY, (int)mesh.width, (int)mesh.height));
                    saveimg.Save(saveName, ImageFormat.Png);
                    saveimg.Dispose();
                }
            }
        }

        public string[] GetDiffString_wjx(string name)
        {
            Path.GetDirectoryName(name);
            Path.GetExtension(name);
            string[] array = Path.GetFileName(name).ToLower().Split(new char[]
            {
                ','
            });
            string[] result;
            if (array.Length == 0)
            {
                result = null;
            }
            else if (array.Length == 1)
            {
                result = array;
            }
            else
            {
                int num = 0;
                for (int i = 1; i < array.Length; i++)
                {
                    if (array[i] != "0")
                    {
                        num++;
                    }
                }
                if (num == 0)
                {
                    result = null;
                }
                else
                {
                    List<string> list = new List<string>();
                    int length = array[0].Length;
                    int j = 1;
                    int num2 = 0;
                    while (j < array.Length)
                    {
                        if (array[j] != "0")
                        {
                            if (j == 1)
                            {
                                string item = array[0] + array[1];
                                list.Add(item);
                            }
                            else if (array[j].Length > 0)
                            {
                                list.Add(string.Format("{0}_{1}", j - 1, array[j]));
                            }
                            num2++;
                        }
                        j++;
                    }
                    result = list.ToArray();
                }
            }
            return result;
        }

        private UInt64 GetKey(int srcOffx, int srcOffy, int width, int height)
        {
            UInt64 key = (UInt32)srcOffx;
            key = (key << 32) + (UInt32)srcOffy;
            //key = (key << 16) + (UInt16)width;
            //key = (key << 16) + (UInt16)height;
            return key;
        }

        
        
    }
}
