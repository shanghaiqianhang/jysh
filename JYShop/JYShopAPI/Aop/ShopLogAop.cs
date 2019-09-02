using Castle.DynamicProxy;
using JYShop.Common.Hubs;
using JYShop.Common.LogHelper;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JYShopAPI.Aop
{
    public class ShopLogAop : IInterceptor
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ShopLogAop(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Intercept(IInvocation invocation)
        {
            var dateIntecept = "" + $"【当前执行方法】：{invocation.Method.Name}\r\n" +
               $"【携带的参数有】：{string.Join(",", invocation.Arguments.Select(q => (q ?? "").ToString()).ToArray())}\r\n";
            try
            {
                MiniProfiler.Current.Step($"执行Service方法：{invocation.Method.Name}() -> ");

                invocation.Proceed();

                if (IsAsynMethod(invocation.Method))
                {
                    if (invocation.Method.ReturnType == typeof(Task))
                    {
                        invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                            invocation.Method.ReturnType.GenericTypeArguments[0],
                            invocation.ReturnValue,
                            async () => await TestActionsAsync(invocation),
                            ex =>
                            {
                                LogEx(ex, ref dateIntecept);
                            });
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                LogEx(ex, ref dateIntecept);
            }

            dateIntecept += ($"【执行完成结果】：{invocation.ReturnValue}");

            Parallel.For(0, 1, e =>
            {
                LogLock.OutSql2Log("AopLog", new string[] { dateIntecept});
            });
            _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()).Wait();
        }

        private async Task TestActionsAsync(IInvocation invocation)
        {
            //Console.WriteLine("Waiting after method execution for " + invocation.MethodInvocationTarget.Name);
            //await Task.Delay(200); // 仅作测试
            //Console.WriteLine("Waited after method execution for " + invocation.MethodInvocationTarget.Name);
        }

        private void LogEx(Exception ex ,ref string dataIntercept)
        {
            if (ex != null)
            {
                //执行的 service 中，收录异常
                MiniProfiler.Current.CustomTiming("Errors:", ex.Message);
                //执行的 service 中，捕获异常
                dataIntercept += ($"方法执行中出现异常：{ex.Message + ex.InnerException}\r\n");
            }
        }

        public static bool IsAsynMethod(MethodInfo method)
        {
            return (method.ReturnType==typeof(Task)||(method.ReturnType.IsGenericType&&method.ReturnType.GetGenericTypeDefinition()==typeof(Task<>)));
        }
    }
}
