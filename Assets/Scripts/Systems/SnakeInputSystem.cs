using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Snake
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class SnakeInputSystem : SystemBase
    {
        protected override void OnCreate() => RequireForUpdate<SnakeState>();

        protected override void OnUpdate()
        {
            var state = SystemAPI.GetSingletonRW<SnakeState>();

            int2 dir = int2.zero;
            if      (Input.GetKeyDown(KeyCode.UpArrow)    || Input.GetKeyDown(KeyCode.W)) dir = new int2(0, 1);
            else if (Input.GetKeyDown(KeyCode.DownArrow)  || Input.GetKeyDown(KeyCode.S)) dir = new int2(0, -1);
            else if (Input.GetKeyDown(KeyCode.LeftArrow)  || Input.GetKeyDown(KeyCode.A)) dir = new int2(-1, 0);
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) dir = new int2(1, 0);

            if (!math.all(dir == int2.zero) && !math.all(dir == -state.ValueRO.HeadDirection))
            {
                state.ValueRW.PendingDirection = dir;
            }
            
            if (!state.ValueRO.Alive && Input.GetKeyDown(KeyCode.Space))
            {
                var entity = SystemAPI.GetSingletonEntity<SnakeState>();
                SystemAPI.GetBuffer<SnakeSegmentElement>(entity).Clear();

                state.ValueRW.Alive            = true;
                state.ValueRW.HeadDirection    = new int2(1, 0);
                state.ValueRW.PendingDirection = new int2(1, 0);
                state.ValueRW.TickTimer        = 0f;
                state.ValueRW.Rng = Unity.Mathematics.Random.CreateFromIndex((uint)UnityEngine.Time.frameCount + 1);
            }
        }
    }
}