using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TpLab.SceneResolver.Editor
{
    /// <summary>
    /// フィールド解決の結果ステータス
    /// </summary>
    internal enum ResolveStatus
    {
        /// <summary>
        /// 正常に解決された
        /// </summary>
        Success,
        
        /// <summary>
        /// 実行時に動的生成されるオブジェクトへの参照（エディタ時には検証不可）
        /// </summary>
        Runtime,
        
        /// <summary>
        /// 解決に失敗した
        /// </summary>
        Error
    }

    /// <summary>
    /// フィールド解決の結果レポート
    /// </summary>
    internal sealed class ResolveReport
    {
        /// <summary>
        /// 解決ステータス
        /// </summary>
        public ResolveStatus Status { get; set; }
        
        /// <summary>
        /// 対象のGameObject
        /// </summary>
        public GameObject GameObject { get; set; }
        
        /// <summary>
        /// コンポーネント名
        /// </summary>
        public string ComponentName { get; set; }
        
        /// <summary>
        /// フィールド名
        /// </summary>
        public string FieldName { get; set; }
        
        /// <summary>
        /// 解決ソース
        /// </summary>
        public ResolveSource Source { get; set; }
        
        /// <summary>
        /// 詳細メッセージ
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// シーン内のResolve属性が付いたフィールドを分析し、解決状況をレポートする
    /// </summary>
    internal class ResolveAnalyzer
    {
        /// <summary>
        /// 分析結果のレポートリスト
        /// </summary>
        public List<ResolveReport> Reports { get; } = new();

        /// <summary>
        /// アクティブシーン内の全MonoBehaviourを分析し、Resolve属性が付いたフィールドを検証する
        /// </summary>
        public void Analyze()
        {
            var scene = SceneManager.GetActiveScene();
            var targets = scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<MonoBehaviour>(true))
                .ToList();

            Reports.Clear();
            foreach (var target in targets)
            {
                AnalyzeFields(target);
            }
        }

        /// <summary>
        /// 指定されたMonoBehaviourのResolve属性が付いたフィールドを分析
        /// </summary>
        /// <param name="target">分析対象のMonoBehaviour</param>
        void AnalyzeFields(MonoBehaviour target)
        {
            var fields = target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsDefined(typeof(ResolveAttribute), true));

            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<ResolveAttribute>();
                AnalyzeField(target, field, attribute);
            }
        }

        /// <summary>
        /// 個別のフィールドを分析し、解決候補を検証する。
        /// <see cref="ResolveSource.Scope"/> の場合は祖先に <see cref="ResolveScope"/> が存在するかも検証する。
        /// </summary>
        /// <param name="target">対象のMonoBehaviour</param>
        /// <param name="field">分析対象のフィールド</param>
        /// <param name="attribute">Resolve属性</param>
        void AnalyzeField(MonoBehaviour target, FieldInfo field, ResolveAttribute attribute)
        {
            // Scope指定時、祖先にResolveScopeが存在しなければ即エラーレポートを返す
            if (attribute.Source == ResolveSource.Scope && !HasScopeRoot(target.gameObject))
            {
                Reports.Add(new ResolveReport
                {
                    Status = ResolveStatus.Error,
                    GameObject = target.gameObject,
                    ComponentName = target.GetType().Name,
                    FieldName = field.Name,
                    Source = attribute.Source,
                    Message = "No ResolveScope found in ancestors. Attach ResolveScope to a parent GameObject."
                });
                return;
            }

            var fieldType = field.FieldType;
            var candidates = ResolveComponentFinder.Find(
                target,
                fieldType.IsArray ? fieldType.GetElementType() : fieldType,
                attribute.Source,
                attribute.Options
            );
            ValidateAndReport(target, field, attribute, candidates);
        }

        /// <summary>
        /// 指定された GameObject の祖先に <see cref="ResolveScope"/> が存在するか確認する。
        /// </summary>
        /// <param name="self">確認対象のGameObject</param>
        /// <returns>祖先にResolveScopeが存在すれば<c>true</c></returns>
        static bool HasScopeRoot(GameObject self)
        {
            var current = self.transform.parent;
            while (current != null)
            {
                if (current.TryGetComponent<ResolveScope>(out _))
                    return true;
                current = current.parent;
            }
            return false;
        }

        /// <summary>
        /// 解決候補を検証し、結果をレポートに追加
        /// </summary>
        /// <param name="target">対象のMonoBehaviour</param>
        /// <param name="field">検証対象のフィールド</param>
        /// <param name="attribute">Resolve属性</param>
        /// <param name="candidates">解決候補のコンポーネントリスト</param>
        void ValidateAndReport(
            MonoBehaviour target,
            FieldInfo field,
            ResolveAttribute attribute,
            IEnumerable<Component> candidates)
        {
            var results = candidates.ToList();
            var fieldType = field.FieldType;
            var isArray = fieldType.IsArray;
            var elementType = isArray
                ? fieldType.GetElementType()
                : fieldType;

            ResolveStatus status;
            string message;

            // 型チェック：Componentを継承しているか
            if (!typeof(Component).IsAssignableFrom(elementType))
            {
                status = ResolveStatus.Error;
                message = "Field type is not a Component.";
            }
            // AllowRuntimeオプション付きフィールド：実行時解決を許可
            else if (attribute.Options.HasFlag(ResolveOptions.AllowRuntime))
            {
                if (results.Count == 0)
                {
                    status = ResolveStatus.Runtime;
                    message = "Will be resolved at runtime (dynamically generated object).";
                }
                else
                {
                    // 実行時に生成される予定だが、既にエディタ時に存在する場合は成功扱い
                    status = ResolveStatus.Success;
                    message = "Resolved successfully (runtime-allowed field).";
                }
            }
            // 候補数チェック：解決候補が見つからない
            else if (results.Count == 0)
            {
                status = ResolveStatus.Error;
                message = "No components found.";
            }
            // 単一フィールドチェック：配列でないのに複数候補がある
            else if (!isArray && results.Count > 1)
            {
                status = ResolveStatus.Error;
                message = "Multiple components found.";
            }
            // 正常に解決
            else
            {
                status = ResolveStatus.Success;
                message = "Resolved successfully.";
            }

            Reports.Add(new ResolveReport
            {
                Status = status,
                GameObject = target.gameObject,
                ComponentName = target.GetType().Name,
                FieldName = field.Name,
                Source = attribute.Source,
                Message = message
            });
        }
    }
}