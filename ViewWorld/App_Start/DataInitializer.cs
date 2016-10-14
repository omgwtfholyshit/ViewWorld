
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Models;
using ViewWorld.Models.Trip;

namespace ViewWorld.App_Start
{
    public sealed class DataInitializer
    {
        private const string DB_NAME = Core.Config.db_name;

        //public async Task CreateSceneriesAsync()
        //{
        //    IConnection databaseConnection = RethinkDb.Driver.RethinkDB.R.Connection().Hostname(RethinkDBConstants.DefaultHostname).Port(RethinkDBConstants.DefaultPort).Timeout(60).Connect();
        //    // Get an object to use the database
        //    //IDatabaseQuery DB = Query.Db(DB_NAME);
        //    Db DB = RethinkDB.R.Db(DB_NAME);
        //    var result = await DB.Table("Sceneries").Delete().RunResultAsync(databaseConnection);
        //    var result2 = await DB.Table("Regions").Delete().RunResultAsync(databaseConnection);
        //    var regionLists = new List<Region>
        //    {
        //        new Region {Id="1",Name="北美旅游",EnglishName = "North America",Initial = "B",SortOrder = 0, IsSubRegion = false,SubRegions = new List<Region> {
        //            new Region {Id="2",Name="美东+美西+夏威夷",EnglishName = "EastWestHawaii",Initial = "M",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },
        //            new Region {Id="3",Name="美东+美西",EnglishName = "EastWestAmerica",Initial = "E",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },
        //            new Region {Id="4",Name="美西+墨西哥",EnglishName = "WestMexico",Initial = "W",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },
        //        }},
        //        new Region {Id="5",Name="欧洲旅游",EnglishName = "Europe",Initial = "EU",SortOrder = 0, IsSubRegion = false,SubRegions = new List<Region> {
        //            new Region {Id="6",Name="法意瑞",EnglishName = "RuiItaly",Initial = "F",SortOrder = 0, IsSubRegion = true,ParentRegionId = "5" },
        //            new Region {Id="7",Name="西欧",EnglishName = "WesternEurope",Initial = "X",SortOrder = 0, IsSubRegion = true ,ParentRegionId = "5"},
        //            new Region {Id="8",Name="中/东欧",EnglishName = "CentralEurope",Initial = "Z",SortOrder = 0, IsSubRegion = true,ParentRegionId = "5" },
        //        }},
        //    };
        //    await DB.Table("Regions").Insert(regionLists.ToArray()).RunResultAsync(databaseConnection);
        //    List<Scenery> sceneList = new List<Scenery>
        //    {
        //        new Scenery { Id= "1", Name = "奥萨布尔大峡谷", EnglishName = "Ausable Chasm", Region = new Region { Id = "2", Name = "美东+美西+夏威夷", EnglishName = "EastWestHawaii", Initial = "M", SortOrder = 0, IsSubRegion = true, ParentRegionId = "1" }, Initial = "A", Coordinate =new GeoPoint{ Latitude = "44.523911", Longitude = "-73.460639" } },
        //        new Scenery { Id= "2", Name = "布鲁克斯山脉", EnglishName = "Brooks Range", Region = new Region { Id = "3", Name = "美东+美西", EnglishName = "EastWestAmerica", Initial = "E", SortOrder = 0, IsSubRegion = true, ParentRegionId = "1" }, Initial = "B", Coordinate = new GeoPoint { Latitude = "68.202280", Longitude = "-152.249314" } },
        //        new Scenery { Id= "3", Name = "育空河", EnglishName = "Yukon River", Region = new Region {Id="4",Name="美西+墨西哥",EnglishName = "WestMexico",Initial = "W",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },Initial = "Y",Coordinate= new GeoPoint{Latitude = "62.632311", Longitude = "-164.791816" } },
        //        new Scenery { Id= "4", Name = "奥林匹克博物馆", EnglishName = "Olympic Museum", Region = new Region {Id="8",Name="中/东欧",EnglishName = "CentralEurope",Initial = "Z",SortOrder = 0, IsSubRegion = true,ParentRegionId = "5" },Initial = "Y",Coordinate= new GeoPoint{Latitude = "62.632311", Longitude = "-164.791816" } },
        //    };
        //    await DB.Table("Sceneries").Insert(sceneList.ToArray()).RunResultAsync(databaseConnection);
        //}
        //#region 同步方法
        //[Obsolete]
        //static void CreateSceneries()
        //{
        //    IConnection databaseConnection = RethinkDb.Driver.RethinkDB.R.Connection().Hostname(RethinkDBConstants.DefaultHostname).Port(RethinkDBConstants.DefaultPort).Timeout(60).Connect();
        //    // Get an object to use the database
        //    //IDatabaseQuery DB = Query.Db(DB_NAME);
        //    Db DB = RethinkDB.R.Db(DB_NAME);
        //    var result = DB.Table("Sceneries").Delete().RunResult(databaseConnection);
        //    var result2 = DB.Table("Regions").Delete().RunResult(databaseConnection);
        //    var regionLists = new List<Region>
        //    {
        //        new Region {Id="1",Name="北美旅游",EnglishName = "North America",Initial = "B",SortOrder = 0, IsSubRegion = false,SubRegions = new List<Region> {
        //            new Region {Id="2",Name="美东+美西+夏威夷",EnglishName = "EastWestHawaii",Initial = "M",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },
        //            new Region {Id="3",Name="美东+美西",EnglishName = "EastWestAmerica",Initial = "E",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },
        //            new Region {Id="4",Name="美西+墨西哥",EnglishName = "WestMexico",Initial = "W",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },
        //        }},
        //        new Region {Id="5",Name="欧洲旅游",EnglishName = "Europe",Initial = "EU",SortOrder = 0, IsSubRegion = false,SubRegions = new List<Region> {
        //            new Region {Id="6",Name="法意瑞",EnglishName = "RuiItaly",Initial = "F",SortOrder = 0, IsSubRegion = true,ParentRegionId = "5" },
        //            new Region {Id="7",Name="西欧",EnglishName = "WesternEurope",Initial = "X",SortOrder = 0, IsSubRegion = true ,ParentRegionId = "5"},
        //            new Region {Id="8",Name="中/东欧",EnglishName = "CentralEurope",Initial = "Z",SortOrder = 0, IsSubRegion = true,ParentRegionId = "5" },
        //        }},
        //    };
        //    // await DB.Table("Regions").Insert(regionLists.ToArray()).RunResultAsync(databaseConnection);
        //    List<Scenery> sceneList = new List<Scenery>
        //    {
        //        new Scenery { Id= "1", Name = "奥萨布尔大峡谷", EnglishName = "Ausable Chasm", Region = new Region { Id = "2", Name = "美东+美西+夏威夷", EnglishName = "EastWestHawaii", Initial = "M", SortOrder = 0, IsSubRegion = true, ParentRegionId = "1" }, Initial = "A", Coordinate =new GeoPoint{ Latitude = "44.523911", Longitude = "-73.460639" } },
        //        new Scenery { Id= "2", Name = "布鲁克斯山脉", EnglishName = "Brooks Range", Region = new Region { Id = "3", Name = "美东+美西", EnglishName = "EastWestAmerica", Initial = "E", SortOrder = 0, IsSubRegion = true, ParentRegionId = "1" }, Initial = "B", Coordinate = new GeoPoint { Latitude = "68.202280", Longitude = "-152.249314" } },
        //        new Scenery { Id= "3", Name = "育空河", EnglishName = "Yukon River", Region = new Region {Id="4",Name="美西+墨西哥",EnglishName = "WestMexico",Initial = "W",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },Initial = "Y",Coordinate= new GeoPoint{Latitude = "62.632311", Longitude = "-164.791816" } },
        //        new Scenery { Id= "4", Name = "奥林匹克博物馆", EnglishName = "Olympic Museum", Region = new Region {Id="8",Name="中/东欧",EnglishName = "CentralEurope",Initial = "Z",SortOrder = 0, IsSubRegion = true,ParentRegionId = "5" },Initial = "Y",Coordinate= new GeoPoint{Latitude = "62.632311", Longitude = "-164.791816" } },
        //    };
         
