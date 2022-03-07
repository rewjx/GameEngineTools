using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//shit code

namespace AquaArc
{
    class Program
    {

        static void PrintHelpMessage()
        {
            Console.WriteLine("unpack(解包): \n" +
                "方式一: 直接将待解包文件拖到exe上\n" +
                "方式二: *.exe 待解包文件名 [保存路径(可选)]\n\n\n");
            Console.WriteLine("封包:\n" +
                "*.exe -p 原封包 待封包文件路径 [保存路径(可选)]\n\n");

        }
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                PrintHelpMessage();
                Console.ReadKey();
                return;
            }
            if (args[0] == "-p")
            {
                if(args.Length < 3)
                {
                    PrintHelpMessage();
                    Console.ReadKey();
                    return;
                }
                string arc = args[1];
                string filepath = args[2];
                string savepath = null;
                if(args.Length > 3)
                {
                    savepath = args[3];
                }
                if (AquaArc.IsAquaArc(arc))
                {
                    AquaArc packer = new AquaArc(arc);
                    packer.Pack(filepath, savepath);
                }
                if (ASFAArc.IsAsfaArc(arc))
                {
                    Console.WriteLine("暂不支持ASFA文件的封包");
                }
            }
            //解包
            else if(File.Exists(args[0]))
            {
                string arc = args[0];
                string savepath = null;
                if (args.Length >= 2)
                {
                    savepath = args[1];
                }
                if (AquaArc.IsAquaArc(arc))
                {
                    AquaArc unpacker = new AquaArc(arc);
                    unpacker.Unpack(savepath);
                }
                if (ASFAArc.IsAsfaArc(arc))
                {
                    ASFAArc unpacker = new ASFAArc(arc, savepath);
                    unpacker.Unpack();
                }
            }
            else
            {
                PrintHelpMessage();
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey();
        }
    }
}
