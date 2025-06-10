

using System;
using System.Runtime;

#if SCENEEDITOR
    using var game = new SolidSilnique.ForgeSceneEditor();
    game.Run();
#else
GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

// Configure Large Object Heap compaction mode
GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

// Set the memory limit higher (in bytes)
// 3GB = 3 * 1024 * 1024 * 1024 = 3221225472 bytes


// Optional: Reserve a large chunk of memory to prevent frequent collections
var memoryPressure = 5368709120L; // 3GB
GC.AddMemoryPressure(memoryPressure);
    using var game = new SolidSilnique.Game1();
    game.Run();
#endif