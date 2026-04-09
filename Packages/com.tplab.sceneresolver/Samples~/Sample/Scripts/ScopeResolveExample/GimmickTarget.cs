using UnityEngine;

namespace TpLab.SceneResolver.Samples.Sample.Scripts
{
    /// <summary>
    /// スコープサンプル用のターゲットコンポーネント。
    /// GimmickController から ResolveSource.Scope で参照される側。
    /// </summary>
    public class GimmickTarget : MonoBehaviour
    {
        [SerializeField]
        string gimmickId;

        /// <summary>
        /// このターゲットを識別するID
        /// </summary>
        public string GimmickId => gimmickId;

        /// <summary>
        /// ターゲットが起動したことをログ出力する
        /// </summary>
        public void Activate()
        {
            Debug.Log($"[GimmickTarget] '{gimmickId}' activated! (GameObject: {gameObject.name})");
        }
    }
}
