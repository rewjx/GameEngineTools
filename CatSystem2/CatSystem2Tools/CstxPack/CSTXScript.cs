using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;


/*         
 *         shit code
 *         shit code
 *         shit code
 *         shit code
 *         shit code             
 *         
 */


namespace CstxPack
{
    public enum TextType
    {
        All,
        OrgMsg,
        ProMsg
    }

    public enum OpType 
    { 
        Invalid,
        UnCompress,
        Compress,
        Export,
        Import
    }

    public class CSTXScript
    {

        public const string CSTXSig = "CSTX";

        private const string orgFormat = "☆{0 }_{1:D6}_{2:D2}☆{3}";

        private const string transFormat = "★{0}_{1:D6}_{2:D2}★{3}";

        public const string MsgTag = "msg";

        public const string NameTag = "name";

        public const string SelectTag = "sel";

        public static HashSet<string> removeStarts = new HashSet<string> { @"\fll" };

        //\u3000 is really important?
        public static HashSet<string> removeEnds = new HashSet<string> { @"\@", @"\n", "\u3000" };

        private string orgCSTXPath = null;

        private string trTextPath = null;

        private string savePath = null;

        private bool isInputCompress;

        private bool isNoProcessText = false;

        private Encoding textEncoding;

        private Regex blankRegex;

        private Regex importRegex;

        private int msgId = 0;

        private int nameId = 0;

        private int selectId = 0;

        public CSTXScript(string orgCSTXpath, string savepath,
            int codePage=65001,
            bool isCompress=true,
            string trTextPath=null, bool NoProcess=false)
        {
            this.orgCSTXPath = orgCSTXpath;
            this.savePath = savepath;
            this.isInputCompress = isCompress;
            this.textEncoding = Encoding.GetEncoding(codePage);
            this.trTextPath = trTextPath;
            this.isNoProcessText = NoProcess;
            this.blankRegex = new Regex(@"^(\s*)(.*?)(\s*)$");
            this.importRegex = new Regex(@"^\s*★(\S+_\d+_\d+)★\s*?(.*?)\s*$");
        }

        public void ProcessMain(OpType op, TextType textType=TextType.OrgMsg)
        {
            DirectoryInfo dir = new DirectoryInfo(this.orgCSTXPath);
            foreach (FileInfo file in dir.GetFiles("*.cstx"))
            {
                string pureName = Path.GetFileNameWithoutExtension(file.Name);
                string saveName = Path.Combine(this.savePath, pureName);
                byte[] data = ReadCSTXByte(file.FullName);
                switch (op)
                {
                    case OpType.UnCompress:
                        {
                            saveName += ".cstx";
                            Helper.WriteBinFile(data, saveName);
                            break;
                        }
                    case OpType.Compress:
                        {
                            saveName += ".cstx";
                            CompressCSTX(data, saveName);
                            break;
                        }
                    case OpType.Export:
                        {
                            saveName += ".txt";
                            List<List<string>> texts = ReadCSTXScript(data);
                            ExportStringMain(texts, saveName, textType);
                            break;
                        }
                    case OpType.Import:
                        {
                            string importPath = Path.Combine(this.trTextPath, pureName + ".txt");
                            saveName += ".cstx";
                            List<List<string>> texts = ReadCSTXScript(data);
                            ImportStringMain(texts, importPath, saveName, textType);
                            break;
                        }
                    default:
                        break;
                }
            }
        }


        public void ExportStringMain(List<List<string>> texts, string saveFile, TextType type)
        {
            switch (type)
            {
                case TextType.All:
                {
                        ExportAllString(texts, saveFile);
                        break;
                }
                case TextType.OrgMsg:
                {
                        ExportOrigMessageTexts(texts, saveFile);
                        break;
                }
                case TextType.ProMsg:
                {
                        ExportProcessedMessageTexts(texts, saveFile);
                        break;
                }
                default:
                    break;
            }

        }


        public void ImportStringMain(List<List<string>> texts, string importFile, string saveFile, TextType type)
        {
            switch (type)
            {
                case TextType.All:
                    {
                        ImportAllString(texts, importFile, saveFile);
                        break;
                    }
                case TextType.OrgMsg:
                    {
                        ImportOrigMessageTexts(texts, importFile, saveFile);
                        break;
                    }
                case TextType.ProMsg:
                    {
                        ImportProcessedMessageTexts(texts, importFile, saveFile);
                        break;
                    }
                default:
                    break;
            }

        }


