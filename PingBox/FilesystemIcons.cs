using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PingBox
{
    public static class FilesystemIcons
    {
        private const int SHGFI_ICON = 0x100;
        private const int SHGFI_SMALLICON = 0x1;
        private const int SHGFI_LARGEICON = 0x0;
        private const int SHGFI_USEFILEATTRIBUTES = 0x10;
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const int FILE_ATTRIBUTE_NORMAL = 0x80;

        #region PublicIcons
        public static Icon ICON_FILE_16x = ExtractIconFromFileX16(@"C:\Windows\system32\shell32.dll", 0);
        public static Icon ICON_FILE_32x = ExtractIconFromFileX32(@"C:\Windows\system32\shell32.dll", 0);
        public static Icon ICON_FILE_48x = ExtractIconFromFileX48(@"C:\Windows\system32\shell32.dll", 0);
        #endregion

        public static Icon SmallIcon(string pfad)
        {
            if (string.IsNullOrEmpty(pfad))
            {
                return ICON_FILE_16x;
            }
            try
            {
                return GetSmallIcon(pfad) ?? ICON_FILE_16x;
            }
            catch (Exception)
            {
                return ICON_FILE_16x;
            }
        }

        public static Icon MediumIcon(string pfad)
        {
            if (string.IsNullOrEmpty(pfad))
            {
                return ICON_FILE_32x;
            }
            try
            {
                return GetLargeIcon(pfad) ?? ICON_FILE_32x;
            }
            catch (Exception)
            {
                return ICON_FILE_32x;
            }
        }

        public static Icon LargeIcon(string pfad)
        {
            if (string.IsNullOrEmpty(pfad))
            {
                return ICON_FILE_48x;
            }
            try
            {
                return GetLargeIcon(pfad) ?? ICON_FILE_48x;
            }
            catch (Exception)
            {
                return ICON_FILE_48x;
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, int uFlags);
        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        #region PrivateMethods
        private static Icon ExtractIconFromFileX16(string file, int iconindex)
        {
            try
            {
                ExtractIconEx(file, iconindex, out IntPtr large, out IntPtr small, 1);
                if (large != IntPtr.Zero)
                {
                    DestroyIcon(large); // 释放不需要的大图标
                }
                if (small != IntPtr.Zero)
                {
                    return Icon.FromHandle(small).Clone() as Icon;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Icon ExtractIconFromFileX32(string file, int iconindex)
        {
            try
            {
                ExtractIconEx(file, iconindex, out IntPtr large, out IntPtr small, 1);
                if (small != IntPtr.Zero)
                {
                    DestroyIcon(small); // 释放不需要的小图标
                }
                if (large != IntPtr.Zero)
                {
                    return Icon.FromHandle(large).Clone() as Icon;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Icon ExtractIconFromFileX48(string file, int iconindex)
        {
            try
            {
                ExtractIconEx(file, iconindex, out IntPtr large, out IntPtr small, 1);
                if (small != IntPtr.Zero)
                {
                    DestroyIcon(small); // 释放不需要的小图标
                }
                if (large != IntPtr.Zero)
                {
                    return Icon.FromHandle(large).Clone() as Icon;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region PublicMethods
        public static Icon GetSmallIcon(string pfad)
        {
            SHFILEINFO shinfo = new SHFILEINFO
            {
                szDisplayName = new string((char)0, 260),
                szTypeName = new string((char)0, 80)
            };
            
            int flags = SHGFI_ICON | SHGFI_SMALLICON;
            int fileAttributes = 0;
            
            // 检查是否是文件夹
            if (System.IO.Directory.Exists(pfad))
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
                fileAttributes = FILE_ATTRIBUTE_DIRECTORY;
            }
            else if (System.IO.File.Exists(pfad))
            {
                fileAttributes = 0;
            }
            else
            {
                // 如果路径不存在,尝试使用 USEFILEATTRIBUTES 标志
                flags |= SHGFI_USEFILEATTRIBUTES;
                fileAttributes = FILE_ATTRIBUTE_NORMAL;
            }
            
            IntPtr result = SHGetFileInfo(pfad, fileAttributes, ref shinfo, Marshal.SizeOf(shinfo), flags);
            
            if (result == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
            {
                return null;
            }
            
            try
            {
                // 克隆图标并释放原始句柄
                Icon icon = Icon.FromHandle(shinfo.hIcon).Clone() as Icon;
                DestroyIcon(shinfo.hIcon);
                return icon;
            }
            catch
            {
                if (shinfo.hIcon != IntPtr.Zero)
                {
                    DestroyIcon(shinfo.hIcon);
                }
                return null;
            }
        }
        
        public static Icon GetLargeIcon(string pfad)
        {
            SHFILEINFO shinfo = new SHFILEINFO
            {
                szDisplayName = new string((char)0, 260),
                szTypeName = new string((char)0, 80)
            };
            
            int flags = SHGFI_ICON | SHGFI_LARGEICON;
            int fileAttributes = 0;
            
            // 检查是否是文件夹
            if (System.IO.Directory.Exists(pfad))
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
                fileAttributes = FILE_ATTRIBUTE_DIRECTORY;
            }
            else if (System.IO.File.Exists(pfad))
            {
                fileAttributes = 0;
            }
            else
            {
                // 如果路径不存在,尝试使用 USEFILEATTRIBUTES 标志
                flags |= SHGFI_USEFILEATTRIBUTES;
                fileAttributes = FILE_ATTRIBUTE_NORMAL;
            }
            
            IntPtr result = SHGetFileInfo(pfad, fileAttributes, ref shinfo, Marshal.SizeOf(shinfo), flags);
            
            if (result == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
            {
                return null;
            }
            
            try
            {
                // 克隆图标并释放原始句柄
                Icon icon = Icon.FromHandle(shinfo.hIcon).Clone() as Icon;
                DestroyIcon(shinfo.hIcon);
                return icon;
            }
            catch
            {
                if (shinfo.hIcon != IntPtr.Zero)
                {
                    DestroyIcon(shinfo.hIcon);
                }
                return null;
            }
        }
        #endregion
    }
}
