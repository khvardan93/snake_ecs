using Unity.Entities;
using UnityEngine;

namespace Snake
{
    public class HeadMarkerAuthoring : MonoBehaviour
    {
        class Baker : Baker<HeadMarkerAuthoring>
        {
            public override void Bake(HeadMarkerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<HeadMarker>(entity);
            }
        }
    }
}