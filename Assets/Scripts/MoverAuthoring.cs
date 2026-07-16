using Unity.Entities;
using UnityEngine;

namespace Snake
{
    public class MoverAuthoring : MonoBehaviour
    {
        public Vector3 Velocity = new (1f, 0f, 0f);

        class Baker : Baker<MoverAuthoring>
        {
            public override void Bake(MoverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveSpeed { Velocity = authoring.Velocity });
            }
        }
    }
}