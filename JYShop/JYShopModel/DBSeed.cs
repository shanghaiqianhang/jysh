using JYShop.Common.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JYShopModel
{
    public class DBSeed
    {
        //原始数据地址
        private static string GitJsonFileFormat = "https://github.com/anjoy8/Blog.Data.Share/raw/master/Blog.Core.Data.json/{0}.tsv";


        public static async Task SeedAsync(MyContext myContext)
        {
            try
            {
                // myContext.Db.DbMaintenance.CreateDatabase();

                myContext.CreateTableByEntity(false,
                    typeof(RoleInfo),
                    typeof(UserInfo),
                    typeof(UserRole),
                    typeof(ShopInfo)
                    );

                // myContext.Db.CodeFirst.InitTables(typeof(UserRole));

                Console.WriteLine("dataBase:shop created success!");
                Console.WriteLine("");

                Console.WriteLine("seed dataBase...");

                if (!await myContext.Db.Queryable<UserInfo>().AnyAsync())
                {
                    myContext.GetEntityDB<UserInfo>().InsertRange(JsonHelper.ParseFormByJson<List<UserInfo>>(GetNetData.Get(string.Format(GitJsonFileFormat, "UserInfo"))));
                    Console.WriteLine("Table:UserInfo created success!");
                }
                else {
                    Console.WriteLine("Table:UserInfo already exists...");
                }

                if (!await myContext.Db.Queryable<RoleInfo>().AnyAsync())
                {
                    myContext.GetEntityDB<RoleInfo>().InsertRange(JsonHelper.ParseFormByJson<List<RoleInfo>>(GetNetData.Get(string.Format(GitJsonFileFormat, "RoleInfo"))));
                    Console.WriteLine("Table:RoleInfo created success!");
                }
                else
                {
                    Console.WriteLine("Table:RoleInfo already exists...");
                }


                if (!await myContext.Db.Queryable<UserRole>().AnyAsync())
                {
                    myContext.GetEntityDB<UserRole>().InsertRange(JsonHelper.ParseFormByJson<List<UserRole>>(GetNetData.Get(string.Format(GitJsonFileFormat, "UserRole"))));
                    Console.WriteLine("Table:UserRole created success!");
                }
                else
                {
                    Console.WriteLine("Table:UserRole already exists...");
                }



                if (!await myContext.Db.Queryable<ShopInfo>().AnyAsync())
                {
                    myContext.GetEntityDB<ShopInfo>().InsertRange(JsonHelper.ParseFormByJson<List<ShopInfo>>(GetNetData.Get(string.Format(GitJsonFileFormat, "ShopInfo"))));
                    Console.WriteLine("Table:ShopInfo created success!");
                }
                else
                {
                    Console.WriteLine("Table:ShopInfo already exists...");
                }

                Console.WriteLine("Done seeding database.");
                Console.WriteLine();

            }
            catch (Exception ex)
            {
                throw new Exception("1、注意要先创建空的数据库\n2、" + ex.Message);
            }
        }
    }
}
