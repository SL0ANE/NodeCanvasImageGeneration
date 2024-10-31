using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;

namespace Sloane.ImageGraph
{
    [CreateAssetMenu(menuName = "Sloane/Image Graph Asset")]
    public class ImageGraph : Graph
    {
        public override Type baseNodeType => typeof(ImageGraphNode);

        public override bool requiresAgent => true;

        public override bool requiresPrimeNode => false;

        public override bool isTree => true;

        public override PlanarDirection flowDirection => PlanarDirection.Horizontal;

        public override bool allowBlackboardOverrides => true;

        public override bool canAcceptVariableDrops => true;
        [SerializeField]
        private int m_OutputNodeID = -1;
        private OutputNode m_OutputNode;
        public OutputNode OutputNode
        {
            get => m_OutputNode; set
            {
                if (m_OutputNode != value && m_OutputNode != null)  Debug.LogWarning("Multiple Output Nodes are not supported.");

                m_OutputNodeID = value.ID;
                m_OutputNode = value;
            }
        }

        protected override void OnGraphValidate()
        {
            m_OutputNode = GetOutputNode();
            base.OnGraphValidate();
        }

        public OutputNode GetOutputNode()
        {
            OutputNode outputNode = null;
            foreach (var node in allNodes)
            {
                if (node.ID == m_OutputNodeID)
                {
                    outputNode = node as OutputNode;
                    break;
                }
            }

            return outputNode;
        }

        public void UpdateGraphNodes()
        {
            foreach (var node in allNodes)
            {
                if (node is InputNode)
                {
                    ((InputNode)node).UpdateNode();
                    break;
                }
            }
        }
    }
}
