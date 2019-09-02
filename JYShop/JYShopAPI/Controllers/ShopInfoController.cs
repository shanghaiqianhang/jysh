using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JYShop.Common;
using JYShop.Common.Helper;
using JYShop.IServices;
using JYShopAPI.SwaggerHelper;
using JYShopModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using static JYShopAPI.SwaggerHelper.CustomApiVersion;

namespace JYShopAPI.Controllers
{

    [Produces("application/json")]
    [Route("api/ShopInfo")]
    [Authorize]
    public class ShopInfoController : ControllerBase
    {
        readonly IShopInfoService _shopInfoServices;
        readonly IRedisCacheManager _redisCacheManager;

        public ShopInfoController(IShopInfoService shopInfoServices, IRedisCacheManager redisCacheManager)
        {
            _shopInfoServices = shopInfoServices;
            _redisCacheManager = redisCacheManager;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="shoptype"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ResponseCache(Duration = 600)]
        public async Task<object> Get(int id, int page = 1, string shoptype = "Online", string key = "")
        {
            int intTotalCount = 6;
            int total;
            int totalCount = 1;
            List<ShopInfo> shopList = new List<ShopInfo>();
            if (string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            using (MiniProfiler.Current.Step("开始加载数据："))
            {
                try
                {
                    if (_redisCacheManager.Get<object>("Redis.ShopInfo") != null)
                    {
                        MiniProfiler.Current.Step("从Redis服务器中加载数据：");

                        shopList = _redisCacheManager.Get<List<ShopInfo>>("Redis.ShopInfo");
                    }
                    else
                    {
                        MiniProfiler.Current.Step("从MSSQL服务器中加载数据：");
                        shopList = await _shopInfoServices.Query(q => true);
                        _redisCacheManager.Set("Redis.ShopInfo", shopList, TimeSpan.FromHours(2));
                    }
                }
                catch (Exception ex)
                {
                    MiniProfiler.Current.CustomTiming("Errors：", "Redis服务未启用，请开启该服务，并且请注意端口号，本项目使用的的6319，而且我的是没有设置密码。" + ex.Message);
                    shopList = await _shopInfoServices.Query(q => true);
                }
            }

            shopList = shopList.Where(q => (q.ShopName != null && q.ShopName.Contains(key))).ToList();
            total = shopList.Count();
            totalCount = shopList.Count() / intTotalCount;
            using (MiniProfiler.Current.Step("获取成功后，开始处理最终数据"))
            {
                shopList = shopList.OrderByDescending(d => d.Id).Skip((page - 1) * intTotalCount).Take(intTotalCount).ToList();

                foreach (var item in shopList)
                {
                    if (!string.IsNullOrEmpty(item.ShopAddress))
                    {
                        item.ShopAddress = (HtmlHelper.ReplaceHtmlTag(item.ShopAddress)).Length >= 200 ? (HtmlHelper.ReplaceHtmlTag(item.ShopAddress)).Substring(0, 200) : (HtmlHelper.ReplaceHtmlTag(item.ShopAddress));
                        int totalLength = 500;
                        if (item.ShopAddress.Length > totalLength)
                        {
                            item.ShopAddress = item.ShopAddress.Substring(0, totalLength);
                        }
                    }
                }

            }

            return Ok(new {
                success = true,
                page,
                total,
                pageCount = totalCount,
                data = shopList
            });


        }



        /// <summary>
        /// 获取博客详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<object> Get(int id)
        {
            var model = await _shopInfoServices.QueryById(id);
            return Ok(new
            {
                success = true,
                data = model
            });
        }


        /// <summary>
        /// 获取详情【无权限】
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DetailNuxtNoPer")]
        [AllowAnonymous]
        public async Task<object> DetailNuxtNoPer(int id)
        {
            var model = await _shopInfoServices.QueryById(id);
            return Ok(new
            {
                success = true,
                data = model
            });
        }

        /// <summary>
        /// 获取shop测试信息 v2版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        ////MVC自带特性 对 api 进行组管理
        //[ApiExplorerSettings(GroupName = "v2")]
        ////路径 如果以 / 开头，表示绝对路径，反之相对 controller 的想u地路径
        //[Route("/api/v2/blog/Blogtest")]
        //和上边的版本控制以及路由地址都是一样的

        [CustomRoute(ApiVersions.V2, "ShopInfotest")]
        [AllowAnonymous]
        public object V2_Blogtest()
        {
            return Ok(new { status = 220, data = "我是第二版的shop信息" });
        }


        /// <summary>
        /// 添加博客【无权限】
        /// </summary>
        /// <param name="blogArticle"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<MessageModel<string>> Post([FromBody] ShopInfo shopInfo)
        {
            var data = new MessageModel<string>();
 

            var id = (await _shopInfoServices.Add(shopInfo));
            data.success = id > 0;
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 删除博客
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Permissions.Name)]
        [Route("Delete")]
        public async Task<MessageModel<string>> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var shopInfo = await _shopInfoServices.QueryById(id);
                data.success = await _shopInfoServices.Update(shopInfo);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = shopInfo?.Id.ObjToString();
                }
            }

            return data;
        }


    }
}