using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// 親子関係の参照解決の例
    /// ResolveSource.ParentとChildrenを使って階層構造から参照を解決します
    /// </summary>
    public class ParentChildResolveExample : MonoBehaviour
    {
        // 親から取得
        [Resolve(ResolveSource.Parent)]
        [SerializeField]
        Rigidbody parentRigidbody;

        [Resolve(ResolveSource.Parent)]
        [SerializeField]
        BoxCollider parentCollider;

        // 子から取得
        [Resolve(ResolveSource.Children)]
        [SerializeField]
        MeshRenderer childRenderer;

        [Resolve(ResolveSource.Children)]
        [SerializeField]
        Light childLight;

        void Start()
        {
            Debug.Log("[ParentChildResolveExample] === Resolve Status ===");
            Debug.Log($"  Rigidbody (Parent): {parentRigidbody != null}");
            Debug.Log($"  BoxCollider (Parent): {parentCollider != null}");
            Debug.Log($"  MeshRenderer (Children): {childRenderer != null}");
            Debug.Log($"  Light (Children): {childLight != null}");

            if (parentRigidbody != null)
            {
                Debug.Log($"[ParentChildResolveExample] Parent name: {parentRigidbody.gameObject.name}");
            }

            if (childRenderer != null)
            {
                Debug.Log($"[ParentChildResolveExample] Child name: {childRenderer.gameObject.name}");
            }
        }
    }
}

