using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Uniframework
{
    /// <summary>
    /// 客户端图形及图标服务接口
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// 获取一个位图
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>返回从位图资源字典中找到的资源，如果不存在则触发异常。</returns>
        Bitmap GetBitmap(string name);
        /// <summary>
        /// 获取一个位图
        /// </summary>
        /// <param name="name">位图名称</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>系统将从指定的图标字典中返回找到的位图资源。</returns>
        Bitmap GetBitmap(string name, int width, int height);
        /// <summary>
        /// 获取一个位图
        /// </summary>
        /// <param name="name">位图名称</param>
        /// <param name="size">位图尺寸</param>
        /// <returns>系统将从指定的图标字典中返回找到的位图资源。</returns>
        Bitmap GetBitmap(string name, Size size);
        /// <summary>
        /// 获取一个图标
        /// </summary>
        /// <param name="name">图标名称</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>系统将从指定的图标字典中返回找到的图标资源。</returns>
        Icon GetIcon(string name, int width, int height);
        /// <summary>
        /// 获取一个图标
        /// </summary>
        /// <param name="name">图标名称</param>
        /// <param name="size">尺寸</param>
        /// <returns>系统将从指定的图标字典中返回找到的图标资源。</returns>
        Icon GetIcon(string name, Size size);
    }
}
