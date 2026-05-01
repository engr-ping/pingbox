using System;
using System.IO;
using System.Linq;
using System.Xml;
using PingBox.Models;

namespace PingBox.Services;

/// <summary>
/// 配置服务实现
/// </summary>
public class ConfigService : IConfigService
{
    private readonly string _appDirectory;
    private readonly string _appName;
    private readonly string _configFileName;

    public string ConfigFilePath { get; }

    public ConfigService()
    {
        _appDirectory = AppContext.BaseDirectory;
        _appName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "PingBox");
        _configFileName = $"{_appName}.xml";
        ConfigFilePath = Path.Combine(_appDirectory, _configFileName);
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    public AppConfig Load()
    {
        if (!File.Exists(ConfigFilePath))
        {
            return new AppConfig();
        }

        try
        {
            var doc = new XmlDocument();
            doc.Load(ConfigFilePath);
            
            var config = new AppConfig();
            
            // 读取设置
            var settingNode = doc.SelectSingleNode("Config/Setting");
            if (settingNode != null)
            {
                config.Title = GetNodeValue(settingNode, "Title", "PingBox");
                config.Width = GetNodeIntValue(settingNode, "Width", 800);
                config.Height = GetNodeIntValue(settingNode, "Height", 600);
                config.LocationX = GetNodeIntValue(settingNode, "LocationX", -1);
                config.LocationY = GetNodeIntValue(settingNode, "LocationY", -1);
                config.ShowInTaskbar = GetNodeBoolValue(settingNode, "StatusBar", true);
                config.HideOnStart = GetNodeBoolValue(settingNode, "HideStart", false);
                config.HideOnRun = GetNodeBoolValue(settingNode, "HideRun", false);
                config.TopMost = GetNodeBoolValue(settingNode, "TopMost", false);
                config.NoExit = GetNodeBoolValue(settingNode, "NotExit", false);
                config.NoReadLnk = GetNodeBoolValue(settingNode, "NoReadLnk", false);
                config.DoubleClickToRun = GetNodeBoolValue(settingNode, "DoubleClickRun", true);
                config.LineSpacing = GetNodeIntValue(settingNode, "LineSpacing", 75);
                config.ColumnSpacing = GetNodeIntValue(settingNode, "ColumnSpacing", 75);
                config.TabButtonWidth = GetNodeIntValue(settingNode, "LabelWidth", 55);
                config.TabButtonHeight = GetNodeIntValue(settingNode, "LabelHeight", 23);
                config.TabButtonDock = GetNodeIntValue(settingNode, "LabelLocation", 1);
                config.TabButtonBackColor1 = GetNodeValue(settingNode, "LabelBackColor1", "#FFFFFF");
                config.TabButtonBackColor2 = GetNodeValue(settingNode, "LabelBackColor2", "#ADD8E6");
                config.TabButtonForeColor1 = GetNodeValue(settingNode, "LabelForeColor1", "#000000");
                config.TabButtonForeColor2 = GetNodeValue(settingNode, "LabelForeColor2", "#000000");
                config.SelectedTabIndex = GetNodeIntValue(settingNode, "PageIndex", 0);
                config.HotkeyEnabled = GetNodeBoolValue(settingNode, "HotKeyOn", false);
                config.HotkeyModifier = GetNodeValue(settingNode, "HotKey1", "Ctr");
                config.HotkeyKey = GetNodeValue(settingNode, "HotKey2", "F2 键");
            }
            
            // 读取页面
            var pagesNode = doc.SelectSingleNode("Config/Pages");
            if (pagesNode != null)
            {
                foreach (XmlNode pageNode in pagesNode.SelectNodes("Page")!)
                {
                    var page = new PageInfo
                    {
                        Name = GetNodeValue(pageNode, "Name", "新页面"),
                        BackgroundImagePath = GetNodeValue(pageNode, "BackImage", ""),
                        ForeColor = GetNodeValue(pageNode, "ListForeColor", "#000000"),
                        BackColor = GetNodeValue(pageNode, "ListBackColor", "#FFFFFF")
                    };
                    
                    // 读取背景图片属性
                    var bkNode = pageNode.SelectSingleNode("BackImage");
                    if (bkNode != null)
                    {
                        page.BackgroundImageTiled = GetAttributeBoolValue(bkNode, "Tiled", false);
                    }
                    
                    // 读取页面项
                    foreach (XmlNode itemNode in pageNode.SelectNodes("Data")!)
                    {
                        var item = new PageItem
                        {
                            Name = GetNodeValue(itemNode, "Name", ""),
                            FullPath = GetNodeValue(itemNode, "FullPath", ""),
                            Arguments = GetNodeValue(itemNode, "Args", ""),
                            RunAsAdmin = GetNodeBoolValue(itemNode, "RunAs", false)
                        };
                        page.Items.Add(item);
                    }
                    
                    config.Pages.Add(page);
                }
            }
            
            return config;
        }
        catch
        {
            // 配置加载失败，尝试从备份恢复
            if (RestoreFromBackup())
            {
                return Load();
            }
            return new AppConfig();
        }
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    public void Save(AppConfig config)
    {
        // 备份当前配置
        if (File.Exists(ConfigFilePath))
        {
            try
            {
                BackupConfig();
            }
            catch
            {
                // 备份失败不影响保存
            }
        }

        var tempFile = ConfigFilePath + ".tmp";
        try
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(declaration);
            
            var cfgNode = doc.CreateElement("Config");
            doc.AppendChild(cfgNode);
            
            var settingNode = doc.CreateElement("Setting");
            cfgNode.AppendChild(settingNode);
            
            // 保存设置
            SetNodeValue(doc, settingNode, "Title", config.Title);
            SetNodeValue(doc, settingNode, "Width", config.Width.ToString());
            SetNodeValue(doc, settingNode, "Height", config.Height.ToString());
            SetNodeValue(doc, settingNode, "LocationX", config.LocationX.ToString());
            SetNodeValue(doc, settingNode, "LocationY", config.LocationY.ToString());
            SetNodeValue(doc, settingNode, "StatusBar", config.ShowInTaskbar.ToString());
            SetNodeValue(doc, settingNode, "HideStart", config.HideOnStart.ToString());
            SetNodeValue(doc, settingNode, "HideRun", config.HideOnRun.ToString());
            SetNodeValue(doc, settingNode, "TopMost", config.TopMost.ToString());
            SetNodeValue(doc, settingNode, "NotExit", config.NoExit.ToString());
            SetNodeValue(doc, settingNode, "NoReadLnk", config.NoReadLnk.ToString());
            SetNodeValue(doc, settingNode, "DoubleClickRun", config.DoubleClickToRun.ToString());
            SetNodeValue(doc, settingNode, "LineSpacing", config.LineSpacing.ToString());
            SetNodeValue(doc, settingNode, "ColumnSpacing", config.ColumnSpacing.ToString());
            SetNodeValue(doc, settingNode, "LabelWidth", config.TabButtonWidth.ToString());
            SetNodeValue(doc, settingNode, "LabelHeight", config.TabButtonHeight.ToString());
            SetNodeValue(doc, settingNode, "LabelLocation", config.TabButtonDock.ToString());
            SetNodeValue(doc, settingNode, "LabelBackColor1", config.TabButtonBackColor1);
            SetNodeValue(doc, settingNode, "LabelBackColor2", config.TabButtonBackColor2);
            SetNodeValue(doc, settingNode, "LabelForeColor1", config.TabButtonForeColor1);
            SetNodeValue(doc, settingNode, "LabelForeColor2", config.TabButtonForeColor2);
            SetNodeValue(doc, settingNode, "PageIndex", config.SelectedTabIndex.ToString());
            SetNodeValue(doc, settingNode, "HotKeyOn", config.HotkeyEnabled.ToString());
            SetNodeValue(doc, settingNode, "HotKey1", config.HotkeyModifier);
            SetNodeValue(doc, settingNode, "HotKey2", config.HotkeyKey);
            
            // 保存页面
            var pagesNode = doc.CreateElement("Pages");
            cfgNode.AppendChild(pagesNode);
            
            foreach (var page in config.Pages)
            {
                var pageNode = doc.CreateElement("Page");
                pagesNode.AppendChild(pageNode);
                
                SetNodeValue(doc, pageNode, "Name", page.Name);
                
                var bkNode = doc.CreateElement("BackImage");
                bkNode.SetAttribute("On", (!string.IsNullOrEmpty(page.BackgroundImagePath)).ToString());
                bkNode.SetAttribute("Tiled", page.BackgroundImageTiled.ToString());
                bkNode.InnerText = page.BackgroundImagePath ?? string.Empty;
                pageNode.AppendChild(bkNode);
                
                SetNodeValue(doc, pageNode, "ListForeColor", page.ForeColor);
                SetNodeValue(doc, pageNode, "ListBackColor", page.BackColor);
                
                foreach (var item in page.Items)
                {
                    var dataNode = doc.CreateElement("Data");
                    pageNode.AppendChild(dataNode);
                    
                    SetNodeValue(doc, dataNode, "Name", item.Name);
                    
                    // 处理路径（尝试使用相对路径）
                    string path = item.FullPath;
                    if (!string.IsNullOrEmpty(path) && Path.IsPathRooted(path))
                    {
                        try
                        {
                            var relativePath = GetRelativePath(_appDirectory, path);
                            if (!string.IsNullOrEmpty(relativePath) && relativePath != path)
                            {
                                path = relativePath;
                            }
                        }
                        catch
                        {
                            // 保持原路径
                        }
                    }
                    SetNodeValue(doc, dataNode, "FullPath", path);
                    SetNodeValue(doc, dataNode, "Args", item.Arguments);
                    SetNodeValue(doc, dataNode, "RunAs", item.RunAsAdmin.ToString());
                }
            }
            
            // 先保存到临时文件
            doc.Save(tempFile);
            
            // 成功后替换原文件
            if (File.Exists(tempFile))
            {
                File.Copy(tempFile, ConfigFilePath, true);
                File.Delete(tempFile);
            }
        }
        catch
        {
            // 保存失败时清理临时文件
            if (File.Exists(tempFile))
            {
                try { File.Delete(tempFile); } catch { }
            }
        }
    }

