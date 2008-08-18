﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 框架权限管理构建策略，在这里对系统中定义的所有需要应用权限的类进行处理
    /// </summary>
    public class AuthorizationStrategy : BuilderStrategy
    {
 
        #region IBuilderStrategy Members

        public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
        {
            WorkItem workItem = GetWorkItem(context, existing);
            if (workItem != null)
            {
                IAuthorizationService authService = workItem.Services.Get<IAuthorizationService>();
                if (authService != null)
                {
                    Type type = existing.GetType();
                    foreach (MethodInfo method in type.GetMethods())
                    {
                        RegisterActionImplementation(context, existing, idToBuild, authService, method);
                    }
                }
            }
            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        public override object TearDown(IBuilderContext context, object item)
        {
            WorkItem workItem = GetWorkItem(context, item);
            if (workItem != null)
            {
                IAuthorizationService catalog = workItem.Services.Get<IAuthorizationService>();
                if (catalog != null)
                {
                    Type type = item.GetType();
                    foreach (MethodInfo method in type.GetMethods())
                    {
                        RemoveActionImplementation(catalog, method);
                    }
                }
            }
            return base.TearDown(context, item);
        }

        #endregion

        #region Assistant functions

        private void RegisterActionImplementation(IBuilderContext context, object existing, string idToBuild, IAuthorizationService authService, MethodInfo method)
        {
            foreach (CommandHandlerAttribute attr in method.GetCustomAttributes(typeof(CommandHandlerAttribute), true))
            {
                string path = GetAuthorizeResourcePath(method);
                if (path != null) // 命令所在的类必须定义资源标签才会注册到系统中
                {
                    //if (logger != null)
                    //    logger.Debug("向系统权限管理服务注册命令：" + attr.CommandName + ", 路径：" + path);
                    //authService.RegisterCommand(attr.CommandName, path);
                }
            }
        }

        private void RemoveActionImplementation(IAuthorizationService authService, MethodInfo method)
        {
            foreach (CommandHandlerAttribute attr in method.GetCustomAttributes(typeof(CommandHandlerAttribute), true))
            {
                //authService.UnRegisterCommand(attr.CommandName, GetAuthorizeResourcePath(method));
            }
        }

        private string GetAuthorizeResourcePath(MethodInfo method)
        {
            AuthorizationResourceAttribute[] attrs = (AuthorizationResourceAttribute[])method.DeclaringType.GetCustomAttributes(typeof(AuthorizationResourceAttribute), true);
            if (attrs.Length > 0)
                return attrs[0].Path;
            else
                return String.Empty;
        }


        /// <summary>
        /// 返回当前上下文中的WorkItem
        /// </summary>
        /// <param name="context">构建上下文</param>
        /// <param name="item">对象</param>
        /// <returns>返回当前所需要的WorkItem</returns>
        private WorkItem GetWorkItem(IBuilderContext context, object item)
        {
            if (item is WorkItem)
                return item as WorkItem;

            return context.Locator.Get<WorkItem>(new DependencyResolutionLocatorKey(typeof(WorkItem), null));
        }

        #endregion
    }
}