        /// <summary>
        /// 字符串首尾的tab键空格键会影响程序运行，为避免编辑的麻烦，导出或导入时获取原始的空白字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<string> GetStringParts(string str)
        {
            Match m = this.blankRegex.Match(str);
            List<string> rtn = new List<string>();
            for(int i=1; i<4; i++)
            {
                rtn.Add(m.Groups[i].Value);
            }
            return rtn;
        }

       public string PostProcessName(string line, out string rmStr)
        {
            int num = line.IndexOf("＠");
            rmStr = null;
            if (num >= 0)
            {
                rmStr = line.Substring(0, num + 1);
                line = line.Remove(0, num + 1);
            }
            return line;
        }



        public List<string> GetSelectItems(List<List<string>> strs, ref int outIdx, ref int inIdx)
        {
            int sid = 0;
            List<string> items = new List<string>();
            while(TextBlockHelper.MoveToNext(strs, ref outIdx, ref inIdx))
            {
                List<string> parts = TextBlockHelper.SplitSelectString(strs[outIdx][inIdx]);
                if (parts == null || parts.Count != 3)
                    return items;
                if (!parts[0].Equals(sid.ToString()))
                    return items;
                items.Add(parts[2]);
                sid += 1;   
            }
            return items;

        }

        public void ImportSelectItems(List<List<string>>orgs, ref List<List<string>>news, 
            ref int outIdx, ref int inIdx, StreamReader sr)
        {
            int sid = 0;
            while (TextBlockHelper.MoveToNext(orgs, ref outIdx, ref inIdx))
            {
                List<string> parts = TextBlockHelper.SplitSelectString(orgs[outIdx][inIdx]);
                if (parts == null || parts.Count != 3)
                    return;
                if (!parts[0].Equals(sid.ToString()))
                    return;
                string s = sr.ReadLine();
                if(s == null)
                {
                    throw new Exception("import file doesn't match original cstx script");
                }
                s = s.Trim();
                string sel = orgs[outIdx][inIdx].Replace(parts[2], s);
                news[outIdx][inIdx] = sel;
                sid += 1;
            }
        }

       
        public void ExportProcessedText(List<List<string>>strs, ref int outId, ref int inId, 
            StreamWriter sw)
        {
            string catStr = "";
            bool isName = false;
            for(; inId<strs[outId].Count; inId++)
            {
                if (!TextBlockHelper.isMessageText(strs[outId][inId], out isName))
                    break;
                if (isName)
                    break;
                string exs = GeneralTextExportProcess(strs[outId][inId]);
                int startpos = TextBlockHelper.TrimStart(exs, removeStarts);
                int endpos = TextBlockHelper.TrimEnd(exs, removeEnds);
                //遇到单独的控制字符，则结束该次对话文本拼接
                if (endpos <= startpos)
                {
                    inId += 1;
                    break;
                }
                catStr += exs.Substring(startpos, endpos - startpos);
            }
            inId -= 1;
            if(!string.IsNullOrWhiteSpace(catStr))
            {
                string mO = string.Format(orgFormat, MsgTag, this.msgId, 0, catStr);
                string mT = string.Format(transFormat, MsgTag, this.msgId, 0, "");
                sw.WriteLine(mO);
                sw.WriteLine(mT);
                sw.WriteLine();
                sw.WriteLine();
                this.msgId += 1;
            }
        }

