using Unity.Entities;
using UnityEngine;

namespace Snake
{
    public class FoodMarkerAuthoring : MonoBehaviour
    {
        class Baker : Baker<FoodMarkerAuthoring>
        {
            public override void Bake(FoodMarkerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<FoodMarker>(entity);
            }
        }
    }
}