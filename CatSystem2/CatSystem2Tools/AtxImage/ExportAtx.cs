using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AtxImage
{
    class ExportAtx
    {
        private AtxImageBase atxImage = null;

        private string savepath;

        public ExportAtx(AtxImageBase atxImage, string savepath)
        {
            this.atxImage = atxImage;
            this.savepath = Path.Combine(savepath, atxImage.FileName);
            if(Directory.Exists(this.savepath) == false)
            {
                Directory.CreateDirectory(this.savepath);
            }

        }


        /// <summary>
        /// 将atx导出为原始未分割的图像，若含有差分信息，同时保存差分图像的坐标信息
        /// </summary>
        public void Export()
        {
            if (this.atxImage == null)
                return;
            string baseName = null;
            Dictionary<string, string> names = new Dictionary<string, string>();
            Dictionary<string, List<int>> offsetInfo = new Dictionary<string, List<int>>();
            HashSet<string> occured = new HashSet<string>();
            for (int i = 0; i < this.atxImage.layoutInfo.Block.Count; i++)
            {
                LayoutInfo.BlockInfo blockInfo = this.atxImage.layoutInfo.Block[i];
                string picName = blockInfo.filenameOld;
                if(string.IsNullOrWhiteSpace(picName))
                {
                    if(baseName == null)
                    {
                        baseName = FindBaseName();
                    }
                    picName = newNameToOldName(baseName, blockInfo.filename);
                }
                if(occured.Contains(picName))
                {
                    picName += "@" + blockInfo.id.ToString();
                }
                occured.Add(picName);
                Bitmap pic = MergeOneBlock(blockInfo);
                string k = blockInfo.filename + "@" + blockInfo.id.ToString();
                names[k] = picName;
                offsetInfo[picName] = new List<int>() { (int)blockInfo.offsetX, 
                    (int)blockInfo.offsetY };
                string pngSavePath = Path.Combine(this.savepath, picName + ".png");
                pic.Save(pngSavePath, ImageFormat.Png);
                pic.Dispose();
            }
            // save file name information for importing
            string jsonStr = Json.Serialize(names);
            string jsonPath = Path.Combine(this.savepath, "info.json");
            using(FileStream fs = new FileStream(jsonPath, FileMode.Create))
            {
                using(StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(jsonStr);
                }
            }

            //save image offset information
            jsonStr = Json.Serialize(offsetInfo);
            jsonPath = Path.Combine(this.savepath, "offset.json");
            using (FileStream fs = new FileStream(jsonPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(jsonStr);
                }
            }
        }

        public string FindBaseName()
        {
            if (this.atxImage == null)
                return null;
            string name = null;
            foreach (LayoutInfo.BlockInfo blockinfo in this.atxImage.layoutInfo.Block)
            {
                string curName = blockinfo.filename;
                if (string.IsNullOrEmpty(curName))
                    continue;
                string[] ss = curName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length == 1)
                {
                    name = ss[0];
                    break;
                }
            }
            if (name == null)
                name = this.atxImage.FileName;
            return name;
        }
        public string newNameToOldName(string baseName, string newName)
        {
            string[] ss = newName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            if(ss.Length == 1 || !int.TryParse(ss[0], out int v))
            {
                return baseName + "_1";
            }
            int len = 0;
            string name = null;
            if(int.TryParse(ss[0], out len))
            {
                name += baseName + "_";
                for(int i=0; i<len; i++)
                {
                    name += "0";
                }
                name += ss[1];
            }
            if (string.IsNullOrEmpty(name))
                name = newName;
            return name;
        }

        /// <summary>
        /// 把一个block合并为一张图片
        /// </summary>
        /// <param name="blockInfo"></param>
        /// <returns></returns>
        public Bitmap MergeOneBlock(LayoutInfo.BlockInfo blockInfo)
        {
            int width = (int)Math.Round(blockInfo.width);
            int height = (int)Math.Round(blockInfo.height);
            Bitmap img = new Bitmap(width, height);
            foreach (LayoutInfo.MeshInfo meshinfo in blockInfo.Mesh)
            {
                Bitmap curtex= atxImage.textures[meshinfo.texNo];
                Bitmap subImg = ImageOp.GetSubBitmap(curtex,
                    new Rectangle((int)meshinfo.viewX, (int)meshinfo.viewY,
                    (int)meshinfo.width, (int)meshinfo.height));
                ImageOp.WriteBitmapAtTargetPosition(ref img, subImg,
                    (int)meshinfo.srcOffsetX, (int)meshinfo.srcOffsetY);
                subImg.Dispose();
            }
            return img;
        }

        public void SaveSplitImages()
        {
            if (this.atxImage == null)
                return;
            for (int i = 0; i < atxImage.layoutInfo.Block.Count; i++)
            {
                string curpath = Path.Combine(this.savepath, "block" + i.ToString());
                Directory.CreateDirectory(curpath);
                string filename = atxImage.layoutInfo.Block[i].filename;
                for (int m=0; m< atxImage.layoutInfo.Block[i].Mesh.Count; m++)
                {
                    LayoutInfo.MeshInfo mesh = atxImage.layoutInfo.Block[i].Mesh[m];
                    Bitmap curimg = atxImage.textures[mesh.texNo];
                    string saveName = Path.Combine(curpath,filename + "#" +  m.ToString() + ".png");
                    Bitmap saveimg = ImageOp.GetSubBitmap(curimg,
                        new Rectangle((int)mesh.viewX, (int)mesh.viewY, (int)mesh.width, (int)mesh.height));
                    saveimg.Save(saveName, ImageFormat.Png);
                    saveimg.Dispose();
                }
            }
        }
  
    }
}
