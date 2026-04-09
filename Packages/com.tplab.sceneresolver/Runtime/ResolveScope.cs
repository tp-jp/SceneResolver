using UnityEngine;

namespace TpLab.SceneResolver
{
    /// <summary>
    /// <see cref="ResolveSource.Scope"/> の解決範囲を定義するマーカーコンポーネント。
    /// このコンポーネントを付与した GameObject の子孫が解決対象となる。
    /// </summary>
    /// <remarks>
    /// 同一シーンに同じギミックセットを複数配置する場合など、
    /// シーン全体ではなく特定の階層内に閉じた参照解決が必要な場合に使用する。
    /// <para>
    /// 使用例:
    /// <code>
    /// [GimmickRoot]  ← ResolveScopeを付与
    ///   ├─ GimmickController  ← [Resolve(ResolveSource.Scope)]
    ///   └─ GimmickTarget
    /// </code>
    /// </para>
    /// </remarks>
    [AddComponentMenu("SceneResolver/ResolveScope")]
    [DisallowMultipleComponent]
    public class ResolveScope : MonoBehaviour
    {
    }
}