    /// <summary>
    /// 从备份恢复配置
    /// </summary>
    public bool RestoreFromBackup()
    {
        try
        {
            var backupDir = Path.Combine(_appDirectory, "Backups");
            if (!Directory.Exists(backupDir))
                return false;
            
            var backupFiles = Directory.GetFiles(backupDir, $"{_appName}_backup_*.xml")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .FirstOrDefault();
            
            if (backupFiles != null && backupFiles.Exists && backupFiles.Length > 100)
            {
                File.Copy(backupFiles.FullName, ConfigFilePath, true);
                return true;
            }
        }
        catch
        {
            // 忽略错误
        }
        return false;
    }

    #region Helper Methods
    
    private void BackupConfig()
    {
        var backupDir = Path.Combine(_appDirectory, "Backups");
        if (!Directory.Exists(backupDir))
        {
            Directory.CreateDirectory(backupDir);
        }
        
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupFile = Path.Combine(backupDir, $"{_appName}_backup_{timestamp}.xml");
        File.Copy(ConfigFilePath, backupFile, true);
        
        // 清理旧备份，只保留最近5个
        var oldBackups = Directory.GetFiles(backupDir, $"{_appName}_backup_*.xml")
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.CreationTime)
            .Skip(5)
            .ToList();
        
        foreach (var oldBackup in oldBackups)
        {
            try { oldBackup.Delete(); } catch { }
        }
    }
    
    private string GetNodeValue(XmlNode parent, string nodeName, string defaultValue)
    {
        var node = parent.SelectSingleNode(nodeName);
        return node?.InnerText ?? defaultValue;
    }
    
    private int GetNodeIntValue(XmlNode parent, string nodeName, int defaultValue)
    {
        var node = parent.SelectSingleNode(nodeName);
        return int.TryParse(node?.InnerText, out var result) ? result : defaultValue;
    }
    
    private bool GetNodeBoolValue(XmlNode parent, string nodeName, bool defaultValue)
    {
        var node = parent.SelectSingleNode(nodeName);
        return bool.TryParse(node?.InnerText, out var result) ? result : defaultValue;
    }
    
    private bool GetAttributeBoolValue(XmlNode node, string attributeName, bool defaultValue)
    {
        var attr = node.Attributes?[attributeName];
        return attr != null && bool.TryParse(attr.Value, out var result) ? result : defaultValue;
    }
    
    private void SetNodeValue(XmlDocument doc, XmlNode parent, string nodeName, string value)
    {
        var node = doc.CreateElement(nodeName);
        node.InnerText = value;
        parent.AppendChild(node);
    }
    
    private string GetRelativePath(string basePath, string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(basePath))
            return fullPath;

        if (!Path.IsPathRooted(fullPath))
            return fullPath;

        if (!basePath.EndsWith(Path.DirectorySeparatorChar))
        {
            basePath += Path.DirectorySeparatorChar;
        }

        var fullRoot = Path.GetPathRoot(fullPath);
        var baseRoot = Path.GetPathRoot(basePath);
        
        if (!string.Equals(fullRoot, baseRoot, StringComparison.OrdinalIgnoreCase))
            return fullPath;

        if (fullPath.StartsWith(@"\\"))
            return fullPath;

        var fullUri = new Uri(fullPath);
        var baseUri = new Uri(basePath);

        var relativePath = Uri.UnescapeDataString(
            baseUri.MakeRelativeUri(fullUri).ToString()
                .Replace('/', Path.DirectorySeparatorChar)
        );

        if (string.IsNullOrEmpty(relativePath))
            return "." + Path.DirectorySeparatorChar;

        return relativePath;
    }
    
    #endregion
}