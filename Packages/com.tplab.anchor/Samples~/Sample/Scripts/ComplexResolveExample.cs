using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// 複合的な参照解決の例
    /// 複数の解決方法を組み合わせて使用する実践的な例
    /// </summary>
    public class ComplexResolveExample : MonoBehaviour
    {
        // 自身のコンポーネント
        [Resolve]
        [SerializeField]
        MeshRenderer meshRenderer;

        [Resolve]
        [SerializeField]
        Collider targetCollider;

        // シーンから取得
        [Resolve(ResolveSource.Scene)]
        [SerializeField]
        Camera mainCamera;

        void Start()
        {
            LogResolveStatus();
            PerformActions();
        }

        void LogResolveStatus()
        {
            Debug.Log("[ComplexResolveExample] === Resolve Status ===");
            Debug.Log($"  MeshRenderer (Self): {meshRenderer != null}");
            Debug.Log($"  Collider (Self): {targetCollider != null}");
            Debug.Log($"  Camera (Scene): {mainCamera != null}");
        }

        void PerformActions()
        {
            if (meshRenderer != null && mainCamera != null)
            {
                // カメラとの距離を計算
                float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
                Debug.Log($"[ComplexResolveExample] Distance to camera: {distance}");

                // 距離に応じてマテリアルの色を変更
                if (meshRenderer.material != null)
                {
                    Color color = Color.Lerp(Color.green, Color.red, distance / 10f);
                    meshRenderer.material.color = color;
                }
            }

            if (targetCollider != null)
            {
                Debug.Log($"[ComplexResolveExample] Collider bounds: {targetCollider.bounds.size}");
            }
        }

        void OnDrawGizmos()
        {
            if (mainCamera != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, mainCamera.transform.position);
            }
        }
    }
}

