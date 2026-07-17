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
            if (segCount == 0)
                return; // init tick hasn't run yet this game

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
            var renderBuffer = em.GetBuffer<SegmentEntityElement>(configEntity);
            float2 half = (float2)(config.GridSize - 1) * 0.5f;

            for (int i = 0; i < segCount; i++)
            {
                em.SetComponentData(renderBuffer[i].Value, new LocalTransform
                {
                    Position = new float3(segments[i].Position.x - half.x,
                                          segments[i].Position.y - half.y, 0f),
                    Rotation = quaternion.identity,
                    Scale    = i == 0 ? 0.95f : 0.85f
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