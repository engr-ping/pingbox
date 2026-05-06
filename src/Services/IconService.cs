using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PingBox.Services;

/// <summary>
/// 图标服务实现（Windows平台）
/// </summary>
public class IconService : IIconService
{
#if WINDOWS
    #region Windows API

    private const int SHGFI_ICON = 0x100;
    private const int SHGFI_SMALLICON = 0x1;
    private const int SHGFI_LARGEICON = 0x0;
    private const int SHGFI_USEFILEATTRIBUTES = 0x10;
    private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
    private const int FILE_ATTRIBUTE_NORMAL = 0x80;

    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, int uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("user32.dll")]
    private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines,
        [Out] byte[]? lpvBits, ref BITMAPINFOHEADER lpbi, uint uUsage);

    [DllImport("gdi32.dll")]
    private static extern int GetObject(IntPtr hgdiobj, int cbBuffer, out GDIBITMAP lpvObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public int dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ICONINFO
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct GDIBITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public short bmPlanes;
        public short bmBitsPixel;
        public IntPtr bmBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    #endregion
#endif

    private Bitmap? _defaultFileIcon;
    private Bitmap? _defaultFolderIcon;

    /// <summary>
    /// 获取文件/文件夹图标
    /// </summary>
    public Bitmap? GetIcon(string path, int size = 48)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        try
        {
            bool isDirectory = Directory.Exists(path);
            bool exists = File.Exists(path) || isDirectory;

#if WINDOWS
            int flags = SHGFI_ICON;
            int fileAttributes = 0;

            if (isDirectory)
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
                fileAttributes = FILE_ATTRIBUTE_DIRECTORY;
            }
            else if (!exists)
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
                fileAttributes = FILE_ATTRIBUTE_NORMAL;
            }

            // 根据大小选择图标类型
            if (size <= 16)
            {
                flags |= SHGFI_SMALLICON;
            }
            else
            {
                flags |= SHGFI_LARGEICON;
            }

            var shfi = new SHFILEINFO();
            IntPtr result = SHGetFileInfo(path, fileAttributes, ref shfi, Marshal.SizeOf(shfi), flags);

            if (result == IntPtr.Zero || shfi.hIcon == IntPtr.Zero)
            {
                return isDirectory ? GetDefaultFolderIcon(size) : GetDefaultFileIcon(size);
            }

            try
            {
                // 将HICON转换为Avalonia Bitmap
                var bitmap = HIconToBitmap(shfi.hIcon);
                DestroyIcon(shfi.hIcon);
                return bitmap;
            }
            catch
            {
                DestroyIcon(shfi.hIcon);
                return isDirectory ? GetDefaultFolderIcon(size) : GetDefaultFileIcon(size);
            }
#else
            return isDirectory ? GetDefaultFolderIcon(size) : GetDefaultFileIcon(size);
#endif
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取默认文件图标
    /// </summary>
    public Bitmap GetDefaultFileIcon(int size = 48)
    {
        if (_defaultFileIcon == null)
        {
            _defaultFileIcon = CreateDefaultFileIcon(size);
        }
        return _defaultFileIcon;
    }

    /// <summary>
    /// 获取默认文件夹图标
    /// </summary>
    public Bitmap GetDefaultFolderIcon(int size = 48)
    {
        if (_defaultFolderIcon == null)
        {
            _defaultFolderIcon = CreateDefaultFolderIcon(size);
        }
        return _defaultFolderIcon;
    }

    /// <summary>
    /// 将HICON转换为Avalonia Bitmap（使用GDI GetDIBits）
    /// </summary>
    private Bitmap HIconToBitmap(IntPtr hIcon)
    {
        if (!GetIconInfo(hIcon, out ICONINFO iconInfo))
            return GetDefaultFileIcon(48);

        try
        {
            if (iconInfo.hbmColor == IntPtr.Zero)
                return GetDefaultFileIcon(48);

            // 获取位图尺寸
            GetObject(iconInfo.hbmColor, Marshal.SizeOf<GDIBITMAP>(), out GDIBITMAP gdiBmp);
            int w = gdiBmp.bmWidth > 0 ? gdiBmp.bmWidth : 32;
            int h = gdiBmp.bmHeight > 0 ? Math.Abs(gdiBmp.bmHeight) : 32;

            var bi = new BITMAPINFOHEADER
            {
                biSize = Marshal.SizeOf<BITMAPINFOHEADER>(),
                biWidth = w,
                biHeight = -h,   // top-down
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0
            };

            var bits = new byte[w * h * 4];
            IntPtr hDC = GetDC(IntPtr.Zero);
            try
            {
                GetDIBits(hDC, iconInfo.hbmColor, 0, (uint)h, bits, ref bi, 0);
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, hDC);
            }

            // Windows gives BGRA; Avalonia Bgra8888 is also BGRA — direct copy works
            var wb = new WriteableBitmap(
                new PixelSize(w, h),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            using var fb = wb.Lock();
            Marshal.Copy(bits, 0, fb.Address, bits.Length);
            return wb;
        }
        catch
        {
            return GetDefaultFileIcon(48);
        }
        finally
        {
            if (iconInfo.hbmColor != IntPtr.Zero) DeleteObject(iconInfo.hbmColor);
            if (iconInfo.hbmMask != IntPtr.Zero) DeleteObject(iconInfo.hbmMask);
        }
    }

    /// <summary>
    /// 创建默认文件图标
    /// </summary>
    private Bitmap CreateDefaultFileIcon(int size)
    {
        // 尝试从资源加载
        try
        {
            var asset = AssetLoader.Open(new Uri("avares://PingBox/Assets/file-icon.png"));
            if (asset != null)
            {
                return new Bitmap(asset);
            }
        }
        catch
        {
            // 忽略错误
        }

        // 如果资源不存在，创建一个简单的占位图标
        return CreatePlaceholderIcon(size, Colors.Gray);
    }

    /// <summary>
    /// 创建默认文件夹图标
    /// </summary>
    private Bitmap CreateDefaultFolderIcon(int size)
    {
        // 尝试从资源加载
        try
        {
            var asset = AssetLoader.Open(new Uri("avares://PingBox/Assets/folder-icon.png"));
            if (asset != null)
            {
                return new Bitmap(asset);
            }
        }
        catch
        {
            // 忽略错误
        }

        // 如果资源不存在，创建一个简单的占位图标
        return CreatePlaceholderIcon(size, Colors.Blue);
    }

    /// <summary>
    /// 创建占位图标
    /// </summary>
    private Bitmap CreatePlaceholderIcon(int size, Color color)
    {
        // 创建一个简单的彩色方块作为占位
        var pixelSize = new PixelSize(size, size);
        var dpi = new Vector(96, 96);
        
        var bitmap = new WriteableBitmap(pixelSize, dpi, PixelFormat.Bgra8888, AlphaFormat.Opaque);
        
        using (var frameBuffer = bitmap.Lock())
        {
            var buffer = new byte[frameBuffer.Size.Height * frameBuffer.RowBytes];
            for (int y = 0; y < frameBuffer.Size.Height; y++)
            {
                for (int x = 0; x < frameBuffer.Size.Width; x++)
                {
                    int offset = y * frameBuffer.RowBytes + x * 4;
                    buffer[offset] = color.B;     // B
                    buffer[offset + 1] = color.G; // G
                    buffer[offset + 2] = color.R; // R
                    buffer[offset + 3] = color.A; // A
                }
            }
            Marshal.Copy(buffer, 0, frameBuffer.Address, buffer.Length);
        }
        
        return bitmap;
    }
}