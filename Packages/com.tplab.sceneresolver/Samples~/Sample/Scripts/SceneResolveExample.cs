using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// シーン内から参照を解決する例
    /// ResolveSource.Sceneを使ってシーン全体から特定のコンポーネントを検索して取得します
    /// </summary>
    public class SceneResolveExample : MonoBehaviour
    {
        [Resolve(ResolveSource.Scene)]
        [SerializeField]
        Camera mainCamera;

        [Resolve(ResolveSource.Scene)]
        [SerializeField]
        AudioListener audioListener;

        void Start()
        {
            Debug.Log($"[SceneResolveExample] Camera resolved: {mainCamera != null}");
            Debug.Log($"[SceneResolveExample] AudioListener resolved: {audioListener != null}");

            if (mainCamera != null)
            {
                Debug.Log($"[SceneResolveExample] Camera name: {mainCamera.gameObject.name}");
            }
        }
    }
}

