using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Class that demonstrates a custom class using sync model attributes.
    /// </summary>
    [SyncDataClass]
    public class SyncSpawnTestSphere : SyncSpawnedObject
    {
        [SyncData]
        public SyncFloat TestFloat;
    }
}

