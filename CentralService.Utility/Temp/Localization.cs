using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Utility.Temp
{
    public struct Localization
    {
        public Region RegionCode { get; set; }
        public string LocalGameId { get; set; }
        public string LocalizedName { get; set; }

        public Localization(Region RegionCode, string LocalGameId, string LocalizedName)
        {
            this.RegionCode = RegionCode;
            this.LocalGameId = LocalGameId;
            this.LocalizedName = LocalizedName;
        }
    }
}
