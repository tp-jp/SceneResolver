using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// 基本的な参照解決の例
    /// ResolveSource.Selfを使って自身のGameObjectから必要なコンポーネントを取得します
    /// </summary>
    public class BasicResolveExample : MonoBehaviour
    {
        [Resolve]
        [SerializeField]
        BoxCollider boxCollider;

        [Resolve]
        [SerializeField]
        Rigidbody rigidBody;

        void Start()
        {
            Debug.Log($"[BasicResolveExample] BoxCollider resolved: {boxCollider != null}");
            Debug.Log($"[BasicResolveExample] Rigidbody resolved: {rigidBody != null}");
            
            if (boxCollider != null)
            {
                Debug.Log($"[BasicResolveExample] BoxCollider size: {boxCollider.size}");
            }
        }
    }
}

