using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaCert.RDS.Repository.UnitTests
{
    [TestFixture]
    public class RepositoryTests
    {
        TESTDBContext1 dbContext;
        TETSDBContext2 snapshotDbContext;
        AssetHierarchyLevel[] assetHierarchyLevels;
        BridgeAssetHierarchy[] bridgeAssetHierarchies;
        DimFund[] funds;
        DimEntity[] entities;

        [SetUp]
        public void TestSetup()
        {
            dbContext = new TESTDBContext1(
                new DbContextOptionsBuilder<TESTDBContext1>()
                    .UseInMemoryDatabase("test").Options);
            snapshotDbContext = new TETSDBContext2(
                    new DbContextOptionsBuilder<TETSDBContext2>()
                    .UseInMemoryDatabase("snapshot").Options);

            funds = new[]
            {
                new DimFund { FundKey = "Unknown", FundName = "Unknown"},
                new DimFund { FundKey = "Fund1", FundName = "AustralianSuper"},
            };

            entities = new[] 
            {
                new DimEntity { TargetKey = 0, TargetType = "DimCurrencyClassification" },
                new DimEntity { TargetKey = 1, TargetType = "DimCurrencyClassification" },
                new DimEntity { TargetKey = 2, TargetType = "DimCurrencyClassification" },
                new DimEntity { TargetKey = 3, TargetType = "Sub Asset Class" },
                new DimEntity { TargetKey = 4, TargetType = "Sub Asset Class" },
                new DimEntity { TargetKey = 5, TargetType = "Sub Asset Class" },
                new DimEntity { TargetKey = 6, TargetType = "Sub Asset Class" },
            };

            assetHierarchyLevels = new[]
            {
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 1", Level = 1 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 2", Level = 2 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 3", Level = 3 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 4", Level = 4 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 5", Level = 5 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 6", Level = 6 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 7", Level = 7 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 8", Level = 8 },
                new AssetHierarchyLevel { Name = "AssetHierarchyLevel 9", Level = 9 },
            };

            bridgeAssetHierarchies = new[]
            {
                new BridgeAssetHierarchy { ParentKey = 1, ChildKey = 2, FundKey = "Fund1", AssetHierarchyLevelKey = 1, CreatedBy = "user", ModifiedBy = "user" },
                new BridgeAssetHierarchy { ParentKey = 1, ChildKey = 3, FundKey = "Fund1", AssetHierarchyLevelKey = 1, CreatedBy = "user", ModifiedBy = "user" },
                new BridgeAssetHierarchy { ParentKey = 4, ChildKey = 5, FundKey = "Fund1", AssetHierarchyLevelKey = 1, CreatedBy = "user", ModifiedBy = "user" },
                new BridgeAssetHierarchy { ParentKey = 4, ChildKey = 6, FundKey = "Fund1", AssetHierarchyLevelKey = 1, CreatedBy = "user", ModifiedBy = "user" },
                new BridgeAssetHierarchy { ParentKey = 4, ChildKey = 7, FundKey = "Fund1", AssetHierarchyLevelKey = 1, CreatedBy = "user", ModifiedBy = "user" },
            };

        }
        [TearDown]
        public void TestTearDown()
        {
        }

        [Test]
        public void SingleInsertTest()
        {
            using (var unitOfWork = new UnitOfWork<AlphacertSnapshotContext>(snapshotDbContext))
            {
                var repo = unitOfWork.Repository<ViewCountry>();

                repo.Insert(new ViewCountry { Code = "NZ", Name = "New Zealand", CurrencyCode = "NZD" });
                var i = unitOfWork.SaveChanges();
                var count = repo.Query.Count();
                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void MultipleInsterTest()
        {
            using (var unitOfWork = new UnitOfWork<AlphacertODSContext>(dbContext))
            {
                var repo = unitOfWork.Repository<AssetHierarchyLevel>();

                repo.Insert(assetHierarchyLevels);
                var i = unitOfWork.SaveChanges();
                var count = repo.Query.Count();
                Assert.AreEqual(assetHierarchyLevels.Length, count);

                // keys were automatically generated
                var keys = repo.Query.Select(a => a.AssetHierarchyLevelKey).ToList();
                Assert.AreEqual(1, keys.FirstOrDefault());
            }
        }

        [Test]
        public async Task RelationalDataTest()
        {
            using (var unitOfWork = new UnitOfWork<AlphacertODSContext>(dbContext))
            {
                var assetHierarchyLevelRepo = unitOfWork.Repository<AssetHierarchyLevel>();
                var dimFundRepo = unitOfWork.Repository<DimFund>();
                var dimEntityRepo = unitOfWork.Repository<DimEntity>();
                var bridgeAssetHierarchyRepo = unitOfWork.Repository<BridgeAssetHierarchy>();

                await assetHierarchyLevelRepo.InsertAsync(assetHierarchyLevels);
                await dimFundRepo.InsertAsync(funds);
                await dimEntityRepo.InsertAsync(entities);
                await bridgeAssetHierarchyRepo.InsertAsync(bridgeAssetHierarchies);
                var i = unitOfWork.SaveChanges();
                Assert.AreEqual(i, assetHierarchyLevels.Count() + funds.Count() + entities.Count() + bridgeAssetHierarchies.Count());
                
                var entity1 = dimEntityRepo.Query.FirstOrDefault();
                var entity2 = dimEntityRepo.Query.Skip(3).FirstOrDefault();
                var fund = dimFundRepo.Query.FirstOrDefault(f => f.FundKey == "Fund1");

                var level = assetHierarchyLevelRepo.Query.FirstOrDefault();
                Assert.IsEmpty(level.BridgeAssetHierarchies);

                // test Include
                level = assetHierarchyLevelRepo.Query.Include(l => l.BridgeAssetHierarchies).FirstOrDefault();
                Assert.IsNotEmpty(level.BridgeAssetHierarchies);
                Assert.IsNull(level.BridgeAssetHierarchies.FirstOrDefault().FundKeyNavigation);
                
                //Test Include and ThenInclude 
                level = assetHierarchyLevelRepo.Query.Include(l => l.Include(a => a.BridgeAssetHierarchies).ThenInclude(b => b.FundKeyNavigation)).FirstOrDefault();
                Assert.IsNotEmpty(level.BridgeAssetHierarchies);
                Assert.IsNotNull(level.BridgeAssetHierarchies.FirstOrDefault().FundKeyNavigation);
                Assert.AreEqual("Fund1", level.BridgeAssetHierarchies.FirstOrDefault().FundKeyNavigation.FundKey);
            }
        }
    }
}
