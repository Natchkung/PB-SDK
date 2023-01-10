using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace Skript47
{
    public class i3PackNode
    {
        public string name;
        public int[] unknown;
        public byte[] packType;
        public int parrent;
        public int fileCount;
        public int fileStep;
        public int fileType;
        public List<I3S.Pack> _pack = new List<I3S.Pack>();

        public i3PackNode(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                br.ReadInt32(); // Тип файла
                br.ReadInt32();
                br.ReadInt32();

                unknown = new int[br.ReadInt32()]; // Неизвестное что то
                for (int i = 0; i < unknown.Length; i++)
                {
                    unknown[i] = br.ReadInt32();
                }
                br.ReadBytes(56);
                parrent = br.ReadInt32();  // Ссылка на родительский елемент

                packType = br.ReadBytes(8); // Описание типа и структуры
                I3S.DecryptB(packType, 3);
                fileType = packType[3];
                fileCount = BitConverter.ToInt16(packType, 4);
                fileStep = fileType == 50 ? 92 : 76;
            }
        }

        public i3PackNode(byte[] data, byte[] i3sBody, int blockID)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                name = br.ReadString();
                var unt = br.ReadInt32(); // Тип файла
                br.ReadInt32();
                br.ReadInt32();

                unknown = new int[br.ReadInt32()]; // Неизвестное что то
                for (int i = 0; i < unknown.Length; i++)
                {
                    unknown[i] = br.ReadInt32();
                }
                br.ReadBytes(56);
                parrent = br.ReadInt32();  // Ссылка на родительский елемент

                packType = br.ReadBytes(8); // Описание типа и структуры

                I3S.DecryptB(packType, 3);
                fileType = packType[3];
                fileCount = BitConverter.ToInt16(packType, 4);
                fileStep = fileType == 50 ? 92 : 76;

                for (int i = 0; i < fileCount; i++)
                {
                    var tempF = br.ReadBytes(fileStep);
                    I3S.DecryptB(tempF, 2);

                    string nameF = Regex.Split(Encoding.Default.GetString(tempF, 0, 52), "\0")[0];

                    int startF = 0;
                    int sizeF = 0;

                    if (fileStep == 76)
                    {
                        startF = BitConverter.ToUInt16(tempF, 56) * 65536 + BitConverter.ToUInt16(tempF, 64);
                        sizeF = BitConverter.ToUInt16(tempF, 58) * 65536 + BitConverter.ToUInt16(tempF, 54);
                    }
                    if (fileStep == 92)
                    {
                        startF = BitConverter.ToUInt16(tempF, 72) * 65536 + BitConverter.ToUInt16(tempF, 80);
                        sizeF = BitConverter.ToUInt16(tempF, 74) * 65536 + BitConverter.ToUInt16(tempF, 70);
                    }

                    var fileData = new byte[0];
                    try
                    {
                        fileData = new byte[sizeF];
                        Array.Copy(i3sBody, startF, fileData, 0, fileData.Length);
                    }
                    catch
                    {

                    }

                    var ThisPack = new I3S.Pack(blockID, startF, name, nameF, fileData);
                    _pack.Add(ThisPack);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (ParrentID = {1} Files = {2} Type = {4})", name, parrent, fileCount, fileStep, fileType);
        }
    }
}
