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
      _missingQuery = GetEntityQuery(
        ComponentType.ReadOnly<WorldRenderBounds>(),
        ComponentType.Exclude<ScreenRenderBounds>());
    }

    protected override void OnUpdate() {
      EntityManager.AddComponent(_missingQuery, typeof(ScreenRenderBounds));
      var worldToCameraMatrix = (float4x4)Camera.main.worldToCameraMatrix;
      var projectionMatrix = Camera.main.projectionMatrix;
      var screenSize = new float2(Screen.width, Screen.height);
      Dependency = Entities
        .WithChangeFilter<WorldRenderBounds>()
        .ForEach((ref ScreenRenderBounds sBounds, in WorldRenderBounds wBounds) =>
           sBounds = new ScreenRenderBounds {
             Value = new MinMaxAABB {
               Min = CameraUtility.WorldToScreenPoint(projectionMatrix, worldToCameraMatrix, wBounds.Value.Min, screenSize).xyy,
               Max = CameraUtility.WorldToScreenPoint(projectionMatrix, worldToCameraMatrix, wBounds.Value.Max, screenSize).xyy
             }
           })
        .ScheduleParallel(Dependency);
    }
  }
}
