using PingBox.Models;

namespace PingBox.Services;

/// <summary>
/// 配置服务接口
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// 配置文件路径
    /// </summary>
    string ConfigFilePath { get; }
    
    /// <summary>
    /// 加载配置
    /// </summary>
    AppConfig Load();
    
    /// <summary>
    /// 保存配置
    /// </summary>
    void Save(AppConfig config);
    
    /// <summary>
    /// 从备份恢复配置
    /// </summary>
    bool RestoreFromBackup();
}