        public void ImportProcessedText(List<List<string>> orgs, ref List<List<string>> news, 
            ref int outIdx, ref int inIdx, Dictionary<string, string> importStrings)
        {
            string catStr = "";
            bool isName = false;
            List<int> lenLog = new List<int>();
            List<string> partStart = new List<string>();
            List<string> partEnd = new List<string>();
            int orgInIdx = inIdx;
            //还原导出过程
            for (; inIdx < orgs[outIdx].Count; inIdx++)
            {
                if (!TextBlockHelper.isMessageText(orgs[outIdx][inIdx], out isName))
                    break;
                if (isName)
                    break;
                string exs = GeneralTextExportProcess(orgs[outIdx][inIdx]);
                int startpos = TextBlockHelper.TrimStart(exs, removeStarts);
                int endpos = TextBlockHelper.TrimEnd(exs, removeEnds);
                if (endpos <= startpos)
                {
                    inIdx += 1;
                    break;
                }
                partStart.Add(exs.Substring(0, startpos));
                partEnd.Add(exs.Substring(endpos, exs.Length - endpos));
                catStr += exs.Substring(startpos, endpos - startpos);
                lenLog.Add(endpos-startpos);
            }
            inIdx -= 1;


            //为了减少删除news中字符串的的麻烦，把导入的字符串按长度比例切分成同样数量的分割字符串
            //保证news的结构和orgs的结构一致。
            if (!string.IsNullOrWhiteSpace(catStr))
            {
                //导入
                string key = string.Format(transFormat, MsgTag, this.msgId, 0, "");
                key = key.Substring(1, key.Length - 2);
                if(!importStrings.ContainsKey(key))
                {
                    throw new Exception("import file doesn't match original cstx script");
                }
                string imCatStr = importStrings[key];
                //再按长度比例切割成同样数量个字符串
                List<string> parts = TextBlockHelper.SplitProcessedImportString(imCatStr, lenLog);
                for(int k=0; k<lenLog.Count; k++)
                {
                    //把两边可能的控制字符加回去
                    string imT = partStart[k] + parts[k] + partEnd[k];
                    string cT = GeneralTextImportProcess(orgs[outIdx][orgInIdx + k], imT);
                    news[outIdx][orgInIdx + k] = cT;
                }
                this.msgId += 1;
            }
        }


        public void ExportProcessedName(string name, StreamWriter sw)
        {
            string exStr = GeneralTextExportProcess(name);
            if (exStr == null)
                return;
            string exName = PostProcessName(exStr, out string cut);
            string nO = string.Format(orgFormat, NameTag, this.nameId, 0, exName);
            sw.WriteLine(nO);
            string nT = string.Format(transFormat, NameTag, this.nameId, 0, "");
            sw.WriteLine(nT);
            sw.WriteLine();
            sw.WriteLine();
            this.nameId += 1;
        }

        public string ImportProcessedName(string orgName, Dictionary<string, string> importStrings)
        {
            string testStr = GeneralTextExportProcess(orgName);
            if (testStr == null)
                return null;
            PostProcessName(testStr, out string cut);
            string key = string.Format(transFormat, NameTag, this.nameId, 0, "");
            key = key.Substring(1, key.Length - 2);
            if(!importStrings.ContainsKey(key))
            {
                throw new Exception("import file doesn't match original cstx script");
            }
            string ims = importStrings[key];
            this.nameId += 1;
            return GeneralTextImportProcess(orgName, cut + ims);
        }

        public void ExportProcessedSelectItems(List<string> items, StreamWriter sw)
        {
            for(int i=0; i<items.Count; i++)
            {
                string selO = string.Format(orgFormat, SelectTag, this.selectId, i, items[i]);
                sw.WriteLine(selO);
                string selT = string.Format(transFormat, SelectTag, this.selectId, i, "");
                sw.WriteLine(selT);
                sw.WriteLine();
                sw.WriteLine();
            }
            this.selectId += 1;
        }

        public void ImportProcessedSelectItems(List<List<string>> orgs, ref List<List<string>> news,
            ref int outIdx, ref int inIdx, Dictionary<string,string> importStrings)
        {
            int sid = 0;
            while (TextBlockHelper.MoveToNext(orgs, ref outIdx, ref inIdx))
            {
                List<string> parts = TextBlockHelper.SplitSelectString(orgs[outIdx][inIdx]);
                if (parts == null || parts.Count != 3)
                    break;
                if (!parts[0].Equals(sid.ToString()))
                    break;
                string key = string.Format(transFormat, SelectTag, this.selectId, sid, "");
                key = key.Substring(1, key.Length - 2);
                if (!importStrings.ContainsKey(key))
                {
                    throw new Exception("import file doesn't match original cstx script");
                }
                string s = importStrings[key];
                string sel = orgs[outIdx][inIdx].Replace(parts[2], s);
                news[outIdx][inIdx] = sel;
                sid += 1;
            }
            this.selectId += 1;
        }

