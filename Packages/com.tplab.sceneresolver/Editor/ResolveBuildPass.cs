using System.Linq;
using TpLab.SceneFlow.Editor.Cores;
using TpLab.SceneFlow.Editor.Passes;
using UnityEngine;

namespace TpLab.SceneResolver.Editor
{
    public class ResolveBuildPass : IPass
    {
        public override void Execute(SceneFlowContext context)
        {
            var scene = context.Scene;
            var targets = scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<MonoBehaviour>(true))
                .ToList();

            foreach (var target in targets)
            {
                ResolveProcessor.ResolveFields(target);
            }
        }
    }
}
