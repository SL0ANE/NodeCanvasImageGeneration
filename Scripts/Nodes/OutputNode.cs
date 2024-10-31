using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;

namespace Sloane.ImageGraph
{
    public class OutputNode : OperationNode
    {
        public override int maxOutConnections => 0;

        protected override void OnUpdateNode()
        {
            var InputNode = inConnections[0].sourceNode as ImageGraphNode;
            m_Width = InputNode.Width;
            m_Height = InputNode.Height;
            Graphics.Blit(InputNode.OutputTexture, OperatingTexture);
        }

        public override void OnParentConnected(int connectionIndex)
        {
            ((ImageGraph)graph).OutputNode = this;

            base.OnParentConnected(connectionIndex);
        }
    }
}
