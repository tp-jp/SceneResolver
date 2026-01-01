using System;

namespace TpLab.SceneResolver
{
    /// <summary>
    /// フィールドの依存関係を解決するための属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ResolveAttribute : Attribute
    {
        public readonly ResolveSource Source;
        
        public ResolveAttribute(ResolveSource source = ResolveSource.Self)
        {
            Source = source;
        }
    }
}
