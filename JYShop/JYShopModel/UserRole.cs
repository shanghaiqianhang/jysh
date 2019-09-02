using System;
using System.Collections.Generic;
using System.Text;

namespace JYShopModel
{
    public class UserRole : RootEntity
    {
        public UserRole()
        {

        }
        public UserRole(int uid, int rid)
        {
            this.userId = uid;
            this.roleId = rid;
            this.IsDelete = 0;
        }
        public int userId { get; set; }

        public int roleId { get; set; }

        public int IsDelete { get; set; }
    }
}
