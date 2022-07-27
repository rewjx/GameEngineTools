using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageCgf
{
    class ImageCGF
    {

        public const uint signature = 0x1A464743;
        /// <summary>
        /// bgr or bgra
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static List<List<byte>> ReadImageData(Bitmap img)
        {
            List<List<byte>> data = new List<List<byte>>();
            int channels = 4;
            if (img.PixelFormat == PixelFormat.Format24bppRgb)
            {
                channels = 3;
            }
            else if(img.PixelFormat == PixelFormat.Format32bppArgb)
            {
                channels = 4;
            }
            else
            {
                throw new Exception("unsupported pixel format");
            }
            for(int i=0; i<channels; i++)
            {
                data.Add(new List<byte>());
            }
            BitmapData bitdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            unsafe
            {
                for(int r =0; r<img.Height; r++)
                {
                    byte* b = (byte*)(bitdata.Scan0 + r * bitdata.Stride);
                    for(int c= 0; c<img.Width; c++)
                    {
                        for(int i=0; i<channels; i++)
                        {
                            data[i].Add(*(b + i));
                        }
                        b += channels;
                    }
                }
            }
            return data;
        }

        public static byte GetMethod(string filename)
        {
            FileStream fs = File.OpenRead(filename);
            byte[] data = new byte[5];
            fs.Read(data, 0, data.Length);
            fs.Close();
            return data[4];
        }

        public static void PrintMethod(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            Dictionary<byte, List<string>> mfile = new Dictionary<byte, List<string>>();
            Dictionary<byte, uint> mcount = new Dictionary<byte, uint>();
            foreach (var item in dir.GetFiles())
            {
                byte m = GetMethod(item.FullName);
                if(!mcount.ContainsKey(m))
                {
                    mcount[m] = 0;
                    mfile[m] = new List<string>();
                }
                mcount[m] += 1;
                mfile[m].Add(Path.GetFileName(item.FullName));
            }
            foreach (var item in mcount.Keys)
            {
                Console.WriteLine(item.ToString() + " : " + mcount[item].ToString());
                if(item == 128)
                {
                    foreach (var n in mfile[item])
                    {
                        Console.WriteLine(n);
                    }
                    Console.WriteLine("======================");
                }
            }
        }

        public static byte[] PackZLibV3(List<byte>data)
        {
            byte px = 0;
            for(int i=0; i<data.Count; i++)
            {
                data[i] ^= px;
                px ^= data[i];
            }
            byte[] compress = Helper.ZlibCompress(data.ToArray());
            //在头部添加checksum
            using(MemoryStream ms = new MemoryStream())
            {
                using(BinaryWriter bw = new BinaryWriter(ms))
                {
                    uint checksum = 0;
                    bw.Write(checksum);
                    bw.Write(compress);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms.ToArray();
                }
            }
        }

        public static byte[] PackZlibV1(List<byte> data)
        {
            byte[] compress = Helper.ZlibCompress(data.ToArray());
            byte[] channel_data = null;
            using(MemoryStream ms = new MemoryStream())
            {
                using(BinaryWriter bw=  new BinaryWriter(ms))
                {
                    uint checksum = 0;
                    bw.Write(checksum);
                    bw.Write(compress);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    channel_data = ms.ToArray();
                }
            }
            //encrypt
            unsafe
            {
                fixed (byte* data8 = channel_data)
                {
                    uint* data32 = (uint*)data8;
                    const uint seed = 0x3977141B;
                    uint key = seed;
                    for(int i=0; i<channel_data.Length; i += 4)
                    {
                        key = Helper.RotL(key, 3);
                        *data32 ^= key;
                        data32++;
                        key += seed;
                    }
                }
            }
            return channel_data;
        }


        public static void Pack2CGFBatch(string picPath, string savePath, byte method)
        {
            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            if (File.Exists(picPath))
            {
                string purename = Path.GetFileNameWithoutExtension(picPath);
                string newname = Path.Combine(savePath, purename + ".cgf");
                Pack2CGF(picPath, newname, method);
            }
            else if (Directory.Exists(picPath))
            {
                DirectoryInfo dir = new DirectoryInfo(picPath);
                foreach (var fileinfo in dir.GetFiles())
                {
                    try
                    {
                        string purename = Path.GetFileNameWithoutExtension(fileinfo.FullName);
                        string newname = Path.Combine(savePath, purename + ".cgf");
                        Pack2CGF(fileinfo.FullName, newname, method);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("pack " + fileinfo.FullName + "  failed");
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
        public static void Pack2CGF(string picpath, string savepath, byte method)
        {
            Bitmap img = new Bitmap(picpath);
            List<List<byte>> data = ReadImageData(img);
            Func<List<byte>, byte[]> PackMethod = null;
            if(method == 3)
            {
                PackMethod = PackZLibV3;
            }
            else if(method == 1)
            {
                PackMethod = PackZlibV1;
            }
            else
            {
                throw new NotSupportedException();
            }
             using(FileStream fs=new FileStream(savepath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    //写signature
                    bw.Write(signature);
                    //写method
                    bw.Write(method);
                    //写bpp
                    byte bpp = (byte)(data.Count * 8);
                    bw.Write(bpp);
                    //写width
                    bw.Write((ushort)img.Width);
                    //height
                    bw.Write((ushort)img.Height);
                    //bgr or bgra
                    for(int i=0; i<data.Count; i++)
                    {
                        byte[] packChannel = PackMethod(data[i]);
                        //length
                        bw.Write(packChannel.Length);
                        //channel
                        bw.Write(packChannel);
                    }

                }
          
            }
        }
    }
}
