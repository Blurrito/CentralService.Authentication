using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Utility.Temp
{
    public struct GameProfile
    {
        public string GameId { get; set; }
        public string FriendCodeId { get; set; }
        public string InternalName { get; set; }
        public string GameKey { get; set; }
        public int Console { get; set; }
        public Platform SpecificConsole { get; set; }
        public List<Localization> LocalizedNames { get; set; }

        public GameProfile(string GameId, string FriendCodeId, string InternalName, string GameKey, int Console, Platform SpecificConsole, List<Localization> LocalizedNames)
        {
            this.GameId = GameId;
            this.FriendCodeId = FriendCodeId;
            this.InternalName = InternalName;
            this.GameKey = GameKey;
            this.Console = Console;
            this.SpecificConsole = SpecificConsole;
            this.LocalizedNames = LocalizedNames;
        }
    }
}
