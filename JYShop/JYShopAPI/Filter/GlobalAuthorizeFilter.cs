using JYShop.Common;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JYShopAPI.Filter
{
    public class GlobalRouteAuthorizeConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var c in application.Controllers)
            {
                if (!c.Filters.Any(e => e is AuthorizeFilter))
                {
                    c.Filters.Add(new AuthorizeFilter(Permissions.Name));
                }
                else
                {
                    // 写了特性，[Authorize] 或 [AllowAnonymous] ，根据情况进行权限认证
                }
            }
        }
    }

    /// <summary>
    /// 全局权限过滤器【无效】
    /// </summary>
    public class GlobalAuthorizeFilter : AuthorizeFilter
    {
        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(item => item is IAsyncAuthorizationFilter && item != this))
            {
                return Task.FromResult(0);
            }
            return base.OnAuthorizationAsync(context);
        }
    }
}
