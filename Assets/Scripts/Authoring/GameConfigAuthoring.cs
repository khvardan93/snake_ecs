using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Snake
{
    public class GameConfigAuthoring : MonoBehaviour
    {
        public int GridWidth = 20;
        public int GridHeight = 14;
        public float TickInterval = 0.16f;

        public GameObject SegmentPrefab;
        public GameObject FoodPrefab;

        class Baker : Baker<GameConfigAuthoring>
        {
            public override void Bake(GameConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new GameConfig
                {
                    GridSize = new int2(authoring.GridWidth, authoring.GridHeight),
                    TickInterval = authoring.TickInterval,
                    SegmentPrefab = GetEntity(authoring.SegmentPrefab, TransformUsageFlags.Dynamic),
                    FoodPrefab = GetEntity(authoring.FoodPrefab, TransformUsageFlags.Dynamic)
                });

                AddComponent(entity, new SnakeState
                {
                    HeadDirection = new int2(1, 0),
                    PendingDirection = new int2(1, 0),
                    Rng = Unity.Mathematics.Random.CreateFromIndex(0x9F6ABC1u),
                    Alive = true
                });
                
                AddComponent(entity, new FoodState());
                
                AddBuffer<SnakeSegmentElement>(entity);
                AddBuffer<SegmentEntityElement>(entity);
            }
        }
    }
}