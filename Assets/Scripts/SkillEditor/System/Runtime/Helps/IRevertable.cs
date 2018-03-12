// Cinema Suite
using UnityEngine;

namespace TimeLine
{
    /// <summary>
    /// Implement this interface with any timeline item that manipulates data in a scene.
    /// </summary>
    interface IRevertable
    {
        RevertMode EditorRevertMode { get; set; }
        RevertMode RuntimeRevertMode { get; set; }

        RevertInfo[] CacheState();
    }
}
