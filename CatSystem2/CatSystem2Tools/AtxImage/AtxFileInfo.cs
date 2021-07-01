//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;


//namespace AtxImage
//{
//    // Token: 0x02000002 RID: 2
//    public class AtxFileInfo
//    {
//        // Token: 0x17000001 RID: 1
//        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
//        // (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
//        public AtxImageBase atxImage { get; protected set; } = null;

//        // Token: 0x17000002 RID: 2
//        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
//        // (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
//        public string FileName { get; protected set; } = null;

//        // Token: 0x17000003 RID: 3
//        // (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
//        // (set) Token: 0x06000006 RID: 6 RVA: 0x0000207A File Offset: 0x0000027A
//        public string BaseName { get; protected set; } = "";

//        // Token: 0x17000004 RID: 4
//        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002083 File Offset: 0x00000283
//        // (set) Token: 0x06000008 RID: 8 RVA: 0x0000208B File Offset: 0x0000028B
//        public Vector2 Anchor { get; private set; } = Vector2.zero;

//        // Token: 0x17000005 RID: 5
//        // (get) Token: 0x06000009 RID: 9 RVA: 0x00002094 File Offset: 0x00000294
//        // (set) Token: 0x0600000A RID: 10 RVA: 0x0000209C File Offset: 0x0000029C
//        public bool PositionMode { get; private set; } = false;

//        // Token: 0x17000006 RID: 6
//        // (get) Token: 0x0600000B RID: 11 RVA: 0x000020A5 File Offset: 0x000002A5
//        // (set) Token: 0x0600000C RID: 12 RVA: 0x000020AD File Offset: 0x000002AD
//        public Rect BoundingBox { get; private set; }

//        // Token: 0x17000007 RID: 7
//        // (get) Token: 0x0600000D RID: 13 RVA: 0x000020B6 File Offset: 0x000002B6
//        // (set) Token: 0x0600000E RID: 14 RVA: 0x000020BE File Offset: 0x000002BE
//        public int TextureCount { get; private set; }

//        // Token: 0x0600000F RID: 15 RVA: 0x000020C8 File Offset: 0x000002C8
//        public AtxFileInfo(string dif, ImageFlipAttribute flip)
//        {
//            this.SetLoadAtx(dif, flip);
//        }

//        // Token: 0x06000010 RID: 16 RVA: 0x00002130 File Offset: 0x00000330
//        public AtxFileInfo(AtxImageBase atx, string filename, int id, ImageFlipAttribute flip, bool posMode)
//        {
//            this.Clear();
//            this.imageFlipAttribute = (flip ?? new ImageFlipAttribute());
//            this.FileName = filename;
//            this.loadType = AtxFileInfo.LoadType.Atx;
//            this.atxImage = atx;
//            this.PositionMode = posMode;
//            this.Names.Add(new ValueTuple<string, int>(filename, id));
//        }

//        // Token: 0x06000011 RID: 17 RVA: 0x000021D8 File Offset: 0x000003D8
//        public void SetLoadAtx(string dif, ImageFlipAttribute flip)
//        {
//            this.Clear();
//            this.imageFlipAttribute = (flip ?? new ImageFlipAttribute());
//            this.FileName = dif;
//            this.loadType = AtxFileInfo.LoadType.File;
//        }

//        // Token: 0x06000012 RID: 18 RVA: 0x00002201 File Offset: 0x00000401
//        private void Clear()
//        {
//            this.Names.Clear();
//        }

//        // Token: 0x06000013 RID: 19 RVA: 0x00002210 File Offset: 0x00000410
//        private bool BeforeLoad()
//        {
//            this.atxImage = this.LoadAtxImage(this.FileName);
//            bool flag = this.atxImage == null;
//            bool result;
//            if (flag)
//            {
//                result = false;
//            }
//            else
//            {
//                string[] array = this.MakeDifImageFileArray(this.FileName);
//                foreach (string item in array)
//                {
//                    this.Names.Add(new ValueTuple<string, int>(item, -1));
//                }
//                result = true;
//            }
//            return result;
//        }

//        // Token: 0x06000014 RID: 20 RVA: 0x00002288 File Offset: 0x00000488
//        public bool Load()
//        {
//            bool flag = this.loadType == AtxFileInfo.LoadType.File;
//            if (flag)
//            {
//                bool flag2 = !this.BeforeLoad();
//                if (flag2)
//                {
//                    return false;
//                }
//            }
//            bool flag3 = this.atxImage == null;
//            bool result;
//            if (flag3)
//            {
//                result = false;
//            }
//            else
//            {
//                AtxMeshGroup atxMeshGroup = this.NewAtxMeshGroup();
//                atxMeshGroup.Add(this);
//                this.BoundingBox = atxMeshGroup.GetBoundingBox();
//                atxMeshGroup.GetRectangleList(this.RectangleList);
//                this.Anchor = atxMeshGroup.GetAnchor();
//                int vertex = atxMeshGroup.GetVertex(out this.StartIndex, out this.Vertices, out this.Uvs, out this.IndexList, out this.TextureSets);
//                this.TextureCount = (from x in this.TextureSets
//                                     select x.TextureIndexList.Count).Sum();
//                atxMeshGroup.Dispose();
//                result = true;
//            }
//            return result;
//        }