        //    //new Scenery {Name = "奥萨布尔大峡谷",EnglishName = "Ausable Chasm", Region = new Region {Id="2",Name="美东+美西+夏威夷",EnglishName = "EastWestHawaii",Initial = "M",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" } ,Initial = "A",Coordinate= {Latitude = "44.523911", Longitude = "-73.460639" } },
        //    //new Scenery {Name = "布鲁克斯山脉",EnglishName = "Brooks Range", Region =new Region {Id="3",Name="美东+美西",EnglishName = "EastWestAmerica",Initial = "E",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" } ,Initial = "B",Coordinate= {Latitude = "68.202280", Longitude = "-152.249314" } },
        //    //new Scenery {Name = "育空河",EnglishName = "Yukon River", Region = new Region {Id="4",Name="美西+墨西哥",EnglishName = "WestMexico",Initial = "W",SortOrder = 0, IsSubRegion = true,ParentRegionId = "1" },Initial = "Y",Coordinate= {Latitude = "62.632311", Longitude = "-164.791816" } },
        //    // new Scenery {Name = "奥林匹克博物馆",EnglishName = "Olympic Museum", Region = new Region {Id="8",Name="中/东欧",EnglishName = "CentralEurope",Initial = "Z",SortOrder = 0, IsSubRegion = true,ParentRegionId = "5" },Initial = "Y",Coordinate= {Latitude = "62.632311", Longitude = "-164.791816" } },
        //    var res = DB.Table("Sceneries").Insert(sceneList.ToArray()).RunResult(databaseConnection);
        //}
        //#endregion
        public static void Init()
        {
           // CreateSceneries();
           // Task.Run(new Action(async () => await new DataInitializer().CreateSceneriesAsync()));
        }
    }
}