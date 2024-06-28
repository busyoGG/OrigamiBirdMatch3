using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResourceUtils
{
    public class ResInfo
    {
        public string name = String.Empty;
        
        public int reference = 0;

        public AsyncOperationHandle handle;
    }
}