using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace YstbDec
{
    class Program
    {
        static void PrintHelpMessage()
        {
            Console.WriteLine("usage:");
            Console.WriteLine("*.exe -k [key Hex value] -i [input] -o [output(optional)] -c [key string codepage(optional)]");
        }
        static bool ProcessMain(CmdParser parser)
        {
            if(!parser.Has("-k") || !parser.Has("-i"))
            {
                return false;
            }
            string kstr = parser["-k"].First.Trim();
            uint kvalue = Convert.ToUInt32(kstr, 16);
            //if(!uint.TryParse(kstr, out uint kvalue))
            //{
            //    return false;
            //}
            string input = parser["-i"].First;
            string output = null;
            int codepage = 932;
            if(parser.Has("-o"))
            {
                output = parser["-o"].First;
            }
            if(parser.Has("-c"))
            {
               if(int.TryParse(parser["-c"].First, out int cp))
                {
                    codepage = cp;
                }
            }
            YstbDec dec = new YstbDec(input, output);
            //Encoding enc = Encoding.GetEncoding(codepage);
            //dec.CalcXorKey(kstr.Trim(), enc);
            dec.ChangeKey2ByteArr(kvalue);
            dec.ProcessMain();
            return true;

        }
        static void Main(string[] args)
        {
            CmdParser cmd = CmdParser.Parse(args);
            if(!ProcessMain(cmd))
            {
                PrintHelpMessage();
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey();
        }
    }
}
