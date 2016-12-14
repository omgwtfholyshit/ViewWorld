using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.Core.Dal;
using ViewWorld.Core.Enum;
using ViewWorld.Core.Models.Identity;
using ViewWorld.Core.Models.ProviderModels;
using ViewWorld.Core.Models.TripModels;
using ViewWorld.Utils;

namespace ViewWorld.Services.Installation
{
    public class InstallService : IInstallService
    {
        readonly IMongoDbRepository Repo;
        public InstallService(IMongoDbRepository _repo)
        {
            this.Repo = _repo;
        }

        public async Task<Result> InsertPermissionStoreData()
        {
            List<Permission> permissionList = new List<Permission>();
            List<PermissionStore> permissionStoreList = new List<PermissionStore>();
            permissionList.Add(new Permission() { Name = "FullAccess", ChineseName = "所有权限", Description = "本系统的最高权限" });
            permissionList.Add(new Permission() { Name = "Region", ChineseName = "区域管理", Description = "管理区域的权限" });
            permissionList.Add(new Permission() { Name = "Trip", ChineseName = "行程管理", Description = "管理行程的权限" });
            permissionList.Add(new Permission() { Name = "Scenery", ChineseName = "景点管理", Description = "管理景点的权限" });
            permissionList.Add(new Permission() { Name = "Memeber", ChineseName = "会员管理", Description = "管理会员的权限" });
            permissionList.Add(new Permission() { Name = "CreditCard", ChineseName = "信用卡管理", Description = "管理信用卡的权限" });
            permissionList.Add(new Permission() { Name = "Provider", ChineseName = "供应商管理", Description = "管理供应商的权限" });
            permissionList.Add(new Permission() { Name = "StartingPoint", ChineseName = "出发地管理", Description = "管理出发地的权限" });
            foreach(var permission in permissionList)
            {
                PermissionStore Pstore = new PermissionStore();
                Pstore.Id = ObjectId.GenerateNewId().ToString();
                Pstore.Permission = permission;
                permissionStoreList.Add(Pstore);
            }
            return await Repo.AddManyAsync(permissionStoreList);
        }

        public async Task<Result> InsertProviderData()
        {
            List<Provider> providerList = new List<Provider>();
            Random rand = new Random();
            for (int i = 0; i < 30; i++)
            {
                string providerName = Tools.Generate_Nickname();
                
                Provider provider = new Provider()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Email = string.Format("{0}@hotmail.com", providerName),
                    Name = providerName,
                    ContactName = providerName,
                    Phone = string.Format("138999{0}", rand.Next(10000, 99999)),
                    AwardRatio = rand.Next(0, 60).ToString(),
                    CommissionRate = rand.Next(0, 40).ToString(),
                    AddedDate = DateTime.Now,
                    UpdatedBy = "Installer",
                    Alias = providerName.Substring(0, 2),
                    IsArchived = false,
                    Description = "TestData",
                    ModifiedDate = DateTime.Now,
                };
                providerList.Add(provider);
            }
            return await Repo.AddManyAsync(providerList);
        }

        public async Task<Result> InsertRegionData()
        {
            List<Region> regionList = new List<Region>();
            Random rand = new Random();
            List<string> mainRegions = "都鲁交吴堡,山竹通明坝,仙朝徐黑洞,依楼溧海峰,句紫清漠山,济大泰流洞,旧武本和城,武云大泽镇".Split(',').ToList();
            List<string> subRegions = "良如港,商丰区,潜镇洞,阴康村,安浮坡,神川洞,桐蔡陂,族奉城,城尚县,州鄱村,盘封岛,桓陵镇,大梅城,峰肇坡,襄罗岛,和左城,阳江城,宜常庄,歙州港,南松区,遂南港,义镇坝,东头村,邹内观,乐黄县,山城谷,台镇镇,山秦洞,两高峰,化富阜,河城洞,大榆乡,图陵崖,集庆坝,子肇堡,间大州,乡城庄,峡春城,河州观,永溪城,安扶坊,东丘峰,皇东城,西台观,张伦坊,凌宜府,兴州县,华太乡,化山港,周河峰,宁川崖,门长道".Split(',').ToList();
            List<string> initials = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',').ToList();
            foreach(var region in mainRegions)
            {
                var listCount = subRegions.Count();
                Region r = new Region()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    EnglishName = "TestData",
                    Initial = initials[rand.Next(0, 25)],
                    IsSubRegion = false,
                    IsVisible = true,
                    Name = region,
                    SortOrder = rand.Next(0, 20),
                    SubRegions = new List<Region>(),
                };
                var srCount = rand.Next(0, listCount / 8);
                //把所有剩余subRegion赋给最后一个MainRegion
                if (region == mainRegions[mainRegions.Count() - 1])
                    srCount = mainRegions.Count();

