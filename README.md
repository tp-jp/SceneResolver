# SceneResolver
SceneResolverは、Unityシーンにおける信頼性の高い参照システムを確立する基盤システムです。  
属性ベースで依存関係を宣言的に解決し、コードをシンプルで保守しやすくします。

## 特徴

- 🎯 **宣言的な依存関係管理**: 属性を使って依存関係を明示的に宣言
- 🔍 **柔軟な検索スコープ**: Self, Parent, Children, Scene から選択可能
- 🛡️ **型安全**: コンパイル時の型チェックで安全性を確保
- 🔧 **解決オプション**: IncludeInactive オプションで非アクティブなオブジェクトも検索可能
- 📊 **ビジュアルエディタ**: エディタウィンドウで解決状況を一覧表示・検証

## 基本的な使い方

### 自身のコンポーネントを取得

```csharp
using UnityEngine;
using TpLab.SceneResolver;

public class MyComponent : MonoBehaviour
{
    [Resolve(ResolveSource.Self)]
    [SerializeField]
    private BoxCollider boxCollider;

    [Resolve(ResolveSource.Self)]
    [SerializeField]
    private Rigidbody rigidBody;
}
```

従来の`GetComponent`を手動で呼ぶ必要がなくなります。

### シーンから参照を取得

```csharp
[Resolve(ResolveSource.Scene)]
[SerializeField]
private Camera mainCamera;

[Resolve(ResolveSource.Scene)]
[SerializeField]
private AudioListener audioListener;
```

`FindObjectOfType`を明示的に呼ぶ必要がなくなります。

### 親子階層から取得

```csharp
// 親から取得
[Resolve(ResolveSource.Parent)]
[SerializeField]
private CharacterController controller;

// 子から取得
[Resolve(ResolveSource.Children)]
[SerializeField]
private MeshRenderer childRenderer;
```

### 複数の参照を配列として取得

配列型フィールドを使用すると、複数のコンポーネントを自動的に取得できます。

```csharp
[Resolve(ResolveSource.Children)]
[SerializeField]
private MeshRenderer[] childRenderers;

[Resolve(ResolveSource.Scene)]
[SerializeField]
private Light[] allLights;
```

### 非アクティブなオブジェクトも検索

`ResolveOptions.IncludeInactive`を使用すると、非アクティブなオブジェクトも検索対象に含めることができます。

```csharp
[Resolve(ResolveSource.Children, ResolveOptions.IncludeInactive)]
[SerializeField]
private Transform[] allChildTransforms; // 非アクティブな子も含む

[Resolve(ResolveSource.Scene, ResolveOptions.IncludeInactive)]
[SerializeField]
private AudioSource[] allAudioSources; // 非アクティブなAudioSourceも含む
```

## エディタウィンドウ

SceneResolverには、シーン内の全フィールド解決状況を確認できるエディタウィンドウが付属しています。

### 開き方

メニューから `Tools > Scene Resolve` を選択

### 機能

- **一覧表示**: シーン内の全Resolve属性付きフィールドの解決状況を表示
- **ステータスフィルター**: Success/Error でフィルタリング可能
- **ソート機能**: 各列をクリックして並び替え
- **GameObjectへのジャンプ**: GameObject列をクリックでHierarchyでハイライト
- **サマリー表示**: 成功/エラーの件数を表示
- **リフレッシュ**: Refreshボタンで最新状態に更新

### 表示内容

| 列 | 説明 |
|---|---|
| Status | 解決ステータス（Success/Error）バッジ表示 |
| GameObject | 対象のGameObject名（クリック可能） |
| Component | コンポーネント名 |
| Field | フィールド名 |
| Source | 検索ソース（Self/Parent/Children/Scene） |
| Message | 詳細メッセージ |

### エラーメッセージ

- **"Field type is not a Component."**: フィールドの型がComponentを継承していない
- **"No components found."**: 指定したソースでコンポーネントが見つからない
- **"Multiple components found."**: 単一フィールドなのに複数のコンポーネントが見つかった（配列型にする必要があります）

### 基本的なパターン

```csharp
public class PlayerController : MonoBehaviour
{
    // 自身のコンポーネント
    [Resolve(ResolveSource.Self)]
    [SerializeField]
    private CharacterController characterController;

    [Resolve(ResolveSource.Self)]
    [SerializeField]
    private Animator animator;

    // シーンからの参照
    [Resolve(ResolveSource.Scene)]
    [SerializeField]
    private Camera mainCamera;
}
```

### 親子階層の活用

```csharp
public class WeaponSystem : MonoBehaviour
{
    // 親のコンポーネント（プレイヤー本体）
    [Resolve(ResolveSource.Parent)]
    [SerializeField]
    private PlayerController player;

    // すべての子の発射ポイント（配列型で自動的に複数取得）
    [Resolve(ResolveSource.Children)]
    [SerializeField]
    private Transform[] firePoints;
}
```

### 複数オブジェクトの管理

```csharp
public class LightingManager : MonoBehaviour
{
    // シーン内のすべてのライト（非アクティブも含む）
    [Resolve(ResolveSource.Scene, ResolveOptions.IncludeInactive)]
    [SerializeField]
    private Light[] allLights;

    void SetBrightness(float intensity)
    {
        foreach (var light in allLights)
        {
            light.intensity = intensity;
        }
    }
}
```

### UI要素の検索

```csharp
public class UIManager : MonoBehaviour
{
    // 子階層の全てのButton（非アクティブも含む）
    [Resolve(ResolveSource.Children, ResolveOptions.IncludeInactive)]
    [SerializeField]
    private Button[] allButtons;

    // 子階層の全てのText要素
    [Resolve(ResolveSource.Children)]
    [SerializeField]
    private Text[] allTexts;

    void DisableAllButtons()
    {
        foreach (var button in allButtons)
        {
            button.interactable = false;
        }
    }
}
```

## ベストプラクティス

### ✅ 推奨

- 常に`[SerializeField]`と組み合わせて使用（Inspector での確認が可能）
- パフォーマンスが重要な場合は`Scene`より`Self`や`Parent`を優先
- 配列型フィールドを使用することで複数の参照を一度に取得可能
- 非アクティブなオブジェクトも検索したい場合は`ResolveOptions.IncludeInactive`を指定
- エディタウィンドウを使って解決状況を定期的に確認
- エラーが表示された場合は早めに修正（配列型にする、検索ソースを変更する等）

### ❌ 非推奨

- 実行時に頻繁に変更される参照への使用
- 動的に生成されるオブジェクトへの参照
- 循環参照の作成
- `IncludeInactive`の不必要な使用（パフォーマンス低下の可能性）

## サンプル

サンプルはPackage Managerの「Samples」セクションからインポートできます。

1. Package Manager を開く
2. SceneResolver パッケージを選択
3. Samples セクションから「Sample」をインポート

サンプルには以下が含まれています：
- 基本的な参照解決の例
- 親子階層の参照解決
- 複数参照の配列取得
- ResolveOptionsの使用例
- 複合的な使用例
- エディタウィンドウでの解決状況確認

## ライセンス

このプロジェクトのライセンスについては、LICENSEファイルを参照してください。
