using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

namespace Uniframework
{
    /// <summary>
    /// 
    /// </summary>
    public enum FileErrorPolicy
    {
        /// <summary>
        /// 
        /// </summary>
        Inform,
        /// <summary>
        /// 
        /// </summary>
        ProvideAlternative
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FileOperationResult
    {
        /// <summary>
        /// 
        /// </summary>
        OK,
        /// <summary>
        /// 
        /// </summary>
        Failed,
        /// <summary>
        /// 
        /// </summary>
        SavedAlternatively
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void FileOperationDelegate();

    /// <summary>
    /// 
    /// </summary>
    public delegate void NamedFileOperationDelegate(string fileName);

    /// <summary>
    /// 文件操作工具类
    /// </summary>
    public static class FileUtility
    {
        readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };
        static string applicationRootPath = AppDomain.CurrentDomain.BaseDirectory;
        const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";

        public static string ApplicationRootPath
        {
            get
            {
                return applicationRootPath;
            }
            set
            {
                applicationRootPath = value;
            }
        }

        public static string NETFrameworkInstallRoot
        {
            get
            {
                using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework"))
                {
                    object o = installRootKey.GetValue("InstallRoot");
                    return o == null ? String.Empty : o.ToString();
                }
            }
        }

        public static string NetSdkInstallRoot
        {
            get
            {
                using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework"))
                {
                    object o = installRootKey.GetValue("sdkInstallRootv2.0");
                    return o == null ? String.Empty : o.ToString();
                }
            }
        }

        public static string[] GetAvailableRuntimeVersions()
        {
            string installRoot = NETFrameworkInstallRoot;
            string[] files = Directory.GetDirectories(installRoot);

            ArrayList runtimes = new ArrayList();
            foreach (string file in files)
            {
                string runtime = Path.GetFileName(file);
                if (runtime.StartsWith("v"))
                {
                    runtimes.Add(runtime);
                }
            }
            return (string[])runtimes.ToArray(typeof(string));
        }

        public static string Combine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return String.Empty;
            }

