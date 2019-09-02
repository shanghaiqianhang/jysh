using JYShop.Common;
using JYShop.IServices;
using JYShop.IRepository;
using JYShopModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JYShop.Services
{
    public class UserRoleService : BaseServices<UserRole>, IUserRoleService
    {
        IUserRoleRepository _dal;

        public UserRoleService(IUserRoleRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }

        [Caching(AbsoluteExpiration = 30)]
        public async Task<int> GetRoleIdByUserId(int uid)
        {
            return ((await base.Query(d => d.userId == uid)).OrderByDescending(q => q.roleId).LastOrDefault()?.roleId).ObjToInt();
        }

        public Task<List<UserRole>> GetRoleModule()
        {
            throw new NotImplementedException();
        }

        public async Task<UserRole> SaveUserRole(int uid, int rid)
        {
            UserRole userRole = new UserRole(uid, rid);

            UserRole model = new UserRole();

            var userlist = await base.Query(q => q.userId == userRole.userId && q.roleId == userRole.roleId);
            if (userlist.Count > 0)
            {
                model = userlist.FirstOrDefault();
            }
            else
            {
                var id = await base.Add(userRole);
                model = await base.QueryById(id);
            }


            return  model;
        }
    }
}
