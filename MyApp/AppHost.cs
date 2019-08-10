﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using Funq;
using MyApp.ServiceInterface;
using ServiceStack.Razor;
using ServiceStack;
using ServiceStack.Html;

namespace MyApp
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Base constructor requires a Name and Assembly where web service implementation is located
        /// </summary>
        public AppHost()
            : base("MyApp", typeof(MyServices).Assembly) { }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get("DebugMode", false),
                WebHostPhysicalPath = MapProjectPath("~/wwwroot"),
                UseCamelCase = true,
            });

            Plugins.Add(new RazorFormat());

            CustomErrorHttpHandlers[HttpStatusCode.NotFound] = new RazorHandler("/notfound");
            CustomErrorHttpHandlers[HttpStatusCode.Forbidden] = new RazorHandler("/forbidden");

            if (Config.DebugMode)
            {
                Plugins.Add(new HotReloadFeature());
            }
        }
    }

    //TODO: remove from v5.6.1
    public static class HtmlExtensions
    {
        public static HtmlString Navbar(this HtmlHelper html) => html.NavBar(ViewUtils.NavItems, null);
        
        public static HtmlString BundleJs(this HtmlHelper html, BundleOptions options) => ViewUtils.BundleJs(
            nameof(BundleJs), HostContext.VirtualFileSources, HostContext.VirtualFiles, Minifiers.JavaScript, options).ToHtmlString();

        public static HtmlString BundleCss(this HtmlHelper html, BundleOptions options) => ViewUtils.BundleCss(
            nameof(BundleCss), HostContext.VirtualFileSources, HostContext.VirtualFiles, Minifiers.Css, options).ToHtmlString();
    }
}