            string result = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                result = Path.Combine(result, paths[i]);
            }
            return result;
        }

        public static bool IsUrl(string path)
        {
            return path.IndexOf(':') >= 2;
        }

        /// <summary>
        /// Convert a path into a fully qualified local file path.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <returns>The fully qualified path.</returns>
        /// <remarks>
        /// <para>
        /// Converts the path specified to a fully
        /// qualified path. If the path is relative it is
        /// taken as relative from the application base 
        /// directory.
        /// </para>
        /// <para>
        /// The path specified must be a local file path, a URI is not supported.
        /// </para>
        /// </remarks>
        public static string ConvertToFullPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            string baseDirectory = "";
            try
            {
                string applicationBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (applicationBaseDirectory != null)
                {
                    // applicationBaseDirectory may be a URI not a local file path
                    Uri applicationBaseDirectoryUri = new Uri(applicationBaseDirectory);
                    if (applicationBaseDirectoryUri.IsFile)
                    {
                        baseDirectory = applicationBaseDirectoryUri.LocalPath;
                    }
                }
            }
            catch
            {
                // Ignore URI exceptions & SecurityExceptions from SystemInfo.ApplicationBaseDirectory
            }

            if (baseDirectory != null && baseDirectory.Length > 0)
            {
                // Note that Path.Combine will return the second path if it is rooted
                return Path.GetFullPath(Path.Combine(baseDirectory, path));
            }
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Converts a given absolute path and a given base path to a path that leads
        /// from the base path to the absoulte path. (as a relative path)
        /// </summary>
        public static string GetRelativePath(string baseDirectoryPath, string absPath)
        {
            if (IsUrl(absPath) || IsUrl(baseDirectoryPath))
            {
                return absPath;
            }
            try
            {
                baseDirectoryPath = Path.GetFullPath(baseDirectoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                absPath = Path.GetFullPath(absPath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("GetRelativePath error '" + baseDirectoryPath + "' -> '" + absPath + "'", ex);
            }

            string[] bPath = baseDirectoryPath.Split(separators);
            string[] aPath = absPath.Split(separators);
            int indx = 0;
            for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
            {
                if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
                    break;
            }

            if (indx == 0)
            {
                return absPath;
            }

            StringBuilder erg = new StringBuilder();

            if (indx == bPath.Length)
            {
                //				erg.Append('.');
                //				erg.Append(Path.DirectorySeparatorChar);
            }
            else
            {
                for (int i = indx; i < bPath.Length; ++i)
                {
                    erg.Append("..");
                    erg.Append(Path.DirectorySeparatorChar);
                }
            }
            erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
            return erg.ToString();
        }

        /// <summary>
        /// Converts a given relative path and a given base path to a path that leads
        /// to the relative path absoulte.
        /// </summary>
        public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
        {
            return Path.GetFullPath(Path.Combine(baseDirectoryPath, relPath));
        }

        public static string GetParent(string path)
        {
            if (path.EndsWith(@"\"))
                return Directory.GetParent(Directory.GetParent(path).FullName).FullName;
            else
                return Directory.GetParent(path).FullName;
        }

        public static bool IsEqualFileName(string fileName1, string fileName2)
        {
            // Optimized for performance:
            //return Path.GetFullPath(fileName1.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToLower() == Path.GetFullPath(fileName2.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToLower();

            if (string.IsNullOrEmpty(fileName1) || string.IsNullOrEmpty(fileName2)) return false;

            char lastChar;
            lastChar = fileName1[fileName1.Length - 1];
            if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
                fileName1 = fileName1.Substring(0, fileName1.Length - 1);
            lastChar = fileName2[fileName2.Length - 1];
            if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
                fileName2 = fileName2.Substring(0, fileName2.Length - 1);

            try
            {
                if (fileName1.Length < 2 || fileName1[1] != ':' || fileName1.IndexOf("/.") >= 0 || fileName1.IndexOf("\\.") >= 0)
                    fileName1 = Path.GetFullPath(fileName1);
                if (fileName2.Length < 2 || fileName2[1] != ':' || fileName2.IndexOf("/.") >= 0 || fileName2.IndexOf("\\.") >= 0)
                    fileName2 = Path.GetFullPath(fileName2);
            }
            catch (Exception) { }
            return string.Equals(fileName1, fileName2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
        {
            try
            {
                baseDirectory = Path.GetFullPath(baseDirectory).ToUpperInvariant();
                testDirectory = Path.GetFullPath(testDirectory).ToUpperInvariant();
                baseDirectory = baseDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                testDirectory = testDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                if (baseDirectory[baseDirectory.Length - 1] != Path.DirectorySeparatorChar)
                    baseDirectory += Path.DirectorySeparatorChar;
                if (testDirectory[testDirectory.Length - 1] != Path.DirectorySeparatorChar)
                    testDirectory += Path.DirectorySeparatorChar;

                return testDirectory.StartsWith(baseDirectory);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
        {
            fileName = Path.GetFullPath(fileName);
            oldDirectory = Path.GetFullPath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            newDirectory = Path.GetFullPath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (IsBaseDirectory(oldDirectory, fileName))
            {
                if (fileName.Length == oldDirectory.Length)
                {
                    return newDirectory;
                }
                return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
            }
            return fileName;
        }

        public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            foreach (string fileName in Directory.GetFiles(sourceDirectory))
            {
                File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
            }
            foreach (string directoryName in Directory.GetDirectories(sourceDirectory))
            {
                DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
            }
        }

        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
        {
            List<string> collection = new List<string>();
            SearchDirectory(directory, filemask, collection, searchSubdirectories, ignoreHidden);
            return collection;
        }

        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
        {
            return SearchDirectory(directory, filemask, searchSubdirectories, false);
        }

        public static List<string> SearchDirectory(string directory, string filemask)
        {
            return SearchDirectory(directory, filemask, true, false);
        }

        /// <summary>
        /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path
        /// <paramref name="directory"/> and all subdirectories
        /// (if <paramref name="searchSubdirectories"/> is true).
        /// The found files are added to the List&lt;string&gt;
        /// <paramref name="collection"/>.
        /// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
        /// </summary>
        static void SearchDirectory(string directory, string filemask, List<string> collection, bool searchSubdirectories, bool ignoreHidden)
        {
            string[] file = Directory.GetFiles(directory, filemask);
            foreach (string f in file)
            {
                if (ignoreHidden && (File.GetAttributes(f) & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                collection.Add(f);
            }

            if (searchSubdirectories)
            {
                string[] dir = Directory.GetDirectories(directory);
                foreach (string d in dir)
                {
                    if (ignoreHidden && (File.GetAttributes(d) & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }
                    SearchDirectory(d, filemask, collection, searchSubdirectories, ignoreHidden);
                }
            }
        }

        public static int MaxPathLength = 260;

        /// <summary>
        /// This method checks the file fileName if it is valid.
        /// </summary>
        public static bool IsValidFileName(string fileName)
        {
            // Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
            //        I can't find a .NET property or method for determining this variable.

            if (fileName == null || fileName.Length == 0 || fileName.Length >= MaxPathLength)
            {
                return false;
            }

            // platform independend : check for invalid path chars

            if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }
            if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0)
            {
                return false;
            }

            if (!Regex.IsMatch(fileName, fileNameRegEx))
            {
                return false;
            }

            // platform dependend : Check for invalid file names (DOS)
            // this routine checks for follwing bad file names :
            // CON, PRN, AUX, NUL, COM1-9 and LPT1-9

            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (nameWithoutExtension != null)
            {
                nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
            }

            if (nameWithoutExtension == "CON" ||
                nameWithoutExtension == "PRN" ||
                nameWithoutExtension == "AUX" ||
                nameWithoutExtension == "NUL")
            {
                return false;
            }

            char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';

            return !((nameWithoutExtension.StartsWith("COM") ||
                      nameWithoutExtension.StartsWith("LPT")) &&
                     Char.IsDigit(ch));
        }

        /// <summary>
        /// Checks that a single directory name (not the full path) is valid.
        /// </summary>
        public static bool IsValidDirectoryName(string name)
        {
            if (!IsValidFileName(name))
            {
                return false;
            }
            if (name.IndexOfAny(new char[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar }) >= 0)
            {
                return false;
            }
            if (name.Trim(' ').Length == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsDirectory(string filename)
        {
            if (!Directory.Exists(filename))
            {
                return false;
            }
            FileAttributes attr = File.GetAttributes(filename);
            return (attr & FileAttributes.Directory) != 0;
        }

        //TODO This code is Windows specific
        static bool MatchN(string src, int srcidx, string pattern, int patidx)
        {
            int patlen = pattern.Length;
            int srclen = src.Length;
            char next_char;

            for (; ; )
            {
                if (patidx == patlen)
                    return (srcidx == srclen);
                next_char = pattern[patidx++];
                if (next_char == '?')
                {
                    if (srcidx == src.Length)
                        return false;
                    srcidx++;
                }
                else if (next_char != '*')
                {
                    if ((srcidx == src.Length) || (src[srcidx] != next_char))
                        return false;
                    srcidx++;
                }
                else
                {
                    if (patidx == pattern.Length)
                        return true;
                    while (srcidx < srclen)
                    {
                        if (MatchN(src, srcidx, pattern, patidx))
                            return true;
                        srcidx++;
                    }
                    return false;
                }
            }
        }

        static bool Match(string src, string pattern)
        {
            if (pattern[0] == '*')
            {
                // common case optimization
                int i = pattern.Length;
                int j = src.Length;
                while (--i > 0)
                {
                    if (pattern[i] == '*')
                        return MatchN(src, 0, pattern, 0);
                    if (j-- == 0)
                        return false;
                    if ((pattern[i] != src[j]) && (pattern[i] != '?'))
                        return false;
                }
                return true;
            }
            return MatchN(src, 0, pattern, 0);
        }

        public static bool MatchesPattern(string filename, string pattern)
        {
            filename = filename.ToUpper();
            pattern = pattern.ToUpper();
            string[] patterns = pattern.Split(';');
            foreach (string p in patterns)
            {
                if (Match(filename, p))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
