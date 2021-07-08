using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace AtxImage
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintJsonTest();
        }

        static void PrintJsonTest()
        {
            string dirpath = @"";
            string path = @"";
            string savepath = @"";
            //DirectoryInfo dir = new DirectoryInfo(path);
            //foreach (FileInfo item in dir.GetFiles("*.atx"))
            //{
            //    AtxImageBase atx = new AtxImageBase();
            //    FileStream fs = new FileStream(item.FullName, FileMode.Open);
            //    byte[] data = null;
            //    using (BinaryReader br = new BinaryReader(fs))
            //    {
            //        data = br.ReadBytes((int)fs.Length);
            //    }
            //    atx.Load(data);
            //    Console.WriteLine(item.FullName + " block num: " + atx.layoutInfo.Block.Count.ToString());
            //}
            AtxImageBase atx = new AtxImageBase();
            atx.Load(path);
            ExportAtx ex = new ExportAtx(atx, savepath);
            ex.SaveSplitImages();
            Console.WriteLine("block num: " + atx.layoutInfo.Block.Count.ToString());
            foreach (LayoutInfo.BlockInfo k in atx.layoutInfo.Block)
            {
                Console.WriteLine("=======================================");
                Console.WriteLine(k.ToString());
            }
        }
    }
}
