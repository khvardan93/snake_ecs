using Unity.Entities;
using Unity.Mathematics;

namespace Snake
{
    public struct GameConfig : IComponentData
    {
        public int2 GridSize;
        public float TickInterval;
    }

    public struct SnakeState : IComponentData
    {
        public int2 HeadDirection;
        public float TickTimer;
    }
}