using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace JYShopModel
{
    public class RoleInfo : RootEntity
    {
        [SugarColumn(IsNullable = true)]
        public int roleId { get; set; }
        [SugarColumn(Length = 50, IsNullable = true)]
        public string RoleName { get; set; }
        public int IsDelete { get; set; }

    }
}
