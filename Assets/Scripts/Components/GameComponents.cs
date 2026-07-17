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
        public int2 PendingDirection;
        public Random Rng;
        public bool Alive;
    }
    
    public struct FoodState : IComponentData
    {
        public int2 Position;
    }
    
    [InternalBufferCapacity(0)]
    public struct SnakeSegmentElement : IBufferElementData
    {
        public int2 Position; // index 0 = head
    }
    
    public struct HeadMarker : IComponentData { }
    
    public struct FoodMarker : IComponentData { }
}