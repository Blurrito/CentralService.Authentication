using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Utility.Temp
{
    public static class GameProfileCollection
    {
        public static readonly List<GameProfile> GameProfiles = new List<GameProfile>
        {
            new GameProfile("ADA", "ADAJ", "pokemondpds", "1vTlwb", 0, Platform.DS, new List<Localization>()
            {
                new Localization(Region.USen, "ADAE", "Pokémon Diamond")
            }),
            new GameProfile("APA", "ADAJ", "pokemondpds", "1vTlwb", 0, Platform.DS, new List<Localization>()
            {
                new Localization(Region.USen, "APAE", "Pokémon Pearl")
            }),
            new GameProfile("CPU", "ADAJ", "pokemonplatds", "IIup73", 0, Platform.DS, new List<Localization>()
            {
                new Localization(Region.USen, "CPUE", "Pokémon Platinum")
            }),
            new GameProfile("IPK", "ADAJ", "", "", 0, Platform.DS, new List<Localization>()
            {
                new Localization(Region.USen, "IPKE", "Pokémon HeartGold")
            }),
            new GameProfile("IPG", "ADAJ", "", "", 0, Platform.DS, new List<Localization>()
            {
                new Localization(Region.USen, "IPGE", "Pokémon SoulSilver")
            })
        };
    }
}
