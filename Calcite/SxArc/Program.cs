using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxArc
{
    class Program
    {

        static void PrintHelpMessage()
        {
            Console.WriteLine("usage:");
            Console.WriteLine("*.exe -i (index file with .sx extension) " +
                "-d (data file related to the index file) " +
                "-s (savepath)");
        }

        static bool Process(CmdParser parser)
        {
            string idxfile, datafile, savepath;
            if (!parser.Has("-i"))
                return false;
            idxfile = parser["-i"].First;
            if (!parser.Has("-d"))
                return false;
            datafile = parser["-d"].First;
            if (!parser.Has("-s"))
                return false;
            savepath = parser["-s"].First;
            SxUnpack unp = new SxUnpack(idxfile, datafile, savepath);
            unp.SetEncKeys(new IndexEncryptKey(), new FileEncryptKey());
            unp.Unpack();
            return true;
        }


        static void Main(string[] args)
        {
            CmdParser cmd = CmdParser.Parse(args);
            if (!Process(cmd))
            {
                PrintHelpMessage();
            }
            Console.ReadKey();

        }
    }
}