//        // Token: 0x06000015 RID: 21 RVA: 0x00002374 File Offset: 0x00000574
//        public virtual AtxMeshGroup NewAtxMeshGroup()
//        {
//            return new AtxMeshGroup();
//        }

//        // Token: 0x06000016 RID: 22 RVA: 0x0000238C File Offset: 0x0000058C
//        public AtxImageBase LoadAtxImage(string dif)
//        {
//            string baseName = this.GetBaseName(dif);
//            bool flag = baseName == null;
//            AtxImageBase result;
//            if (flag)
//            {
//                result = null;
//            }
//            else
//            {
//                bool flag2 = this.BaseName == baseName && this.atxImage != null;
//                if (flag2)
//                {
//                    result = this.atxImage;
//                }
//                else
//                {
//                    this.atxImage = this.ReadAtxImageFromFile(baseName);
//                    bool flag3 = this.atxImage == null;
//                    if (flag3)
//                    {
//                        result = null;
//                    }
//                    else
//                    {
//                        result = this.atxImage;
//                    }
//                }
//            }
//            return result;
//        }

//        // Token: 0x06000017 RID: 23 RVA: 0x00002404 File Offset: 0x00000604
//        protected virtual AtxImageBase ReadAtxImageFromFile(string baseName)
//        {
//            FileManager instance = SingletonClass<FileManager>.Instance;
//            return instance.ReadAtxImage("IMAGE", baseName, !this.IsTextureReadable);
//        }

//        // Token: 0x06000018 RID: 24 RVA: 0x00002434 File Offset: 0x00000634
//        protected string GetBaseName(string name)
//        {
//            string fileName = Path.GetFileName(name);
//            string[] array = fileName.Split(new char[]
//            {
//                ','
//            });
//            bool flag = array.Length == 0;
//            string result;
//            if (flag)
//            {
//                result = null;
//            }
//            else
//            {
//                bool flag2 = array.Length == 1;
//                if (flag2)
//                {
//                    result = array[0];
//                }
//                else
//                {
//                    result = array[0] + array[1];
//                }
//            }
//            return result;
//        }

//        // Token: 0x06000019 RID: 25 RVA: 0x0000248C File Offset: 0x0000068C
//        protected string[] MakeDifImageFileArray(string name)
//        {
//            string directoryName = Path.GetDirectoryName(name);
//            string extension = Path.GetExtension(name);
//            string text = Path.GetFileName(name).ToLower();
//            string[] array = text.Split(new char[]
//            {
//                ','
//            });
//            bool flag = array.Length == 0;
//            string[] result;
//            if (flag)
//            {
//                result = null;
//            }
//            else
//            {
//                bool flag2 = array.Length == 1;
//                if (flag2)
//                {
//                    result = array;
//                }
//                else
//                {
//                    int num = 0;
//                    for (int i = 1; i < array.Length; i++)
//                    {
//                        bool flag3 = array[i] != "0";
//                        if (flag3)
//                        {
//                            num++;
//                        }
//                    }
//                    bool flag4 = num == 0;
//                    if (flag4)
//                    {
//                        result = null;
//                    }
//                    else
//                    {
//                        List<string> list = new List<string>();
//                        int num2 = array[0].Length + 1;
//                        int j = 1;
//                        int num3 = 0;
//                        while (j < array.Length)
//                        {
//                            bool flag5 = array[j] != "0";
//                            if (flag5)
//                            {
//                                bool flag6 = j == 1;
//                                if (flag6)
//                                {
//                                    this.BaseName = array[0] + array[1];
//                                    list.Add(this.BaseName);
//                                }
//                                else
//                                {
//                                    bool flag7 = array[j].Length > 0;
//                                    if (flag7)
//                                    {
//                                        list.Add(string.Format("{0}_{1}", j - 1, array[j]));
//                                    }
//                                }
//                                num3++;
//                            }
//                            j++;
//                        }
//                        result = list.ToArray();
//                    }
//                }
//            }
//            return result;
//        }

//        // Token: 0x04000002 RID: 2
//        public ImageFlipAttribute imageFlipAttribute;

//        // Token: 0x04000005 RID: 5
//        [TupleElementNames(new string[]
//        {
//            "name",
//            "id"
//        })]
//        public List<ValueTuple<string, int>> Names = new List<ValueTuple<string, int>>();

//        // Token: 0x04000007 RID: 7
//        public bool IsTextureReadable = false;

//        // Token: 0x0400000A RID: 10
//        public List<RectInt> RectangleList = new List<RectInt>();

//        // Token: 0x0400000B RID: 11
//        public AtxMeshGroup.TextureSet[] TextureSets;

//        // Token: 0x0400000D RID: 13
//        public int[] StartIndex;

//        // Token: 0x0400000E RID: 14
//        public List<Vector3> Vertices;

//        // Token: 0x0400000F RID: 15
//        public List<Vector2> Uvs;

//        // Token: 0x04000010 RID: 16
//        public List<int> IndexList;

//        // Token: 0x04000011 RID: 17
//        private AtxFileInfo.LoadType loadType;

//        // Token: 0x02000020 RID: 32
//        private enum LoadType
//        {
//            // Token: 0x0400009E RID: 158
//            File,
//            // Token: 0x0400009F RID: 159
//            Atx
//        }
//    }
//}
