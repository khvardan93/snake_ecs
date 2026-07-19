using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Snake
{
    public class RenderColorAuthoring : MonoBehaviour
    {
        class Baker : Baker<RenderColorAuthoring>
        {
            public override void Bake(RenderColorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new URPMaterialPropertyBaseColor
                {
                    Value = new Unity.Mathematics.float4(1f, 1f, 1f, 1f)
                });
            }
        }
    }
}