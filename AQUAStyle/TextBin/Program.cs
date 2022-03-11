using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextBin
{
    class Program
    {
        static void PringHelpMsg()
        {
            Console.WriteLine("export(提取文本): \n" +
                "*.exe -e 待提取文本的text.bin文件 保存路径 [-f(可选)] [-c(可选)]\n" +
                "若添加-f命令,则部分可能无需翻译的文本会被过滤掉,从而不输出到txt文件,否则会输出所有字符串\n" +
                "若添加-c命令,则会把原文本复制到待编辑的行(即以●{0:D6}●格式开头的行),否则会留空。\n\n");
            Console.WriteLine("import(导入文本):\n" +
                "*.exe -i 提取时输出的json文件 待导入的文本文件  [保存文件名(可选)]\n" +
                "说明:只会导入txt中以●{0:D6}●格式为开头的部分，☆{0:D6}☆开头的部分仅便于人工对照\n\n");
        }
        static void Main(string[] args)
        {
            if(args.Length < 3)
            {
                PringHelpMsg();
                Console.ReadKey();
                return;
            }
            if(args[0]  != "-e" && args[0] != "-i")
            {
                PringHelpMsg();
                Console.ReadKey();
                return;
            }
            if(args[0] == "-e")
            {
                string binFile = args[1];
                string savePath = args[2];
                bool isOnly91 = false;
                bool isCopy = false;
                if (args.Length > 3 && args[3] == "-f")
                    isOnly91 = true;
                if (args.Length > 3 && args[3] == "-c")
                    isCopy = true;
                if (args.Length > 4 && args[4] == "-f")
                    isOnly91 = true;
                if (args.Length > 4 && args[4] == "-c")
                    isCopy = true;
                TextBinParser parser = new TextBinParser();
                List<OPStruct> data = parser.ParseBinFile(binFile);
                parser.ExportString(data, savePath, isOnly91, isCopy);
            }
            else
            {
                string jsonFile = args[1];
                string txtFile = args[2];
                string saveName = null;
                if (args.Length > 3)
                    saveName = args[3];
                else
                    saveName = Path.Combine(Path.GetDirectoryName(jsonFile), "newtext.bin");
                TextBinParser parser = new TextBinParser();
                List<OPStruct> data = parser.ImportString(jsonFile, txtFile);
                parser.WriteBinFile(data, saveName);
            }
            Console.WriteLine("press any key to continue");
            Console.ReadKey();

        }
    }
}
