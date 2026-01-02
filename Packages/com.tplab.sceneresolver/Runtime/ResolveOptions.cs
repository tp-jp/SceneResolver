using System;

namespace TpLab.SceneResolver
{
    [Flags]
    public enum ResolveOptions
    {
        /// <summary>
        /// オプションなし
        /// </summary>
        None = 0,

        /// <summary>
        /// 非アクティブなGameObjectも参照解決の対象に含める
        /// </summary>
        IncludeInactive = 1 << 0,
    }
}