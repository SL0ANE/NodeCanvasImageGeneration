using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sloane.ImageGraph
{
    public static class ShaderPropertyStorage
    {
        public static readonly int Width = Shader.PropertyToID("_Width");
        public static readonly int Height = Shader.PropertyToID("_Height");
        public static readonly int CurrentBuffer = Shader.PropertyToID("_CurrentBuffer");
        public static readonly int PreviousBuffer = Shader.PropertyToID("_PreviousBuffer");
        public static readonly int Iteration = Shader.PropertyToID("_Iteration");
        public static readonly int IterationTime = Shader.PropertyToID("_IterationTime");
        public static readonly int ZThreadCount = Shader.PropertyToID("ZThreadCount");
    }
}