        public void ExportProcessedMessageTexts(List<List<string>> strs, string saveFile)
        {

            this.msgId = 0;
            this.nameId = 0;
            this.selectId = 0;
            this.isNoProcessText = false;
            int outIdx = 0;
            int inIdx = 0;
            int newOutIdx = -1;
            int newInIdx = -1;
            using (FileStream fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, textEncoding))
                {
                    while (outIdx >= 0 && outIdx < strs.Count)
                    {
                        TextBlockHelper.SeekToNextLabel(strs, outIdx, inIdx, out newOutIdx, out newInIdx);
                        if (newOutIdx < 0 || newInIdx < 0)
                            break;
                        outIdx = newOutIdx;
                        inIdx = newInIdx;
                        while (TextBlockHelper.MoveToNext(strs, ref outIdx, ref inIdx))
                        {
                            string curStr = strs[outIdx][inIdx];
                            if (TextBlockHelper.GetLabels(curStr) != null)
                            {
                                break;
                            }
                            if (TextBlockHelper.isSelectTag(curStr))
                            {
                                List<string> items = GetSelectItems(strs, ref outIdx, ref inIdx);
                                ExportProcessedSelectItems(items, sw);
                            }
                            curStr = strs[outIdx][inIdx];
                            if (TextBlockHelper.isMessageText(curStr, out bool isName))
                            {
                                if(isName)
                                {
                                    ExportProcessedName(curStr, sw);
                                }
                                else
                                {
                                    ExportProcessedText(strs, ref outIdx, ref inIdx, sw);
                                }
                            }
                        }

                    }
                }
            }
        }

        public Dictionary<string, string> ReadImportProcessedFile(string path)
        {
            Dictionary<string, string> rtn = new Dictionary<string, string>();
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using(StreamReader sr = new StreamReader(fs, this.textEncoding))
                {
                    string line = sr.ReadLine();
                    while(line != null)
                    {
                        line = line.Trim();
                        if(!string.IsNullOrWhiteSpace(line))
                        {
                            Match m = this.importRegex.Match(line);
                            if(m.Groups.Count == 3 && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                            {
                                string k = m.Groups[1].Value;
                                string v = m.Groups[2].Value.Trim();
                                rtn.Add(k, v);                
                            }
   
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            return rtn;
        }
        public void ImportProcessedMessageTexts(List<List<string>>orgs, string importFile, string saveFile)
        {
            this.msgId = 0;
            this.nameId = 0;
            this.selectId = 0;
            this.isNoProcessText = false;
            int outIdx = 0;
            int inIdx = 0;
            int newOutIdx = -1;
            int newInIdx = -1;

            List<List<string>> news = new List<List<string>>(orgs);
            Dictionary<string, string> importStrings = ReadImportProcessedFile(importFile);

            while (outIdx >= 0 && outIdx < orgs.Count)
            {
                TextBlockHelper.SeekToNextLabel(orgs, outIdx, inIdx, out newOutIdx, out newInIdx);
                if (newOutIdx < 0 || newInIdx < 0)
                    break;
                outIdx = newOutIdx;
                inIdx = newInIdx;
                while (TextBlockHelper.MoveToNext(orgs, ref outIdx, ref inIdx))
                {
                    string curStr = orgs[outIdx][inIdx];
                    if (TextBlockHelper.GetLabels(curStr) != null)
                    {
                        break;
                    }
                    if (TextBlockHelper.isSelectTag(curStr))
                    {
                        ImportProcessedSelectItems(orgs, ref news, ref outIdx, ref inIdx,
                            importStrings);
                    }
                    curStr = orgs[outIdx][inIdx];
                    if (TextBlockHelper.isMessageText(curStr, out bool isName))
                    {
                        if(isName)
                        {
                            string newName = ImportProcessedName(curStr, importStrings);
                            if (newName != null)
                                news[outIdx][inIdx] = newName;
                        }
                        else
                        {
                            ImportProcessedText(orgs, ref news, ref outIdx, ref inIdx,
                                importStrings);
                        }
                    }
                }
            }
            byte[] data = WriteCSTXScript(news);
            CompressCSTX(data, saveFile);
        }

        public void ExportOrigMessageTexts(List<List<string>> strs, string saveFile)
        {
            int outIdx = 0;
            int inIdx = 0;
            int newOutIdx = -1;
            int newInIdx = -1;
            using (FileStream fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, textEncoding))
                {
                    while(outIdx >= 0 && outIdx < strs.Count)
                    {
                        TextBlockHelper.SeekToNextLabel(strs, outIdx, inIdx, out newOutIdx, out newInIdx);
                        if (newOutIdx < 0 || newInIdx < 0)
                            break;
                        outIdx = newOutIdx;
                        inIdx = newInIdx;
                        while(TextBlockHelper.MoveToNext(strs, ref outIdx, ref inIdx))
                        {
                            string curStr = strs[outIdx][inIdx];
                            if(TextBlockHelper.GetLabels(curStr) != null)
                            {
                                break;
                            }
                            if(TextBlockHelper.isSelectTag(curStr))
                            {
                                List<string> items = GetSelectItems(strs, ref outIdx, ref inIdx);
                                for(int i=0; i<items.Count; i++)
                                {
                                    sw.WriteLine("\t\t" + items[i]);
                                }
                            }
                            curStr = strs[outIdx][inIdx];
                            if(TextBlockHelper.isMessageText(curStr, out bool isName))
                            {
                                string exStr = GeneralTextExportProcess(curStr);
                                if (exStr != null)
                                {

                                    if (this.isNoProcessText)
                                    {
                                        sw.WriteLine(exStr);
                                    }
                                    else if (isName)
                                    {
                                        sw.WriteLine(PostProcessName(exStr, out string cut));
                                    }
                                    else
                                    {
                                        sw.WriteLine("\t" + exStr);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }


        public void ImportOrigMessageTexts(List<List<string>> orgs, string importFile, string saveFile)
        {
            int outIdx = 0;
            int inIdx = 0;
            int newOutIdx = -1;
            int newInIdx = -1;
            List<List<string>> news = new List<List<string>>(orgs);
            using (FileStream fs = new FileStream(importFile, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, textEncoding))
                {
                    while (outIdx >= 0 && outIdx < orgs.Count)
                    {
                        TextBlockHelper.SeekToNextLabel(orgs, outIdx, inIdx, out newOutIdx, out newInIdx);
                        if (newOutIdx < 0 || newInIdx < 0)
                            break;
                        outIdx = newOutIdx;
                        inIdx = newInIdx;
                        while (TextBlockHelper.MoveToNext(orgs, ref outIdx, ref inIdx))
                        {
                            string curStr = orgs[outIdx][inIdx];
                            if (TextBlockHelper.GetLabels(curStr) != null)
                            {
                                break;
                            }
                            if(TextBlockHelper.isSelectTag(curStr))
                            {
                                ImportSelectItems(orgs, ref news, ref outIdx, ref inIdx, sr);
                            }
                            curStr = orgs[outIdx][inIdx];
                            if (TextBlockHelper.isMessageText(curStr, out bool isName))
                            {

                                string testStr = GeneralTextExportProcess(curStr);
                                if (testStr != null)
                                {
                                    string ims = sr.ReadLine();
                                    if(ims == null)
                                    {
                                        throw new Exception(importFile + ":  import strings don't match original cstx script");
                                    }
                                    if (this.isNoProcessText)
                                    {
                                        news[outIdx][inIdx] = ims;
                                    }
                                    ims = ims.Trim();
                                    if (isName)
                                    {
                                        PostProcessName(testStr, out string cutstr);
                                        string imN = GeneralTextImportProcess(curStr, cutstr + ims);
                                        news[outIdx][inIdx] = imN;
                                    }
                                    else
                                    {
                                        string imT = GeneralTextImportProcess(curStr, ims);
                                        news[outIdx][inIdx] = imT;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            byte[] data = WriteCSTXScript(news);
            CompressCSTX(data, saveFile);

        }


        public string GeneralTextExportProcess(string orgStr)
        {
            if(this.isNoProcessText)
            {
                return orgStr;
            }
            List<string> partStr = GetStringParts(orgStr);
            if(!string.IsNullOrWhiteSpace(partStr[1]))
            {
                return partStr[1];
            }
            return null;
        }

        public string GeneralTextImportProcess(string orgStr, string importStr)
        {
            if(this.isNoProcessText)
            {
                return importStr;
            }
            List<string> partStr = GetStringParts(orgStr);
            if(string.IsNullOrWhiteSpace(partStr[1]))
            {
                return orgStr;
            }
            return partStr[0] + importStr.Trim() + partStr[2];
        
        }

        
        public void ExportAllString(List<List<string>> strs, string saveFile)
        {
            using(FileStream fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write))
            {
                using(StreamWriter sw = new StreamWriter(fs, textEncoding))
                {
                    for(int i=0; i<strs.Count; i++)
                    {
                        for(int k=0; k<strs[i].Count; k++)
                        {
                            string writeStr = GeneralTextExportProcess(strs[i][k]);
                            if(writeStr != null)
                            {
                                sw.WriteLine(writeStr);
                            }
                        }
                    }
                }
            }
        }


        public void ImportAllString(List<List<string>> strs, string importFile, string saveFile)
        {
            List<List<string>> news = new List<List<string>>();
            using(FileStream fs = new FileStream(importFile, FileMode.Open, FileAccess.Read))
            {
                using(StreamReader sr = new StreamReader(fs, textEncoding))
                {
                    for(int chunk=0; chunk < strs.Count; chunk++)
                    {
                        List<string> chunkStr = new List<string>();
                        for(int k=0; k<strs[chunk].Count; k++)
                        {
                            string testStr = GeneralTextExportProcess(strs[chunk][k]);
                            if(testStr == null)
                            {
                                chunkStr.Add(strs[chunk][k]);
                            }
                            else
                            {
                                string s = sr.ReadLine();
                                if (s == null)
                                {
                                    throw new Exception(importFile + ":  import strings don't match original cstx script");
                                }
                                chunkStr.Add(GeneralTextImportProcess(strs[chunk][k], s));
                            }
                        }
                        news.Add(chunkStr);
                    }
                }
            }
            byte[] data = WriteCSTXScript(news);
            CompressCSTX(data, saveFile);
        }



        public List<List<string>> ReadCSTXScript(byte[] data)
        {
            BinaryBuffer buffer = new BinaryBuffer(data);
            int chunkNum = buffer.ReadInt();
            if (chunkNum <= 0)
                return null;
            List<List<string>> rtn = new List<List<string>>();
            for (int i=0; i<chunkNum; i++)
            {
                int strNum = buffer.ReadLength();
                List<string> chunkStr = new List<string>();
                for(int k=0; k<strNum; k++)
                {
                    string s = buffer.ReadString(textEncoding);
                    chunkStr.Add(s);
                }
                rtn.Add(chunkStr);
            }
            return rtn;
        }

        public byte[] WriteCSTXScript(List<List<string>> scriptStrings)
        {
            if (scriptStrings == null || scriptStrings.Count == 0)
                return null;
            BinaryBuffer writeBuf = new BinaryBuffer();
            writeBuf.WriteInt(scriptStrings.Count);
            for(int i=0; i<scriptStrings.Count; i++)
            {
                writeBuf.WriteLength(scriptStrings[i].Count);
                for(int k=0; k<scriptStrings[i].Count; k++)
                {
                    writeBuf.WriteString(scriptStrings[i][k], textEncoding);
                }
            }
            return writeBuf.GetWriteBufferData();
        }


        public byte[] ReadCSTXByte(string fileName)
        {
            if(this.isInputCompress)
            {
                return UnCompressCSTX(fileName);
            }
            else
            {
                return Helper.ReadBinFile(fileName);
            }
        }
        
        /// <summary>
        /// 给定文件名，解压该cstx文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public byte[] UnCompressCSTX(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                byte[] buf = new byte[fs.Length];
                fs.Read(buf, 0, (int)fs.Length);
                bool isCSTX = Helper.CheckFileSignature(buf, CSTXSig);
                if(!isCSTX)
                {
                    return null;
                }
                return Helper.BinaryUnpack(buf);
            }
        }

        public void CompressCSTX(byte[] data, string saveFile)
        {
            using(FileStream fs= new FileStream(saveFile, FileMode.Create, FileAccess.Write))
            {
                byte[] compress = Helper.BinaryPack(data, CSTXSig, true);
                fs.Write(compress, 0, compress.Length);
            }
        }

        public void CompressCSTX(string readFile, string compressFile)
        {
            using (FileStream fs = new FileStream(readFile, FileMode.Open))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                CompressCSTX(data, compressFile);
            }
        }


    }
}
