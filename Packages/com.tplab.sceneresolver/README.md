# SceneResolver
SceneResolverは、Unityシーンにおける信頼性の高い参照システムを確立する基盤システムです。  
属性ベースで依存関係を宣言的に解決し、コードをシンプルで保守しやすくします。

詳細なドキュメントはプロジェクトルートの [README.md](../../../../README.md) を参照してください。

## ResolveSource 一覧

| ソース | 説明 |
|---|---|
| `Self` | 自身の GameObject から解決 |
| `Parent` | 親 GameObject から解決 |
| `Children` | 子 GameObject から解決 |
| `Scene` | シーン全体から解決 |
| `Scope` | 最も近い祖先の `ResolveScope` 配下から解決 |

## ResolveSource.Scope の使い方

同一ギミックセットをシーンに複数配置する場合、親 GameObject に `ResolveScope` を付与し、  
フィールドの `ResolveSource` を `Scope` に指定するだけで階層内に閉じた解決が可能です。

```csharp
// 親に ResolveScope を付与し、子コンポーネントで Scope を指定
public class GimmickController : MonoBehaviour
{
    [Resolve(ResolveSource.Scope)]
    [SerializeField]
    private GimmickTarget target;
}
```

