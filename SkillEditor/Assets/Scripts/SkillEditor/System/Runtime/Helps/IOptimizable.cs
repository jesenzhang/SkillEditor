
namespace TimeLine
{
    /// <summary>
    /// 预操作接口 在播放前加载所有时间线
    /// </summary>
    interface IOptimizable
    {
        // 开关
        bool CanOptimize { get; set; }
        
        // 操作函数
        void Optimize();
    }
}
