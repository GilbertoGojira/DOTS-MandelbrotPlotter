using Mandelbrot.Math;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Mandelbrot {
  public class WorldBoundsToScreenBoundsSystem : SystemBase {
    EntityQuery _missingQuery;

    protected override void OnCreate() {
      base.OnCreate();
      RequireSingletonForUpdate<MainCamera>();
      _missingQuery = GetEntityQuery(
        ComponentType.ReadOnly<WorldRenderBounds>(),
        ComponentType.Exclude<ScreenRenderBounds>());
    }

    protected override void OnUpdate() {
      EntityManager.AddComponent(_missingQuery, typeof(ScreenRenderBounds));
      var camera = EntityManager.GetComponentObject<Camera>(GetSingletonEntity<MainCamera>());
      var worldToCameraMatrix = (float4x4)camera.worldToCameraMatrix;
      var projectionMatrix = camera.projectionMatrix;
      var screenSize = new float2(Screen.width, Screen.height);
      Dependency = Entities
        .WithChangeFilter<WorldRenderBounds>()
        .ForEach((ref ScreenRenderBounds sBounds, in WorldRenderBounds wBounds) =>
           sBounds = new ScreenRenderBounds {
             Value = new MinMaxAABB {
               Min = new float3(CameraUtility.WorldToScreenPoint(projectionMatrix, worldToCameraMatrix, wBounds.Value.Min, screenSize), 0),
               Max = new float3(CameraUtility.WorldToScreenPoint(projectionMatrix, worldToCameraMatrix, wBounds.Value.Max, screenSize), 0)
             }
           })
        .ScheduleParallel(Dependency);
    }
  }
}
