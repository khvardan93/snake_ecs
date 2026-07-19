using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Snake
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct SnakeRenderSyncSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var configEntity = SystemAPI.GetSingletonEntity<GameConfig>();
            var config = em.GetComponentData<GameConfig>(configEntity);

            int segCount = em.GetBuffer<SnakeSegmentElement>(configEntity).Length;

            // --- 1. Grow the render pool to match the sim ---
            while (em.GetBuffer<SegmentEntityElement>(configEntity).Length < segCount)
            {
                var e = em.Instantiate(config.SegmentPrefab);      // STRUCTURAL CHANGE
                em.GetBuffer<SegmentEntityElement>(configEntity)   // re-fetch: old handle is dead
                  .Add(new SegmentEntityElement { Value = e });
            }

            // --- 2. Spawn the food visual once ---
            if (em.GetComponentData<FoodState>(configEntity).RenderEntity == Entity.Null)
            {
                var f = em.Instantiate(config.FoodPrefab);         // STRUCTURAL CHANGE
                var fs = em.GetComponentData<FoodState>(configEntity);
                fs.RenderEntity = f;
                em.SetComponentData(configEntity, fs);             // write the copy back
            }

            // --- 3. Position everything (no structural changes below this line) ---
            var segments = em.GetBuffer<SnakeSegmentElement>(configEntity);
            var renderNow = em.GetBuffer<SegmentEntityElement>(configEntity);
            float2 half = (float2)(config.GridSize - 1) * 0.5f;

            var snakeState = em.GetComponentData<SnakeState>(configEntity);
            float t = math.saturate(snakeState.TickTimer / config.TickInterval);
            
            for (int i = 0; i < renderNow.Length; i++)
            {
                bool active = i < segCount;
                // above the em.SetComponentData call, inside the loop body:
                float2 cell = math.lerp((float2)segments[i].PreviousPosition,
                    (float2)segments[i].Position, t);
                
                em.SetComponentData(renderNow[i].Value, new LocalTransform
                {
                    Position = active
                        ? new float3(cell.x - half.x, cell.y - half.y, 0f)
                        : float3.zero,
                    Rotation = quaternion.identity,
                    Scale = i == 0 ? 0.95f : 0.85f
                });
            }

            var food = em.GetComponentData<FoodState>(configEntity);
            em.SetComponentData(food.RenderEntity, new LocalTransform
            {
                Position = new float3(food.Position.x - half.x, food.Position.y - half.y, 0f),
                Rotation = quaternion.identity,
                Scale    = 0.7f
            });
        }
    }
}