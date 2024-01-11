using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HNSFreeze
{
    public class Config : IBasePluginConfig
    {
        [JsonPropertyName("FreezeTeam")]
        public int FreezeTeam { get; set; } = 2;


        [JsonPropertyName("FreezeTime")]
        public int FreezeTime { get; set; } = 5;

        [JsonPropertyName("FreezeRadius")]
        public int FreezeRadius { get; set; } = 200;

        //[JsonPropertyName("FreezeColour")]
        //public int FreezeColour { get; set; } = ChatColors.Blue;

        [JsonPropertyName("DisableFreeze")]
        public int DisableFreeze { get; set; } = 0;

        [JsonPropertyName("NoFlashTeam")]
        public int NoFlashTeam { get; set; } = 2;

        [JsonPropertyName("DisableNoFlash")]
        public int DisableNoFlash { get; set; } = 0;

        public int Version { get; set; } = 1;
    }
}
