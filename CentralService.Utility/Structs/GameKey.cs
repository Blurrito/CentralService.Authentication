using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Utility.Structs
{
    internal struct GameKey
    {
        public string GameName { get; set; }
        public string InternalName { get; set; }
        public string Key { get; set; }

        public GameKey(string GameName, string InternalName, string Key)
        {
            this.GameName = GameName;
            this.InternalName = InternalName;
            this.Key = Key;
        }
    }
}
