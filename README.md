# SceneResolver
SceneResolverは、Unityシーンにおける信頼性の高い参照システムを確立する基盤システムです。  
属性ベースで依存関係を宣言的に解決し、コードをシンプルで保守しやすくします。

## 特徴

- 🎯 **宣言的な依存関係管理**: 属性を使って依存関係を明示的に宣言
- 🔍 **柔軟な検索スコープ**: Self, Parent, Children, Scene から選択可能
- 🛡️ **型安全**: コンパイル時の型チェックで安全性を確保
- 📝 **エディタ統合**: Inspectorで解決結果を確認可能

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

## API リファレンス

### ResolveAttribute

```csharp
public class ResolveAttribute : Attribute
{
    public ResolveAttribute(ResolveSource source = ResolveSource.Self)
}
```

フィールドの依存関係を解決するための属性です。

**パラメータ:**
- `source`: 参照の解決元（デフォルト: `ResolveSource.Self`）

**配列型フィールド:**

フィールドが配列型の場合、自動的に複数の結果を取得します。

```csharp
// 単一の参照
[Resolve(ResolveSource.Scene)]
[SerializeField]
private Camera mainCamera;

// 複数の参照（配列型）
[Resolve(ResolveSource.Scene)]
[SerializeField]
private Camera[] allCameras;
```

### ResolveSource

```csharp
public enum ResolveSource
{
    Self,      // 自身のGameObjectから検索
    Parent,    // 親GameObjectから検索
    Children,  // 子GameObjectから検索
    Scene,     // シーン全体から検索
}
```


## 使用例

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
    // シーン内のすべてのライト（配列型で自動的に複数取得）
    [Resolve(ResolveSource.Scene)]
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

## ベストプラクティス

### ✅ 推奨

- 常に`[SerializeField]`と組み合わせて使用（Inspector での確認が可能）
- パフォーマンスが重要な場合は`Scene`より`Self`や`Parent`を優先
- 配列型フィールドを使用することで複数の参照を一度に取得可能

### ❌ 非推奨

- 実行時に頻繁に変更される参照への使用
- 動的に生成されるオブジェクトへの参照
- 循環参照の作成

## サンプル

サンプルはPackage Managerの「Samples」セクションからインポートできます。

1. Package Manager を開く
2. SceneResolver パッケージを選択
3. Samples セクションから「Sample」をインポート

サンプルには以下が含まれています：
- 基本的な参照解決の例
- 親子階層の参照解決
- 複数参照の配列取得
- 複合的な使用例

## ライセンス

このプロジェクトのライセンスについては、LICENSEファイルを参照してください。
