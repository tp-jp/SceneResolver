using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TpLab.SceneResolver.Editor
{
    /// <summary>
    /// Resolve属性が付与されたフィールドの参照解決を処理するプロセッサー
    /// </summary>
    internal static class ResolveProcessor
    {
        /// <summary>
        /// 指定されたMonoBehaviourのすべてのResolve属性付きフィールドを解決します
        /// </summary>
        /// <param name="target">解決対象のMonoBehaviour</param>
        public static void ResolveFields(MonoBehaviour target)
        {
            var fields = target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsDefined(typeof(ResolveAttribute), true));

            foreach (var field in fields)
            {
                var attribute  = field.GetCustomAttribute<ResolveAttribute>();
                ResolveField(target, field, attribute);
            }
        }
        
        /// <summary>
        /// 単一のフィールドに対して参照解決を実行します
        /// </summary>
        /// <param name="target">解決対象のMonoBehaviour</param>
        /// <param name="field">解決するフィールド情報</param>
        /// <param name="attribute">フィールドに付与されたResolve属性</param>
        static void ResolveField(MonoBehaviour target, FieldInfo field, ResolveAttribute attribute)
        {
            var fieldType = field.FieldType;
            var candidates = ResolveBySource(
                target,
                fieldType.IsArray ? fieldType.GetElementType() : fieldType,
                attribute.Source
            );
            AssignResolvedValue(target, field, candidates);
        }
        
        /// <summary>
        /// 指定されたResolveSourceに基づいてコンポーネントを検索します
        /// </summary>
        /// <param name="self">検索の基準となるMonoBehaviour</param>
        /// <param name="targetType">検索するコンポーネントの型</param>
        /// <param name="source">検索範囲を指定するResolveSource</param>
        /// <returns>見つかったコンポーネントのコレクション</returns>
        /// <exception cref="ArgumentOutOfRangeException">サポートされていないResolveSourceが指定された場合</exception>
        static IEnumerable<Component> ResolveBySource(
            MonoBehaviour self,
            Type targetType,
            ResolveSource source)
        {
            return source switch
            {
                ResolveSource.Self => self.GetComponents(targetType),
                ResolveSource.Parent => self.GetComponentsInParent(targetType),
                ResolveSource.Children => self.GetComponentsInChildren(targetType),
                ResolveSource.Scene => Object.FindObjectsOfType(targetType).OfType<Component>(),
                _ => throw new ArgumentOutOfRangeException(nameof(source))
            };
        }
        
        /// <summary>
        /// 解決されたコンポーネントをフィールドに割り当てます
        /// </summary>
        /// <param name="self">対象のMonoBehaviour</param>
        /// <param name="field">値を設定するフィールド情報</param>
        /// <param name="candidates">解決されたコンポーネントの候補</param>
        /// <remarks>
        /// 配列型のフィールドには複数の候補を配列として割り当て、
        /// 非配列型のフィールドには単一の候補のみを割り当てます。
        /// 条件を満たさない場合はエラーログを出力します。
        /// </remarks>
        static void AssignResolvedValue(
            MonoBehaviour self,
            FieldInfo field,
            IEnumerable<Component> candidates)
        {
            var results = candidates.ToList();

            var fieldType = field.FieldType;
            var isArray = fieldType.IsArray;

            if (results.Count == 0)
            {
                Logger.LogError($"Resolve failed: {field.Name}", self);
                return;
            }
            if (!isArray)
            {
                if (results.Count != 1)
                {
                    Logger.LogError($"Resolve failed: {field.Name}", self);
                    return;
                }
                field.SetValue(self, results[0]);
            }
            else
            {
                var elementType = fieldType.GetElementType();
                var typedArray = Array.CreateInstance(elementType, results.Count);
                Array.Copy(results.ToArray(), typedArray, results.Count);
                field.SetValue(self, typedArray);
            }
        }
    }
}
