using Unity.Entities;
using Unity.Mathematics;

namespace Snake
{
    public struct MoveSpeed : IComponentData
    {
        public float3 Velocity;
    }
}