using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewWorld.Models.Trip
{
    public class Region
    {
        public Region()
        {
            SortOrder = 0;
            IsSubRegion = false;
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Initial { get; set; }
        public int SortOrder { get; set; }
        public bool IsSubRegion { get; set; }
        public string ParentRegionId { get; set; }
        public List<Region> SubRegions { get; set; }

    }
}