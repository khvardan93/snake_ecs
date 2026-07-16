using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Snake
{
    [BurstCompile]
    public partial struct SnakeTickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<GameConfig>();
            var snake = SystemAPI.GetSingletonRW<SnakeState>();

            snake.ValueRW.TickTimer += SystemAPI.Time.DeltaTime;
            if (snake.ValueRO.TickTimer < config.TickInterval)
                return;
            snake.ValueRW.TickTimer -= config.TickInterval;

            // --- everything below runs exactly once per tick ---
            UnityEngine.Debug.Log($"TICK, direction {snake.ValueRO.HeadDirection}");
        }
    }
}