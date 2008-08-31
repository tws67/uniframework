using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 客户端图形及图标服务
    /// </summary>
    public class ImageService : IImageService
    {
        private readonly static string ICON_EXTENSION = ".ico";
        private readonly static string IMAGE_EXTENSION = ".png";

        private object SyncObj = new object(); // locker
        private string imagePath = String.Empty;
        private Dictionary<Size, Dictionary<string, Icon>> iconsCache = new Dictionary<Size, Dictionary<string, Icon>>();
        private Dictionary<string, Bitmap> imagesCache = new Dictionary<string, Bitmap>();
        private Dictionary<string, string> imagesMap = new Dictionary<string, string>();

        #region IImageService Members

        /// <summary>
        /// 获取一个位图
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>返回从位图资源字典中找到的资源，如果不存在则触发异常。</returns>
        public Bitmap GetBitmap(string name)
        {
            lock (SyncObj) {
                Bitmap bitmap = null;
                string key = Parse(name, null);
                // 首先从缓存中查找资源
                if (imagesCache.ContainsKey(key))
                    return imagesCache[key];

                // 如果不存在则根据给出的名字从外部加载并缓存起来
                string filename = Path.GetFileNameWithoutExtension(key) + IMAGE_EXTENSION;
                if (File.Exists(filename))
                {
                    bitmap = new Bitmap(filename);
                }
                else
                {
                    List<string> files = FileUtility.SearchDirectory(ImagePath, filename, true);
                    if (files.Count > 0)
                    {
                        bitmap = new Bitmap(files[0]);
                    }
                }

                imagesCache[key] = bitmap;
                return bitmap;
            }
        }

        /// <summary>
        /// 获取一个位图
        /// </summary>
        /// <param name="name">位图名称</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>系统将从指定的图标字典中返回找到的位图资源，如果没有找到将从位图字典中返回。</returns>
        public Bitmap GetBitmap(string name, int width, int height)
        {
            return GetBitmap(name, new Size(width, height));
        }

        /// <summary>
        /// 获取一个位图
        /// </summary>
        /// <param name="name">位图名称</param>
        /// <param name="size">位图尺寸</param>
        /// <returns>系统将从指定的图标字典中返回找到的位图资源，如果没有找到将从位图字典中返回。</returns>
        public Bitmap GetBitmap(string name, Size size)
        {
            Icon icon = GetIcon(name, size);
            if (icon != null)
                return icon.ToBitmap();
            else
                throw new UniframeworkException("不存在名称为 \"" + name + "\" 的图标资源。");
        }

        /// <summary>
        /// 获取一个图标
        /// </summary>
        /// <param name="name">图标名称</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>系统将从指定的图标字典中返回找到的图标资源。</returns>
        public Icon GetIcon(string name, int width, int height)
        {
            Size size = new Size(width, height);
            return GetIcon(name, size);
        }

        /// <summary>
        /// 获取一个图标
        /// </summary>
        /// <param name="name">图标名称</param>
        /// <param name="size">尺寸</param>
        /// <returns>系统将从指定的图标字典中返回找到的图标资源。</returns>
        public Icon GetIcon(string name,Size size)
        {
            lock (SyncObj) {
                Icon icon = null;
                string key = Parse(name, null);

                // 首先从图标缓存中查找图标
                if (iconsCache.ContainsKey(size) && iconsCache[size].ContainsKey(key)) {
                    icon = iconsCache[size][key];
                    return icon;
                }

                string filename = Path.GetFileNameWithoutExtension(key) + ICON_EXTENSION;
                imagesMap[key] = filename;
                if (File.Exists(filename))
                {
                    icon = new Icon(filename, size);
                }
                else {
                    List<string> files = FileUtility.SearchDirectory(ImagePath, filename, true);
                    if (files.Count > 0) {
                        icon = new Icon(files[0], size);
                    }
                }

                if (!iconsCache.ContainsKey(size)) { // 保存图标到缓存中
                    iconsCache.Add(size, new Dictionary<string,Icon>());
                    iconsCache[size][key] = icon;
                }
                return icon;
            }
        }

        #endregion

        #region assistant functions

        private string ImagePath
        {
            get {
                if (String.IsNullOrEmpty(imagePath)) {
                    imagePath = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Resources\";
                    if (!Directory.Exists(imagePath))
                        Directory.CreateDirectory(imagePath);
                }
                return imagePath;
            }
        }

        /// <summary>
        /// 分析字符串将其中以${...}括起来的字符串资源替换为实际的值。
        /// </summary>
        /// <param name="input"></param>
        /// <param name="customTags"></param>
        /// <returns></returns>
        public string Parse(string input, string[,] customTags)
        {
            if (input == null)
                return null;
            int pos = 0;
            StringBuilder output = null; // don't use StringBuilder if input is a single property
            do
            {
                int oldPos = pos;
                pos = input.IndexOf("${", pos);
                if (pos < 0)
                {
                    if (output == null)
                    {
                        return input;
                    }
                    else
                    {
                        if (oldPos < input.Length)
                        {
                            // normal text after last property
                            output.Append(input, oldPos, input.Length - oldPos);
                        }
                        return output.ToString();
                    }
                }
                if (output == null)
                {
                    if (pos == 0)
                        output = new StringBuilder();
                    else
                        output = new StringBuilder(input, 0, pos, pos + 16);
                }
                else
                {
                    if (pos > oldPos)
                    {
                        // normal text between two properties
                        output.Append(input, oldPos, pos - oldPos);
                    }
                }
                int end = input.IndexOf('}', pos + 1);
                if (end < 0)
                {
                    output.Append("${");
                    pos += 2;
                }
                else
                {
                    string property = input.Substring(pos + 2, end - pos - 2);
                    string val = GetValue(property, customTags);
                    if (val == null)
                    {
                        output.Append("${");
                        output.Append(property);
                        output.Append('}');
                    }
                    else
                    {
                        output.Append(val);
                    }
                    pos = end + 1;
                }
            } while (pos < input.Length);
            return output.ToString();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="customTags">The custom tags.</param>
        /// <returns></returns>
        private string GetValue(string propertyName, string[,] customTags)
        {
            return propertyName;
        }

        #endregion
    }
}
