using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
            var configEntity = SystemAPI.GetSingletonEntity<GameConfig>();
            var snake = SystemAPI.GetSingletonRW<SnakeState>();
            var segments = SystemAPI.GetBuffer<SnakeSegmentElement>(configEntity);

            // --- Init: empty buffer means "new game" ---
            if (segments.Length == 0)
            {
                int2 center = config.GridSize / 2;
                for (int i = 0; i < 3; i++)
                    segments.Add(new SnakeSegmentElement { Position = new int2(center.x - i, center.y) });
                return;
            }

            // --- Tick accumulator (unchanged) ---
            snake.ValueRW.TickTimer += SystemAPI.Time.DeltaTime;
            if (snake.ValueRO.TickTimer < config.TickInterval)
                return;
            snake.ValueRW.TickTimer -= config.TickInterval;

            // --- Move: new head in front, drop the tail ---
            snake.ValueRW.HeadDirection = snake.ValueRO.PendingDirection;
            int2 newHead = segments[0].Position + snake.ValueRO.HeadDirection;
            segments.Insert(0, new SnakeSegmentElement { Position = newHead });
            segments.RemoveAt(segments.Length - 1);
            
            foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<HeadMarker>())
            {
                transform.ValueRW.Position = new float3(newHead.x, newHead.y, 0f);
            }
        }
    }
}