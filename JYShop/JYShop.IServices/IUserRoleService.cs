using JYShopModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JYShop.IServices
{
    public interface IUserRoleService : IBaseServices<UserRole>
    {
        Task<UserRole> SaveUserRole(int uid, int rid);

        Task<int> GetRoleIdByUserId(int uid);

        
        /// <summary>
        /// 获取全部 角色接口(按钮)关系数据   
        /// </summary>
        /// <returns></returns>
        Task<List<UserRole>> GetRoleModule();//RoleModulePermission
    }
}
