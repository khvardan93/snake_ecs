using Unity.Entities;
using Unity.Mathematics;

namespace Snake
{
    public struct GameConfig : IComponentData
    {
        public int2 GridSize;
        public float TickInterval;
        
        public Entity SegmentPrefab;
        public Entity FoodPrefab;
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
        public Entity RenderEntity;
    }
    
    [InternalBufferCapacity(0)]
    public struct SnakeSegmentElement : IBufferElementData
    {
        public int2 Position; // index 0 = head
    }
    
    [InternalBufferCapacity(0)]
    public struct SegmentEntityElement : IBufferElementData
    {
        public Entity Value;
    }
    
    public struct HeadMarker : IComponentData { }
    
    public struct FoodMarker : IComponentData { }
}