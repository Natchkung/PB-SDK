using System;

namespace Skript47
{
    public class i3Skeleton
    {
        public int BoneMatrixListAttr;

        public i3Skeleton(byte []data)
        {
            BoneMatrixListAttr = BitConverter.ToInt32(data, 8);
        }

        public override string ToString()
        {
            return string.Format("i3BoneMatrixListAttr: {0}", BoneMatrixListAttr);
        }
    }
}