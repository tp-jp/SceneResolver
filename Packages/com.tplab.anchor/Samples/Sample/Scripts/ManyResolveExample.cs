using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// 複数の参照を配列として解決する例
    /// 複数のコンポーネントを配列として取得します
    /// </summary>
    public class ManyResolveExample : MonoBehaviour
    {
        // 子要素のすべてのMeshRendererを取得
        [Resolve(ResolveSource.Children)]
        [SerializeField]
        MeshRenderer[] childRenderers;

        // シーン内のすべてのLightを取得
        [Resolve(ResolveSource.Scene)]
        [SerializeField]
        Light[] allLights;

        void Start()
        {
            Debug.Log("[ManyResolveExample] === Resolve Status ===");
            Debug.Log($"  Child MeshRenderers count: {childRenderers?.Length ?? 0}");
            Debug.Log($"  All Lights count: {allLights?.Length ?? 0}");

            // すべての子レンダラーを列挙
            if (childRenderers != null && childRenderers.Length > 0)
            {
                Debug.Log($"[ManyResolveExample] Found {childRenderers.Length} child renderers:");
                for (int i = 0; i < childRenderers.Length; i++)
                {
                    Debug.Log($"  [{i}] {childRenderers[i].gameObject.name}");
                }
            }

            // すべてのライトを列挙
            if (allLights != null && allLights.Length > 0)
            {
                Debug.Log($"[ManyResolveExample] Found {allLights.Length} lights:");
                for (int i = 0; i < allLights.Length; i++)
                {
                    Debug.Log($"  [{i}] {allLights[i].gameObject.name} - {allLights[i].type}");
                }
            }
        }

        void Update()
        {
            // すべての子レンダラーの色を時間に応じて変化させる
            if (childRenderers != null)
            {
                Color color = Color.HSVToRGB((Time.time * 0.1f) % 1f, 0.7f, 1f);
                foreach (var childRenderer in childRenderers)
                {
                    if (childRenderer != null && childRenderer.material != null)
                    {
                        childRenderer.material.color = color;
                    }
                }
            }
        }
    }
}

