using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace Snake
{
    // SubScene.AutoLoadScene relies on background streaming that can stall on WebGL
    // (single-threaded, no worker jobs), leaving the scene stuck at a blank clear-color
    // screen with no entities ever spawned. Force a blocking load explicitly instead.
    public partial struct SubSceneLoaderSystem : ISystem
    {
        bool _requested;
        bool _loaded;

        public void OnUpdate(ref SystemState state)
        {
            if (_loaded)
                return;

            if (!_requested)
            {
                foreach (var sceneReference in SystemAPI.Query<RefRO<SceneReference>>())
                {
                    SceneSystem.LoadSceneAsync(state.WorldUnmanaged, sceneReference.ValueRO.SceneGUID, new SceneSystem.LoadParameters
                    {
                        Flags = SceneLoadFlags.BlockOnImport | SceneLoadFlags.BlockOnStreamIn
                    });
                }
                _requested = true;
                return;
            }

            bool anyFound = false;
            bool allLoaded = true;
            foreach (var (_, entity) in SystemAPI.Query<RefRO<SceneReference>>().WithEntityAccess())
            {
                anyFound = true;
                if (!SceneSystem.IsSceneLoaded(state.WorldUnmanaged, entity))
                    allLoaded = false;
            }

            if (anyFound && allLoaded)
            {
                _loaded = true;
                Debug.Log("SubSceneLoaderSystem: subscene loaded.");
            }
        }
    }
}
