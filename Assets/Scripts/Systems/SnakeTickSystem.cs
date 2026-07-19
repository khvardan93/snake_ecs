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
            var configEntity = SystemAPI.GetSingletonEntity<GameConfig>();
            var snake = SystemAPI.GetSingletonRW<SnakeState>();
            var segments = SystemAPI.GetBuffer<SnakeSegmentElement>(configEntity);

            // --- Init: empty buffer means "new game" ---
            if (segments.Length == 0)
            {
                int2 center = config.GridSize / 2;
                for (int i = 0; i < 3; i++)
                {
                    segments.Add(new SnakeSegmentElement
                    {
                        Position = new int2(center.x - i, center.y),
                        PreviousPosition = new int2(center.x - i, center.y)
                    });
                }
                
                RespawnFood(ref state, configEntity, config);
                return;
            }
            
            if (!snake.ValueRO.Alive)
                return;

            // --- Tick accumulator (unchanged) ---
            snake.ValueRW.TickTimer += SystemAPI.Time.DeltaTime;
            if (snake.ValueRO.TickTimer < config.TickInterval)
                return;
            snake.ValueRW.TickTimer -= config.TickInterval;

            // --- Move: new head in front, drop the tail ---
            snake.ValueRW.HeadDirection = snake.ValueRO.PendingDirection;
            int2 newHead = segments[0].Position + snake.ValueRO.HeadDirection;
            
            if (math.any(newHead < 0) || math.any(newHead >= config.GridSize))
            {
                snake.ValueRW.Alive = false;
                snake.ValueRW.TickTimer = config.TickInterval; 
                return;
            }
            
            bool ate = math.all(newHead == SystemAPI.GetComponent<FoodState>(configEntity).Position);

            int checkCount = ate ? segments.Length : segments.Length - 1;
            for (int i = 0; i < checkCount; i++)
            {
                if (math.all(segments[i].Position == newHead))
                {
                    snake.ValueRW.Alive = false;
                    snake.ValueRW.TickTimer = config.TickInterval; 
                    return;
                }
            }
            
            int2 dropped = segments[segments.Length - 1].Position;
            
            segments.Insert(0, new SnakeSegmentElement { Position = newHead });
            
            if (ate)
                RespawnFood(ref state, configEntity, config);
            else
                segments.RemoveAt(segments.Length - 1);

            for (int i = 0; i < segments.Length; i++)
            {
                var s = segments[i];
                s.PreviousPosition = i + 1 < segments.Length
                    ? segments[i + 1].Position                   // slide from where my follower stands
                    : (ate ? s.Position : dropped);              // tail: vacated cell; grown tail: born in place
                segments[i] = s;                                 // write the copy back
            }
        }
        
        static void RespawnFood(ref SystemState state, Entity configEntity, in GameConfig config)
        {
            var em = state.EntityManager;
            var segments = em.GetBuffer<SnakeSegmentElement>(configEntity);
            var snake = em.GetComponentData<SnakeState>(configEntity);

            int2 pos;
            bool onSnake;
            int guard = 0;
            do
            {
                pos = snake.Rng.NextInt2(int2.zero, config.GridSize);
                onSnake = false;
                for (int i = 0; i < segments.Length; i++)
                    if (math.all(segments[i].Position == pos)) { onSnake = true; break; }
            } while (onSnake && ++guard < 1000);

            var food = em.GetComponentData<FoodState>(configEntity);
            food.Position = pos;
            em.SetComponentData(configEntity, food);
            em.SetComponentData(configEntity, snake); // <-- the important line
        }
    }
}