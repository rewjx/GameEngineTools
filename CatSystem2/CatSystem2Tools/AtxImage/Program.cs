using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AtxImage
{
    class Program
    {

        public static void PrintHelpMessage()
        {
            Console.WriteLine("export atx to png or import png to atx(not supported yet)\n");
            Console.WriteLine("usage:");
            Console.WriteLine("export: *.exe -out -src(atx path you want to export) -save(the path you want to save the exported files)");
            Console.WriteLine("\noptional: -r(process src floder recursively)");

            Console.WriteLine("\n\n");
            Console.WriteLine("examples:");
            Console.WriteLine("export: *.exe -out -r -src atxpath -save savepath");
        }

        public static bool Process(CmdParser parser)
        {
            bool isImport = parser.Has("-in");
            bool isExport = parser.Has("-out");
            if((isImport && isExport) || (!isImport && !isExport))
            {
                return false;
            }
            string srcpath = null;
            string savepath = null;
            if (!parser.Has("-src"))
                return false;
            if (!parser.Has("-save"))
                return false;
            srcpath = parser["-src"].First;
            savepath = parser["-save"].First;
            bool isRecursive = parser.Has("-r");
            if(!Directory.Exists(savepath))
            {
                Directory.CreateDirectory(savepath);
            }
            if(isExport)
            {
                AtxProcessor.ExportAtxDir(srcpath, savepath, isRecursive);
            }
            return true;
        }
        static void Main(string[] args)
        {
            CmdParser parse = CmdParser.Parse(args);
            if(!Process(parse))
            {
                PrintHelpMessage();
                Console.ReadKey();
            }
        }



    }
}
