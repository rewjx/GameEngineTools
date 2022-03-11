using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TextBin
{
    [Serializable]
    class OPStruct
    {
        public byte[] ops = null;

        public string textStr = null;

        public OPStruct() { }

        public OPStruct(byte[] ops, string text)
        {
            this.ops = ops;
            this.textStr = text;
        }
    }

    class TextBinParser
    {
        Encoding textEncoding = Encoding.UTF8;

        private const string orgFormat = "☆{0:D6}☆{1}";

        private const string modFormat = "●{0:D6}●{1}";

        private Regex orgRegex;

        private Regex modRegex;
        

        public TextBinParser(Encoding enc = null)
        {
            if (enc == null)
                textEncoding = Encoding.UTF8;
            else
                textEncoding = enc;

            orgRegex = new Regex(@"^\s*☆(\d+)☆(.*)$");
            modRegex = new Regex(@"^\s*●(\d+)●(.*)$");
        }


        public int ParseStrLength(byte[] ops)
        {
            int curpos = 0;
            int strlen = 0;
            while(curpos < ops.Length)
            {
                int nextlen = GetStrLengthOpLen(ops[curpos]);
                if(nextlen == -1)
                {
                    curpos += GetNextOpLength(ops[curpos]) + 1;
                    continue;
                }
                //检查是否是op的末尾
                if(curpos + nextlen + 1 != ops.Length)
                    throw new Exception("parse op Failed!");
                //合法的字符串长度op
                if (nextlen == 0)
                    strlen = ops[curpos] & 0x1F;
                else if (nextlen == 1)
                    strlen = ops[curpos + 1];
                else if (nextlen == 2)
                    strlen = Helper.ToInt16BigEndian(ops, curpos + 1);
                else if (nextlen == 4)
                    strlen = Helper.ToInt32BigEndian(ops, curpos + 1);
                break;
            }
            return strlen;
        }

        /// <summary>
        /// 更改ops里的字符串长度为指定值
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="newvalue"></param>
        public void WriteStrLength(ref byte[] ops, int newvalue)
        {
            if (newvalue < 0)
                return;
            int curpos = 0;
            //先找到编码字符串长度的起始op位置
            while(curpos < ops.Length)
            {
                int nextlen = GetStrLengthOpLen(ops[curpos]);
                if (nextlen == -1)
                {
                    curpos += GetNextOpLength(ops[curpos]) + 1;
                    continue;
                }
                else
                    break;
            }
            byte[] lenBytes = null;
            //curpos为编码字符串长度的起始op位置
            if (newvalue <= 0x1F)
            {
                lenBytes = new byte[1];
                lenBytes[0] = (byte)(0xA0 + newvalue);
            }
            else if (newvalue <= 0xFF)
            {
                lenBytes = new byte[2];
                lenBytes[0] = 0xD9;
                lenBytes[1] = (byte)newvalue;
            }
            else if(newvalue <= ushort.MaxValue)
            {
                lenBytes = new byte[3];
                lenBytes[0] = 0xDA;
                Helper.WriteShortBigEndian((short)newvalue, ref lenBytes, 1);
            }
            else
            {
                lenBytes = new byte[5];
                lenBytes[0] = 0xDB;
                Helper.WriteIntBigEndian(newvalue, ref lenBytes, 1);
            }
            if(curpos + lenBytes.Length > ops.Length)
            {
                byte[] newops = new byte[curpos + lenBytes.Length];
                Array.Copy(ops, newops, ops.Length);
                ops = newops;
            }
            Array.Copy(lenBytes, 0, ops, curpos, lenBytes.Length);
        }

        /// <summary>
        /// 判断该op是否是字符串长度相关的op
        /// 不是返回-1,是则返回接下来还有几个字节的op一起表示字符串长度
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public int GetStrLengthOpLen(byte op)
        {
            if (op >= 0xA0 && op <= 0xBF)
                return 0;
            else if (op == 0xC4 || op == 0xD9)
                return 1;
            else if (op == 0xC5 || op == 0xDA)
                return 2;
            else if (op == 0xC6 || op == 0xDB)
                return 4;
            else
                return -1;
        }

        public int GetNextOpLength(byte op)
        {
            if (op >= 0x90 && op <= 0x9F)
                return 0;
            else if (op >= 0xA0 && op <= 0xBF)
                return 0;
            else if ((op >= 0xC4 && op <= 0xC6) || (op >= 0xCA && op <= 0xD3))
                return 1 << (op & 3);
            else if (op >= 0xD9 && op <= 0xDB)
                return 1 << ((op & 3) - 1);
            else if (op >= 0xDC && op <= 0xDF)
                return 2 << (op & 1);
            else
                throw new Exception("unknown or unexpected opcode Error!(未知opcode)");

        }

        public List<OPStruct> ParseBinFile(string file)
        {
            using(FileStream fs = new FileStream(file, FileMode.Open))
            {
                return ParseBinFile(fs);
            }    
        }
        public List<OPStruct> ParseBinFile(Stream stream)
        {
            List<OPStruct> data = new List<OPStruct>();
            using(BinaryReader br = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    List<byte> curOps = new List<byte>();
                    byte op = br.ReadByte();
                    curOps.Add(op);
                    int nextlen = GetNextOpLength(op);
                    if (nextlen != 0)
                        curOps.AddRange(br.ReadBytes(nextlen));
                    //读完一组op
                    int strlen = ParseStrLength(curOps.ToArray());
                    string str = null;
                    if(strlen != 0)
                    {
                        str = this.textEncoding.GetString(br.ReadBytes(strlen));
                    }
                    OPStruct oneOp = new OPStruct(curOps.ToArray(), str);
                    data.Add(oneOp);
                }
            }
            return data;
        }

        public void WriteBinFile(List<OPStruct> data, string binFile)
        {
            if (data == null)
                return;
            using(FileStream fs = new FileStream(binFile, FileMode.Create))
            {
                using(BinaryWriter bw = new BinaryWriter(fs))
                {
                    for(int i=0; i<data.Count; i++)
                    {
                        bw.Write(data[i].ops);
                        if(data[i].textStr != null)
                        {
                            bw.Write(this.textEncoding.GetBytes(data[i].textStr));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导出文本
        /// </summary>
        /// <param name="data"></param>
        /// <param name="onlyCode91"></param>
        public void ExportString(List<OPStruct>data,string savepath, bool onlyCode91=false, bool isCopy=false)
        {
            //先序列化List<OPStruct>
            if(!Directory.Exists(savepath))
            {
                Directory.CreateDirectory(savepath);
            }
            string jsonFile = Path.Combine(savepath, "ops.json");
            string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
            using(FileStream fs = new FileStream(jsonFile, FileMode.Create))
            {
                using(StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(jsonStr);
                }
            }
            string textFile = Path.Combine(savepath, "strings.txt");
            using (FileStream fs = new FileStream(textFile, FileMode.Create))
            {
                using(StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    for(int i=0; i<data.Count; i++)
                    {
                        if (data[i].textStr == null)
                            continue;
                        if (i > 0 && onlyCode91 && data[i - 1].ops[0] != 0x91)
                            continue;
                        //导出文本
                        string str = string.Format(orgFormat, i, data[i].textStr);
                        sw.WriteLine(str);
                        if(isCopy)
                            str = string.Format(modFormat, i, data[i].textStr);
                        else
                            str = string.Format(modFormat, i, "");
                        sw.WriteLine(str);
                    }
                }
            }
        }

        public List<OPStruct> ImportString(string jsonFile, string txtFile)
        {
            List<OPStruct> data = null;
            using(FileStream fs = new FileStream(jsonFile, FileMode.Open))
            {
                using(StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string jsonStr = sr.ReadToEnd();
                    data= JsonConvert.DeserializeObject<List<OPStruct>>(jsonStr);
                }
            }
            Dictionary<int, string> idx_texts = ReadTxtFile(txtFile);
            foreach (int idx in idx_texts.Keys)
            {
                data[idx].textStr = idx_texts[idx];
                int byteslen = this.textEncoding.GetByteCount(data[idx].textStr);
                WriteStrLength(ref data[idx].ops, byteslen);
            }
            return data;
        }

        public Dictionary<int, string> ReadTxtFile(string file)
        {
            Dictionary<int, string> idx_texts = new Dictionary<int, string>();
            using(FileStream fs = new FileStream(file, FileMode.Open))
            {
                using(StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string line = sr.ReadLine();
                    while(line != null)
                    {
                        Match m = modRegex.Match(line);
                        if(m.Success)
                        {
                            int idx = int.Parse(m.Groups[1].Value);
                            string textStr = m.Groups[2].Value;
                            //由于字符串含有换行符,所以该textStr需要继续读取到下一个orgRegex或者文件末尾
                            line = sr.ReadLine();
                            while(line != null)
                            {
                                Match orgm = orgRegex.Match(line);
                                if (orgm.Success)
                                    break;
                                //添加到textStr后
                                textStr += "\n";
                                textStr += line;
                                line = sr.ReadLine();
                            }
                            idx_texts.Add(idx, textStr);
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            return idx_texts;
        }


    }
}
