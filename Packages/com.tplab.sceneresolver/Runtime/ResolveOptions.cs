using System;

namespace TpLab.SceneResolver
{
    /// <summary>
    /// Resolve属性の解決動作を制御するオプション
    /// </summary>
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

        /// <summary>
        /// 実行時に動的生成されるオブジェクトを解決対象とする。
        /// エディタ時の静的解析では検証をスキップし、実行時の解決を許可する。
        /// </summary>
        AllowRuntime = 1 << 1,
    }
}