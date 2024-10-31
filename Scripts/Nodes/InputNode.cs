using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEditor;
using UnityEngine;

namespace Sloane.ImageGraph
{
    public abstract class InputNode : ImageGraphNode
    {
        public override int maxInConnections => 0;
        public override int maxOutConnections => -1;
        public override void OnValidate(Graph assignedGraph)
        {
            base.OnValidate(assignedGraph);
            UpdateNode();
        }
    }
}
