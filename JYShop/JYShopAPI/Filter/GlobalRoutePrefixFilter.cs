using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JYShopAPI.Filter
{
    public class GlobalRoutePrefixFilter : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _attributeRouteModel;

        public GlobalRoutePrefixFilter(IRouteTemplateProvider routeTemplateProvider)
        {
            _attributeRouteModel = new AttributeRouteModel(routeTemplateProvider);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var c in application.Controllers)
            { 
                // 已经标记了 RouteAttribute 的 Controller
                var matchSelectors = c.Selectors.Where(x => x.AttributeRouteModel != null).ToList();

                if (matchSelectors.Any())
                {
                    foreach (var ms in matchSelectors)
                    {
                        // 在 当前路由上 再 添加一个 路由前缀
                        ms.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_attributeRouteModel, ms.AttributeRouteModel);
                    }
                }

                // 没有标记 RouteAttribute 的 Controller
                var unmatchedSelectors = c.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        // 添加一个 路由前缀
                        selectorModel.AttributeRouteModel = _attributeRouteModel;
                    }
                }
            }
        }
    }
}
