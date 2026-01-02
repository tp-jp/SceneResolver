using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TpLab.SceneResolver.Editor
{
    /// <summary>
    /// 指定されたResolveSourceに基づいてコンポーネントを検索するユーティリティクラス
    /// </summary>
    internal static class ResolveComponentFinder
    {
        /// <summary>
        /// 指定されたResolveSourceに基づいてコンポーネントを検索する。
        /// </summary>
        /// <param name="context">検索の起点となるコンポーネント</param>
        /// <param name="targetType">検索対象のコンポーネント</param>
        /// <param name="source">検索範囲を決定する ResolveSource</param>
        /// <param name="options">解決時のオプション</param>
        /// <returns>コンポーネントの列挙</returns>
        public static IEnumerable<Component> Find(
            Component context,
            Type targetType,
            ResolveSource source,
            ResolveOptions options
        )
        {
            var includeInactive = options.HasFlag(ResolveOptions.IncludeInactive);
            return source switch
            {
                ResolveSource.Self => context.GetComponents(targetType),
                ResolveSource.Parent => context.GetComponentsInParent(targetType, includeInactive),
                ResolveSource.Children => context.GetComponentsInChildren(targetType, includeInactive),
                ResolveSource.Scene => Object.FindObjectsOfType(targetType, includeInactive).OfType<Component>(),
                _ => Enumerable.Empty<Component>()
            };
        }
    }
}