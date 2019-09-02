using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JYShop.Common.Helper;
using JYShop.IServices;
using JYShopAPI.AuthHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JYShopAPI.Controllers
{
    /// <summary>
    /// 登录管理【无权限】
    /// </summary>
    [Produces("application/json")]
    [Route("api/Login")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        readonly IUserInfoService _userInfoService;
        readonly IUserRoleService _userRoleService;
        readonly IRoleInfoService _roleService;
        readonly PermissionRequirement _requirement;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="sysUserInfoServices"></param>
        /// <param name="userRoleServices"></param>
        /// <param name="roleServices"></param>
        /// <param name="requirement"></param>
        public LoginController(IUserInfoService sysUserInfoServices, IUserRoleService userRoleServices, IRoleInfoService roleServices, PermissionRequirement requirement)
        {
            this._userInfoService = sysUserInfoServices;
            this._userRoleService = userRoleServices;
            this._roleService = roleServices;
            _requirement = requirement;
        }

        /// <summary>
        /// 测试 MD5 加密字符串
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MD5Password")]
        public string MD5Password(string password = "")
        {
            return MD5Helper.MD5Encrypt32(password);
        }

        /// <summary>
        /// 获取JWT的方法1
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Token")]
        public async Task<object> GetJwtStr(string name, string pwd)
        {
            string jwtStr = string.Empty;
            bool suc = false;

            var users = await _userInfoService.Query(q => q.UserName == name && q.PWD == MD5Helper.MD5Encrypt32(pwd));
            if (users != null && users.Count > 0)
            {
                TokenModelJwt tokenModel = new TokenModelJwt() { Uid = 1, Role = "Admin" };
                jwtStr = JwtHelper.IssueJwt(tokenModel);
                suc = true;
            }
            else
            {
                jwtStr = "login fail!!!";
            }

            return Ok(new
            {
                success = suc,
                token = jwtStr
            });
        }

        /// <summary>
        /// 获取JWT的方法2：给Nuxt提供
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTokenNuxt")]
        public object GetJwtStrForNuxt(string name, string pwd)
        {
            string jwtStr = string.Empty;
            bool suc = false;
            //这里就是用户登陆以后，通过数据库去调取数据，分配权限的操作
            //这里直接写死了
            if (name == "admin" && pwd == "admin")
            {
                TokenModelJwt tokenModel = new TokenModelJwt
                {
                    Uid = 1,
                    Role = "Admin"
                };

                jwtStr = JwtHelper.IssueJwt(tokenModel);
                suc = true;
            }
            else
            {
                jwtStr = "login fail!!!";
            }
            
            return Ok(new
            {
                success = suc,
                data = new { success = suc, token = jwtStr }
            });
        }
    }
}