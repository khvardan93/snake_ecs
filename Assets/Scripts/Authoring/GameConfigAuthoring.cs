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

        class Baker : Baker<GameConfigAuthoring>
        {
            public override void Bake(GameConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new GameConfig
                {
                    GridSize = new int2(authoring.GridWidth, authoring.GridHeight),
                    TickInterval = authoring.TickInterval
                });

                AddComponent(entity, new SnakeState
                {
                    HeadDirection = new int2(1, 0)
                });
            }
        }
    }
}