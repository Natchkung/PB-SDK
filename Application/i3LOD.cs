using System;

namespace Skript47
{
    public class i3LOD
    {
        public int skeleton;
        public int boneRef;
        public int memoryBuffer;

        public i3LOD(byte[] data)
        {
            skeleton = BitConverter.ToInt32(data, 8);
            boneRef = BitConverter.ToInt32(data, 16);
            memoryBuffer = BitConverter.ToInt32(data, 24);
        }

        public override string ToString()
        {
            return string.Format("i3Skeleton:{0}, i3BoneRef:{1}, i3MemoryBuffer:{2}", skeleton, boneRef, memoryBuffer);
        }
    }
}
