using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Skript47
{

    public class i3Animation
    {
        public List<Slot> _slot = new List<Slot>();
        
        public i3Animation(byte[] data)
        {
            using (var br = new BinaryReader(new MemoryStream(data)))
            {
                br.ReadByte();
                var vaule = new int[br.ReadInt32()];
                for (int i = 0; i < vaule.Length; i++)
                {
                    var ID = br.ReadInt32(); // ID
                    var name = Encoding.Default.GetString(br.ReadBytes(32));  // название кости
                    _slot.Add(new Slot() { ID = ID, name = name});
                }
            }
        }

        public class Slot 
        {
            public int ID;
            public string name;

            public Slot()
            {

            }
        }

        public override string ToString()
        {
            return string.Format("{0}", _slot.Count);
        }
    }
}
