using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Snake
{
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (transform, speed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>>())
            {
                transform.ValueRW.Position += speed.ValueRO.Velocity * dt;
            }
        }
    }
}