using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// スコープサンプル用のコントローラーコンポーネント。
    /// <para>
    /// ResolveSource.Scope を使い、同じ ResolveScope 配下の GimmickTarget のみを参照解決する。
    /// これにより同一ギミックセットをシーンに複数配置しても、
    /// それぞれのコントローラーが自分のスコープ内のターゲットだけを参照できる。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 必須セットアップ:
    /// <code>
    /// [GimmickRoot]           ← ResolveScope を付与
    ///   ├─ GimmickController  ← このコンポーネント
    ///   └─ GimmickTarget
    /// </code>
    /// </remarks>
    public class GimmickController : MonoBehaviour
    {
        /// <summary>
        /// 同じ ResolveScope 配下の GimmickTarget を自動解決する。
        /// ResolveSource.Scene にするとシーン全体から探すため、
        /// 複数セット配置すると競合する点に注意。
        /// </summary>
        [Resolve(ResolveSource.Scope)]
        [SerializeField]
        GimmickTarget target;

        void Start()
        {
            Debug.Log($"[GimmickController] ('{gameObject.transform.parent?.name}') " +
                      $"Target resolved: {target != null}");

            if (target != null)
            {
                Debug.Log($"[GimmickController] Target GimmickId: '{target.GimmickId}'");
            }
        }

        /// <summary>
        /// スコープ内のターゲットを起動する
        /// </summary>
        [ContextMenu("Activate Target")]
        public void ActivateTarget()
        {
            if (target != null)
                target.Activate();
            else
                Debug.LogWarning("[GimmickController] Target is not resolved.");
        }
    }
}
