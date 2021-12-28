using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Mandelbrot.Jobs {
  [BurstCompile]
  public struct ComponentCopyJob<TCOPY, TCOMP> : IJobChunk
    where TCOMP : struct, IComponentData
    where TCOPY : struct, IComponentDataCopy<TCOMP> {
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly]
    public EntityTypeHandle EntityHandle;
    [ReadOnly]
    public ComponentTypeHandle<TCOMP> ComponentTypeHandle;

    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
      var entities = chunk.GetNativeArray(EntityHandle);
      var comps = chunk.GetNativeArray(ComponentTypeHandle);
      for (var i = 0; i < entities.Length; ++i) {
        TCOPY copy = default;
        copy.Value = comps[i];
        ECB.AddComponent(firstEntityIndex + i, entities[i], copy);
      }
    }
  }

  [BurstCompile]
  public struct RestoreComponentDataCopy<TCOPY, TCOMP> : IJobChunk
    where TCOMP : struct, IComponentData
    where TCOPY : struct, IComponentDataCopy<TCOMP> {
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly]
    public EntityTypeHandle EntityHandle;
    [ReadOnly]
    public ComponentTypeHandle<TCOPY> ComponentCopyTypeHandle;
    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
      var entities = chunk.GetNativeArray(EntityHandle);
      var copies = chunk.GetNativeArray(ComponentCopyTypeHandle);
      for (var i = 0; i < entities.Length; ++i)
        ECB.SetComponent(firstEntityIndex + i, entities[i], copies[i].Value);
    }
  }
}