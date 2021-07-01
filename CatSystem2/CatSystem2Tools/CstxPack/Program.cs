using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace CstxPack
{
    class Program
    {

        public static void PrintHelpMessage()
        {
            Console.WriteLine("usage: *.exe [operationType] [arguments]\n");

            Console.WriteLine("support four operations: \n" +
                "-u:uncompress cstx script\n" +
                "-c:compress cstx script\n" +
                "-e:export texts\n" +
                "-i:import texts\n\n");

            Console.WriteLine("arguments:\n" +
                "-op(orginal cstx script path for processing)\n" +
                "-sp(save path)\n\n" +
                 "===arguments for texts operation:===\n" +
                 "-ip(import texts path)\n" +
                 "-text(options: all, omsg, pmsg, default is omsg): all: all texts; \n" +
                 "omsg: name, choice and message texts; pmsg: highly processed name, choice and message texts\n" +
                 "=======This parameter(-text) needs to be the same when importing and exporting!!!=========\n"+
                 "-com(true or false,default is true):is the original cstx script compressed\n" +
                 "-cp(codepage, default is 65001):input and output use the same codepage\n" +
                 "-no(this option is not used by default): dont't do any process when export or import texts, only valid in all or omsg mode\n\n\n");

            Console.WriteLine("examples:\n" +
                "uncompress:          *.exe -u -op [cstx script directory] -sp [save directory]\n" +
                "compress:            *.exe -c -op [cstx script directory] -sp [save directory]\n" +
                "export all texts:    *.exe -e -op [cstx script directory] -sp [save directory] -text all -com true -cp 65001\n" +
                "import not processed omsg texts:   *.exe -i -op [cstx script directory] -sp [save directory] -ip [import texts directory] -text omsg -com true -cp 65001 -no");    
        }

        public static OpType CheckOpType(CmdParser parser)
        {
            OpType op = OpType.Invalid;
            if(parser.Has("-u"))
            {
                op = OpType.UnCompress;
            }
            if(parser.Has("-c"))
            {
                if (op != OpType.Invalid)
                    return OpType.Invalid;
                op = OpType.Compress;
            }
            if(parser.Has("-e"))
            {
                if (op != OpType.Invalid)
                    return OpType.Invalid;
                op = OpType.Export;
            }
            if(parser.Has("-i"))
            {
                if (op != OpType.Invalid)
                    return OpType.Invalid;
                op = OpType.Import;
            }
            return op;
        }

        public static TextType GetTextType(CmdParser parser)
        {
            TextType type = TextType.OrgMsg;
            if(parser.Has("-text"))
            {
                string option = parser["-text"].First;
                if(option.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    type = TextType.All;
                }
                else if(option.Equals("omsg", StringComparison.OrdinalIgnoreCase))
                {
                    type = TextType.OrgMsg;
                }
                else if(option.Equals("pmsg", StringComparison.OrdinalIgnoreCase))
                {
                    type = TextType.ProMsg;
                }
            }
            return type;
        }

        public static bool Process(CmdParser parser)
        {

            string trPath = null;
            int codePage = 65001;
            bool isCompress = true;
            bool isNoProcessText = false;

            OpType op = CheckOpType(parser);
            if (op == OpType.Invalid)
            {
                return false;
            }
            if (!parser.Has("-op"))
                return false;
            string cstxPath = parser["-op"].First;
            if (string.IsNullOrWhiteSpace(cstxPath))
                return false;
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), "_save_");
            if(parser.Has("-sp"))
            {
                savePath = parser["-sp"].First;
            }
            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            if(parser.Has("-ip"))
            {
                trPath = parser["-ip"].First;
            }
            TextType textType = GetTextType(parser);
            if (parser.Has("-com"))
            {
                string option = parser["-com"].First;
                if (option.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    isCompress = false;
                }
            }
            if(parser.Has("-cp"))
            {
                int icp;
                if(int.TryParse(parser["-cp"].First, out icp))
                {
                    if (icp >= 0)
                        codePage = icp;
                }
            }
            isNoProcessText = parser.Has("-no");
            if(op == OpType.Import && !Directory.Exists(trPath))
            {
                return false;
            }
            if(op == OpType.UnCompress)
            {
                isCompress = true;
            }
            else if(op == OpType.Compress)
            {
                isCompress = false;
            }
            CSTXScript script = new CSTXScript(orgCSTXpath: cstxPath,
                savepath: savePath, codePage: codePage, isCompress: isCompress,
                trTextPath: trPath, NoProcess:isNoProcessText);
            script.ProcessMain(op, textType);
            return true;

        }
        static void Main(string[] args)
        {
            CmdParser parse = CmdParser.Parse(args);
            if(!Process(parse))
            {
                PrintHelpMessage();
                Console.ReadLine();
            }
            return;
        }
    }
}
