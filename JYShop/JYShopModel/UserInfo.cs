using SqlSugar;
using System;

namespace JYShopModel
{
    public class UserInfo : RootEntity
    {
        public int UserId { get; set; }
        [SugarColumn(Length = 50, IsNullable = true)]
        public string UserName { get; set; }
        [SugarColumn(Length = 50, IsNullable = true)]
        public string PWD { get; set; }

    }
}
