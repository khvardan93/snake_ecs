using Unity.Mathematics;

namespace Snake
{
    public static class Palette
    {
        public static readonly float4 Head     = new float4(0.0356f, 0.8715f, 0.7455f, 1f); // #35F0E0
        public static readonly float4 Body0    = new float4(0.0185f, 0.6723f, 0.7455f, 1f); // #25D6E0
        public static readonly float4 Body1    = new float4(0.0048f, 0.1120f, 0.1560f, 1f); // #0F5E6E
        public static readonly float4 Food     = new float4(1.0000f, 0.1946f, 0.0284f, 1f); // #FF7A2F
        public static readonly float4 EatFlash = new float4(0.4790f, 1.0000f, 0.9387f, 1f); // #B8FFF8
    }
}