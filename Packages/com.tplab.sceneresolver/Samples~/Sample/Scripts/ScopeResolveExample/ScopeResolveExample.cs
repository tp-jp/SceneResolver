using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// ResolveSource.Scope を使ったサンプルの動作確認スクリプト。
    /// <para>
    /// 同一ギミックセット（GimmickController + GimmickTarget）を
    /// シーンに複数配置しても、それぞれが自スコープ内のみを参照できることを示す。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 推奨シーン構成:
    /// <code>
    /// ScopeResolveExample      ← このコンポーネントを付与
    ///
    /// [GimmickRoot_A]          ← ResolveScope を付与
    ///   ├─ GimmickController   (GimmickId: "A" のターゲットのみ解決される)
    ///   └─ GimmickTarget       (GimmickId: "A")
    ///
    /// [GimmickRoot_B]          ← ResolveScope を付与
    ///   ├─ GimmickController   (GimmickId: "B" のターゲットのみ解決される)
    ///   └─ GimmickTarget       (GimmickId: "B")
    /// </code>
    /// </remarks>
    public class ScopeResolveExample : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("=== ScopeResolveExample ===");
            Debug.Log("各GimmickControllerが自スコープ内のGimmickTargetのみを解決します。");
            Debug.Log("Consoleログで各ControllerがどのTargetと紐付いたか確認してください。");
        }
    }
}
