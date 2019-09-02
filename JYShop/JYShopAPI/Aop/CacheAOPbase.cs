using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JYShopAPI.Aop
{
    public abstract class CacheAOPbase : IInterceptor
    {
        public abstract void Intercept(IInvocation invocation);

        protected string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();

            string key = $"{typeName}:{methodName}";
            foreach (var parm in methodArguments)
            {
                key = $"{key}{parm}:";
            }
            return key.TrimEnd(':');
        }

        protected static string GetArgumentValue(object arg)
        {
            if (arg is DateTime || arg is DateTime?)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");
            if (arg is string || arg is ValueType || arg is Nullable)
                return arg.ToString();
            if (arg != null)
            {
                if (arg.GetType().IsClass)
                {
                    return JYShop.Common.Helper.MD5Helper.MD5Encrypt16(Newtonsoft.Json.JsonConvert.SerializeObject(arg));
                }
            }

            return string.Empty;
        }
    }
}
