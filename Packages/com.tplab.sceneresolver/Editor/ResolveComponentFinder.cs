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
        /// <param name="targetType">検索対象のコンポーネント型</param>
        /// <param name="source">検索範囲を決定する <see cref="ResolveSource"/></param>
        /// <param name="options">解決時のオプション</param>
        /// <returns>条件に一致するコンポーネントの列挙</returns>
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
                ResolveSource.Self     => context.GetComponents(targetType),
                ResolveSource.Parent   => context.GetComponentsInParent(targetType, includeInactive),
                ResolveSource.Children => context.GetComponentsInChildren(targetType, includeInactive),
                ResolveSource.Scene    => Object.FindObjectsOfType(targetType, includeInactive).OfType<Component>(),
                ResolveSource.Scope    => FindInScope(context, targetType, includeInactive),
                _                      => Enumerable.Empty<Component>()
            };
        }

        /// <summary>
        /// 最も近い祖先の <see cref="ResolveScope"/> 配下からコンポーネントを検索する。
        /// <see cref="ResolveScope"/> が見つからない場合はシーンルートを起点とする。
        /// </summary>
        /// <param name="context">検索の起点となるコンポーネント</param>
        /// <param name="targetType">検索対象のコンポーネント型</param>
        /// <param name="includeInactive">非アクティブなGameObjectを含めるか</param>
        /// <returns>スコープ内に一致するコンポーネントの列挙</returns>
        static IEnumerable<Component> FindInScope(Component context, Type targetType, bool includeInactive)
        {
            var scopeRoot = FindScopeRoot(context.gameObject);
            return scopeRoot != null
                ? scopeRoot.GetComponentsInChildren(targetType, includeInactive)
                : Enumerable.Empty<Component>();
        }

        /// <summary>
        /// 指定されたGameObjectの祖先を遡り、最も近い <see cref="ResolveScope"/> を持つGameObjectを返す。
        /// 見つからない場合は <c>null</c> を返す。
        /// </summary>
        /// <param name="self">探索の起点となるGameObject</param>
        /// <returns>最も近い祖先の <see cref="ResolveScope"/> を持つGameObject、またはnull</returns>
        static GameObject FindScopeRoot(GameObject self)
        {
            var current = self.transform.parent;
            while (current != null)
            {
                if (current.TryGetComponent<ResolveScope>(out _))
                    return current.gameObject;
                current = current.parent;
            }
            return null;
        }
    }
}

