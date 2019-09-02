using JYShop.Common.LogHelper;
using JYShop.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JYShopAPI.Middlewares
{
    public class RequRespLogMildd
    {
        private readonly RequestDelegate _next;
        private readonly IShopInfoService _shopService;

        public RequRespLogMildd(RequestDelegate next, IShopInfoService shopService)
        {
            _next = next;
            _shopService = shopService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 过滤，只有接口
            if (context.Request.Path.Value.Contains("api"))
            {
                context.Request.EnableBuffering();
                Stream originalBody = context.Response.Body;
                try
                {
                    // 存储请求数据
                    RequestDataLog(context.Request);
                    using (var ms = new MemoryStream())
                    {
                        context.Response.Body = ms;
                        await _next(context);

                        ResponseDataLog(context.Response, ms);
                        ms.Position = 0;

                        await ms.CopyToAsync(originalBody);

                    }
                }
                catch (Exception ex)
                {
                    // 记录异常
                    //ErrorLogData(context.Response, ex);
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private void RequestDataLog(HttpRequest request)
        {
            var sr = new StreamReader(request.Body);

            var content = $" QueryData:{request.Path + request.QueryString}\r\n BodyData:{sr.ReadToEnd()}";

            if (!string.IsNullOrEmpty(content))
            {
                Parallel.For(0, 1, e =>
                {
                    LogLock.OutSql2Log("RequestResponseLog", new string[] { "Request Data:", content });

                });

                request.Body.Position = 0;
            }

        }


        private void ResponseDataLog(HttpResponse response, MemoryStream ms)
        {
            ms.Position = 0;
            var ResponseBody = new StreamReader(ms).ReadToEnd();

            // 去除 Html
            var reg = "<[^>]+>";
            var isHtml = Regex.IsMatch(ResponseBody, reg);

            if (!string.IsNullOrEmpty(ResponseBody))
            {
                Parallel.For(0, 1, e =>
                {
                    LogLock.OutSql2Log("RequestResponseLog", new string[] { "Response Data:", ResponseBody });

                });
            }
        }
    }
}
