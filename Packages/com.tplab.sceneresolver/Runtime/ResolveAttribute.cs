using System;
using UnityEngine;

namespace TpLab.SceneResolver
{
    /// <summary>
    /// フィールドの依存関係を自動的に解決するための属性。
    /// MonoBehaviourのフィールドにこの属性を付けることで、指定したソースから対応するコンポーネントを自動的に取得できます。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ResolveAttribute : PropertyAttribute
    {
        /// <summary>
        /// コンポーネントを検索するソース（自身、親、子など）
        /// </summary>
        public readonly ResolveSource Source;
        
        /// <summary>
        /// 解決時のオプション設定
        /// </summary>
        public readonly ResolveOptions Options;

        /// <summary>
        /// ResolveAttribute のコンストラクタ。
        /// </summary>
        /// <param name="source">コンポーネントを検索するソース。デフォルトはSelf（自身のGameObject）</param>
        /// <param name="options">解決時のオプション。デフォルトはNone</param>
        public ResolveAttribute(
            ResolveSource source = ResolveSource.Self,
            ResolveOptions options = ResolveOptions.None
        )
        {
            Source = source;
            Options = options;
        }
    }
}