using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient
{
    public class BuilderUtility
    {
        private static readonly string ResourcePath = @"\Resources\";
        private static readonly string DEFAULT_IMAGE = "${gear}";

        /// <summary>
        /// 获取工具条管理器
        /// </summary>
        /// <param name="workItem">工作项</param>
        /// <returns>返回系统外壳定义的工具条管理器</returns>
        public static BarManager GetBarManager(WorkItem workItem)
        {
            Guard.ArgumentNotNull(workItem, "workItem");

            BarManager barManager = workItem.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
            return barManager;
        }

        /// <summary>
        /// 获取指定名称的Command组件
        /// </summary>
        /// <param name="workItem">工作项</param>
        /// <param name="name">命令名称</param>
        /// <returns>返回指定名称的命令组件，否则为空</returns>
        public static Command GetCommand(WorkItem workItem, string name)
        {
            Command result = workItem.Commands.Get<Command>(name);
            if (result != null)
                return result;
            List<WorkItem> WorkItems = (List<WorkItem>)workItem.WorkItems.FindByType<WorkItem>();
            foreach (WorkItem item in WorkItems)
            {
                result = GetCommand(item, name);
                if (result != null)
                    break;
            }
            return result;
        }

        /// <summary>
        /// 返回系统资源存放路径
        /// </summary>
        /// <returns></returns>
        public static String GetResourcePath()
        {
            return FileUtility.GetParent(FileUtility.ApplicationRootPath) + ResourcePath;
        }

        /// <summary>
        /// 合成路径
        /// </summary>
        /// <param name="parentPath">上级路径</param>
        /// <param name="id">当前标识</param>
        /// <returns>路径</returns>
        public static String CombinPath(string parentPath, string id)
        {
            if (parentPath.EndsWith("/"))
                return parentPath + id;
            else
                return parentPath + "/" + id;
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Bitmap GetBitmap(WorkItem workItem, string filename, int width, int height)
        {
            Size size = new Size(width, height);
            IImageService imageService = workItem.Services.Get<IImageService>();
            if (imageService != null) {
                Bitmap bitmap = imageService.GetBitmap(filename, size);
                if (bitmap == null)
                    bitmap = imageService.GetBitmap(DEFAULT_IMAGE, size);
                return bitmap;
            }
            throw new UniframeworkException("没有加载 ImageService 服务不能获取指定的图像资源。");
        }

        public static Bitmap GetBitmap(WorkItem workItem, string filename)
        {
            IImageService imageService = workItem.Services.Get<IImageService>();
            if (imageService != null) {
                Bitmap bitmap = imageService.GetBitmap(filename);
                if (bitmap == null)
                    bitmap = imageService.GetBitmap(DEFAULT_IMAGE);
                return bitmap;
            }
            throw new UniframeworkException("没有加载 ImageService 服务不能获取指定的图像资源。");
        }
    }
}
