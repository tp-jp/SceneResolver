namespace TpLab.SceneResolver
{
    /// <summary>
    /// 参照解決のソースを指定する列挙型
    /// </summary>
    public enum ResolveSource
    {
        /// <summary>
        /// 自身のコンポーネントから解決する
        /// </summary>
        Self,
        
        /// <summary>
        /// 親オブジェクトのコンポーネントから解決する
        /// </summary>
        Parent,
        
        /// <summary>
        /// 子オブジェクトのコンポーネントから解決する
        /// </summary>
        Children,
        
        /// <summary>
        /// シーン全体から解決する
        /// </summary>
        Scene,
    }
}