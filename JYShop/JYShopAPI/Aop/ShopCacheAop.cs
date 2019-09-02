using Castle.DynamicProxy;
using JYShop.Common;
using JYShop.Common.MemoryCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JYShopAPI.Aop
{
    public class ShopCacheAop : CacheAOPbase
    {
        private readonly ICaching _cache;
        public ShopCacheAop(ICaching cache)
        {
            _cache = cache;
        }

        public override void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) is CachingAttribute qCachingAttribute)
            {
                var cachekey = CustomCacheKey(invocation);
                var cachevalue = _cache.Get(cachekey);
                if (cachevalue != null)
                {
                    invocation.ReturnValue = cachevalue;
                    return;
                }
                invocation.Proceed();

                if (!string.IsNullOrWhiteSpace(cachekey))
                {
                    _cache.Set(cachekey, invocation.ReturnValue);
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
