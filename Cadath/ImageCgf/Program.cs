using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageCgf
{
    class Program
    {
        static void PrintHelpMsg()
        {
            Console.WriteLine("usage: exe <picture path>  [save path] [method(1-3)]");
        }

        static void Main(string[] args)
        {
            byte method = 1;
            if (args.Length < 1)
            {
                PrintHelpMsg();
                Console.ReadKey();
                return;
            }
            string picpath = args[0];
            string savepath = Path.Combine(Path.GetDirectoryName(picpath), "pack_cgf");
            if(args.Length >= 2)
            {
                savepath = args[1];
            }
            if(args.Length >= 3)
            {
                if(!byte.TryParse(args[2], out method))
                {
                    method = 1;
                }
            }
            ImageCGF.Pack2CGFBatch(picpath, savepath, method);

            
        }
    }
}