                string[] srlist = new string[srCount];
                for(int i = 0; i < srCount; i++)
                {
                    int index = rand.Next(0, listCount);
                    srlist[i] = subRegions[index];
                    subRegions.RemoveAt(index);
                    listCount--;
                }
                foreach(var sr in srlist)
                {
                    Region subr = new Region()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        EnglishName = "TestData",
                        Initial = initials[rand.Next(0, 25)],
                        IsSubRegion = true,
                        IsVisible = true,
                        Name = sr,
                        SortOrder = rand.Next(20, 40),
                        ParentRegionId = r.Id,
                    };
                    r.SubRegions.Add(subr);
                }
                regionList.Add(r);
            }
            return await Repo.AddManyAsync(regionList);
        }

        public async Task<Result> InsertSceneryData()
        {
            string scenerynames = "格罗姆高哨站,亡灵海,焚木之桥,哈尔什谷,试炼崖,荆棘王座,火刃之池,群星磨坊,哨兵竞技场,农田之海,永夜礼拜堂,阿拉希灯塔,西瘟疫林地,铸魔堡,刀塔礼拜堂,白骨岭,哈尔什神殿,壁炉修道院,石槌圣地,灰烬港,阿尔科隆山谷,鹰巢礼拜堂,怒风湖,石碑挖掘场,洛丹米尔伐木场,古拉巴什水坝,元素哨岗,蛮锤尖塔,奥蕾莉亚城堡,黑避难所,哨兵营地,哨兵壁垒,幽暗山谷,安戈洛之脊,基尔索罗小屋,希尔斯布莱德海湾,阿拉希之地,南海洞穴,日蚀之林,刺刀森林,雷姆洛斯挖掘场,法力庇护所,守望岗,时光泥潭,法迪尔湖,奥特兰克森林,铁炉伐木场,基尔索罗鸦巢,时光之桥,蛮锤山谷,闪金圣地,";
            scenerynames += "艾森娜洞穴,环礁石林,夜色环型山,提尔荒野,安戈洛洞穴,日蚀神庙,灰之桥,古树海,阿塔哈卡岗哨,天界巢穴,双塔荒野,蕨墙峰,恐怖祭坛,陶拉祖之痕,淤泥瀑布,病木城,沙塔尔营地,影月之门,恐怖王座,林边之池,哈尔什之桥,达隆之岛,血毒林,月光灯塔,月光营地,铸魔沼泽,佐拉姆竞技场,剃刀谷,碧玉之脊,瑟银祭坛,森金城,夜色坟场,壁炉环型山,陶拉祖荒野,古树堡,凄凉小屋,东瘟疫矿洞,巨龙荒野,阿拉希水坝,尼耶尔的荒野,罪恶荒野,葬影之痕,病木之岛,病木堡,阿塔哈卡墓场,巨槌修道院,祖尔树林,厄运瀑布,深沙崖,死亡岭,莫沙彻之地";
            List<string> sceneNamelist = scenerynames.Split(',').ToList();
            List<Scenery> sceneryList = new List<Scenery>();
            List<string> initials = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',').ToList();
            var result = await Repo.GetAllAsync<Region>();
            List<Region> subRegionList = new List<Region>();
            int listCount = subRegionList.Count();
            var rand = new Random();
            foreach (var region in result.Entities)
            {
                subRegionList.AddRange(region.SubRegions);
            }
            foreach(var name in sceneNamelist)
            {
                var subregion = subRegionList[rand.Next(0, listCount)];
                Scenery s = new Scenery()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    EnglishName = "TestData",
                    Initial = initials[rand.Next(0, 25)],
                    Name = name,
                    RegionId = subregion.Id,
                    ParentRegionId = subregion.ParentRegionId,
                    Popularity = rand.Next(0, 10000),
                    PublishedAt = DateTime.Now,
                    Publisher = "TestData",
                    Address = name
                };
                sceneryList.Add(s);
            }
            return await Repo.AddManyAsync(sceneryList);
        }

        public async Task<Result> InsertStartingPointData()
        {
            string names = "金鑫宾馆,雅居宾馆,温馨家园,万客来宾馆,时代宾馆,馨园梦雅,卓怡酒店,颐馨宾馆,恒悦宾馆,怡清园宾馆,沁园宾馆,缘遇宾馆,雅之家,春光小城,四季宾馆,聚缘宾馆,万豪宾馆,合龙宾馆,,朋来宾馆,诚洁宾馆,静园宾馆,福临宾馆,忆家宾馆,嘉诚宾馆,馨缘宾馆,聚贤宾馆,百川宾馆,怡都宾馆,雅艺宾馆,馨雅宾馆";
            List<string> nameList = names.Split(',').ToList();
            List<string> initials = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',').ToList();
            List<StartingPoint> pointList = new List<StartingPoint>();
            var result = Repo.GetAll<Provider>();
            var rand = new Random();
            foreach(var name in nameList)
            {
                var index = rand.Next(0, result.Entities.Count());
                var provider = result.Entities.ElementAt(index);
                StartingPoint point = new StartingPoint()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    IsArchived = false,
                    Address = string.Format("{0}路{1}号", name, rand.Next(0, 999)),
                    Popularity = rand.Next(0, 10000),
                    DepartTime = string.Format("{0}am", rand.Next(0, 12)),
                    AddedDate = DateTime.Now,
                    ProviderAlias = provider.Alias,
                    ProviderId = provider.Id,
                    ProviderName = provider.Name,
                    UpdatedBy = "TestData",
                    AvailableDays = "1,3,5",
                    Landmark = "what's this?",
                    ModifiedDate = DateTime.Now
                };
                pointList.Add(point);
            }
            return await Repo.AddManyAsync(pointList);
        }

        public List<ApplicationUser> GenerateUserList()
        {
            List<ApplicationUser> userList = new List<ApplicationUser>();
            string name = Tools.Generate_Nickname();
            string avatar = "/Images/DefaultImages/UnknownSex.jpg";
            var rand = new Random();
            ApplicationUser admin = new ApplicationUser()
            {
                UserName = "13800000001",
                Email = string.Format("{0}@hotmail.com", name),
                EmailConfirmed = true,
                PhoneNumber = "13800000001",
                PhoneNumberConfirmed = true,
                Avatar = avatar,
                Department = "核心",
                DOB = DateTime.Now,
                Country = "中国",
                NickName = name,
                Points = 5000,
                Sex = SexType.Male,
                RegisteredAt = DateTime.Now
            };
            admin.Roles.Add(UserRole.Admin);
            admin.Permissions.Add(new Permission() { Name = "FullAccess", ChineseName = "所有权限", Description = "本系统的最高权限" });
            userList.Add(admin);

            name = Tools.Generate_Nickname();
            ApplicationUser sale = new ApplicationUser()
            {
                UserName = "13800000002",
                Email = string.Format("{0}@hotmail.com", name),
                EmailConfirmed = true,
                PhoneNumber = "13800000002",
                PhoneNumberConfirmed = true,
                Avatar = avatar,
                Department = "销售",
                DOB = DateTime.Now,
                Country = "中国",
                NickName = name,
                Points = 5000,
                Sex = SexType.Female,
                RegisteredAt = DateTime.Now
            };
            sale.Roles.Add(UserRole.Sales);
            sale.Permissions.Add(new Permission() { Name = "Region", ChineseName = "区域管理", Description = "管理区域的权限" });
            sale.Permissions.Add(new Permission() { Name = "Trip", ChineseName = "行程管理", Description = "管理行程的权限" });
            sale.Permissions.Add(new Permission() { Name = "Scenery", ChineseName = "景点管理", Description = "管理景点的权限" });
            sale.Permissions.Add(new Permission() { Name = "Provider", ChineseName = "供应商管理", Description = "管理供应商的权限" });
            sale.Permissions.Add(new Permission() { Name = "StartingPoint", ChineseName = "出发地管理", Description = "管理出发地的权限" });
            userList.Add(sale);

            name = Tools.Generate_Nickname();
            ApplicationUser sale2 = new ApplicationUser()
            {
                UserName = "13800000003",
                Email = string.Format("{0}@hotmail.com", name),
                EmailConfirmed = true,
                PhoneNumber = "13800000003",
                PhoneNumberConfirmed = true,
                Avatar = avatar,
                Department = "销售",
                DOB = DateTime.Now,
                Country = "中国",
                NickName = name,
                Points = 5000,
                Sex = SexType.Male,
                RegisteredAt = DateTime.Now
            };
            sale2.Roles.Add(UserRole.Sales);
            sale2.Permissions.Add(new Permission() { Name = "Region", ChineseName = "区域管理", Description = "管理区域的权限" });
            sale2.Permissions.Add(new Permission() { Name = "Trip", ChineseName = "行程管理", Description = "管理行程的权限" });
            sale2.Permissions.Add(new Permission() { Name = "Scenery", ChineseName = "景点管理", Description = "管理景点的权限" });
            sale2.Permissions.Add(new Permission() { Name = "Provider", ChineseName = "供应商管理", Description = "管理供应商的权限" });
            sale2.Permissions.Add(new Permission() { Name = "StartingPoint", ChineseName = "出发地管理", Description = "管理出发地的权限" });
            userList.Add(sale2);

            for (int i = 4; i < 7; i++)
            {
                name = Tools.Generate_Nickname();
                ApplicationUser customer = new ApplicationUser()
                {
                    UserName = string.Format("1380000000{0}", i),
                    Email = string.Format("{0}@hotmail.com", name),
                    EmailConfirmed = true,
                    PhoneNumber = string.Format("1380000000{0}",i),
                    PhoneNumberConfirmed = true,
                    Avatar = avatar,
                    Department = "客户",
                    DOB = DateTime.Now,
                    Country = "中国",
                    NickName = name,
                    Points = 5000,
                    Sex = SexType.Female,
                };
                if (i % 2 == 0)
                {
                    customer.Sex = SexType.Male;
                }
                userList.Add(customer);
            }
            return userList;
        }
    }
}
