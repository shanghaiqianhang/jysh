using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace JYShopModel
{
    public class ShopInfo : RootEntity
    {
        [SugarColumn(Length = 50, IsNullable = true)]
        public string ShopCode { get; set; }
        [SugarColumn(Length = 50, IsNullable = true)]
        public string ShopName { get; set; }

        public string ShopAddress { get; set; }
        [SugarColumn(Length = 50, IsNullable = true)]
        public string ShopType { get; set; }
    }
}
