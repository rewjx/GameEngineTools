using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//shit code

namespace AquaArc
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("请输入要解包的文件路径及保存路径[可选]");
                return;
            }
            string arc = args[0];
            string savepath = null;
            if (args.Length >= 2)
            {
                savepath = args[1];
            }
            if(AquaArc.IsAquaArc(arc))
            {
                AquaArc unpacker = new AquaArc(arc, savepath);
                unpacker.Unpack();
            }
            if(ASFAArc.IsAsfaArc(arc))
            {
                ASFAArc unpacker = new ASFAArc(arc, savepath);
                unpacker.Unpack();
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey();
        }
    }
}
