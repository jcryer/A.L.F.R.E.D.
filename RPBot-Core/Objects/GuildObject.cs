using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class GuildObject
    {
        public class RootObject
        {
            public RootObject(int id, string name)
            {
                this.Id = id;
                this.Name = name;
            }

            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatSheetObject
        {
            public string Name;
            public int Xp;

            public StatSheetObject(string name, int xp)
            {
                Name = name;
                Xp = xp;
            }
        }
    }
}
