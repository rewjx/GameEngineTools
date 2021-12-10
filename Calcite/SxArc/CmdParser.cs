using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SxArc
{
    public class CmdParser
    {
        private Dictionary<string, Command> keyValuePairs = new Dictionary<string, Command>();

        static public CmdParser Parse(string[] args)
        {
            return new CmdParser(args.ToList());
        }

        static public CmdParser Parse(List<string> args)
        {
            return new CmdParser(args);
        }

        private CmdParser(List<string> args)
        {
            if (args.Count <= 0) return;
            for (int keyIndex = 0; keyIndex < args.Count; keyIndex++)
            {
                var key = args[keyIndex].Trim();
                if (!key.StartsWith("-")) continue;
                List<string> values = new List<string>();
                for (int valueIndex = keyIndex + 1; valueIndex < args.Count; valueIndex++)
                {
                    if (args[valueIndex].Trim().StartsWith("-")) break;
                    values.Add(args[valueIndex].Trim());
                }
                Command cmd = new Command(values);
                keyValuePairs.Add(key, cmd);
            }
        }

        public bool Has(string key)
        {
            return keyValuePairs.ContainsKey(key);
        }

        public Command this[string key]
        {
            get
            {
                return keyValuePairs[key];
            }
        }
    }

    public class Command
    {
        private List<string> mCmds;
        private int mIndex = 0;

        public Command(List<string> cmds)
        {
            this.mCmds = cmds;
        }

        public string First
        {
            get
            {
                mIndex = 0;
                return mCmds[mIndex];
            }
        }

        public string Last
        {
            get
            {
                mIndex = mCmds.Count - 1;
                return mCmds[mIndex];
            }
        }

        public string Get(int index)
        {
            if (index < mCmds.Count)
            {
                mIndex = index;
                return mCmds[mIndex];
            }
            return null;
        }

        public bool HasNext
        {
            get
            {
                return mIndex < mCmds.Count - 1;
            }
        }

        public string Next
        {
            get
            {
                if (HasNext)
                    return mCmds[mIndex++];
                return null;
            }
        }

        public List<string> All
        {
            get
            {
                return mCmds;
            }
        }
    }
}
