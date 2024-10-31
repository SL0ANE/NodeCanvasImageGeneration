using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;

namespace Sloane.ImageGraph
{
    public abstract class ImageGraphNode : Node
    {
        [System.AttributeUsage(System.AttributeTargets.Field)]
        protected class AutoSortWithParentsConnections : System.Attribute { }
        sealed public override System.Type outConnectionType { get { return typeof(ImageGraphConnection); } }
        sealed public override bool allowAsPrime { get { return false; } }
        sealed public override bool canSelfConnect { get { return false; } }
        public override Alignment2x2 commentsAlignment { get { return Alignment2x2.Bottom; } }
        public override Alignment2x2 iconAlignment { get { return Alignment2x2.Default; } }
        public override int maxInConnections { get { return 1; } }
        public override int maxOutConnections { get { return 0; } }
        public virtual Texture OutputTexture { get; }
        public virtual Texture PreviewTexture
        {
            get
            {
                return OutputTexture;
            }
        }
        protected int m_UpdateRef;
        protected int m_Width;
        protected int m_Height;
        public int Width => m_Width;
        public int Height => m_Height;
        private static RenderTexture m_DefaultTexture;
        protected static RenderTexture DefaultTexture
        {
            get
            {
                if (m_DefaultTexture == null)
                {
                    var currentActive = RenderTexture.active;
                    var defaultTextureDescriptor = new RenderTextureDescriptor(128, 128, RenderTextureFormat.ARGB32, 0);

                    m_DefaultTexture = new RenderTexture(defaultTextureDescriptor);
                    RenderTexture.active = m_DefaultTexture;
                    GL.Clear(true, true, Color.magenta);

                    RenderTexture.active = currentActive;
                }

                return m_DefaultTexture;
            }
        }

        protected virtual void OnUpdateNode()
        {

        }

        public void UpdateNode()
        {
            if (m_UpdateRef > 0)
            {
                Debug.LogWarning("UpdateGraph is called recursively. Aborting.");
                return;
            }

            m_UpdateRef += 1;

            try
            {
                OnUpdateNode();

                foreach (var connection in outConnections)
                {
                    var targetNode = connection.targetNode as ImageGraphNode;
                    targetNode.UpdateNode();
                }

                m_UpdateRef = 0;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                m_UpdateRef = 0;
            }
        }

        public override void OnParentConnected(int connectionIndex)
        {
            UpdateNode();
            base.OnParentConnected(connectionIndex);
        }

        public override void OnParentDisconnected(int connectionIndex)
        {
            UpdateNode();
            base.OnParentDisconnected(connectionIndex);
        }

        public virtual void OnParentConnectionsSortChanged()
        {

        }

        public void TrySortParentConnectionsByRelativePosition()
        {
            var original = inConnections.ToList();
            if (graph.flowDirection == PlanarDirection.Horizontal)
            {
                inConnections = inConnections.OrderBy(c => c.sourceNode.rect.center.y).ToList();
            }
            if (graph.flowDirection == PlanarDirection.Vertical)
            {
                inConnections = inConnections.OrderBy(c => c.sourceNode.rect.center.x).ToList();
            }
            var oldIndeces = inConnections.Select(x => original.IndexOf(x)).ToArray();

            foreach (var field in GetType().RTGetFields())
            {
                if (field.RTIsDefined<AutoSortWithParentsConnections>(true))
                {
                    var list = field.GetValue(this) as IList;
                    if (list != null)
                    {
                        var temp = new object[list.Count];
                        for (var i = 0; i < list.Count; i++) { temp[i] = list[i]; }
                        for (var i = 0; i < oldIndeces.Length; i++) { list[i] = temp[oldIndeces[i]]; }
                    }
                }
            }

            OnParentConnectionsSortChanged();
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnNodeGUI()
        {
            base.OnNodeGUI();

            GUILayout.BeginHorizontal();

            UnityEditor.EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(128, 128), PreviewTexture, null, ScaleMode.ScaleToFit);

            GUILayout.EndHorizontal();
        }

        protected override void OnNodeReleased()
        {
            foreach (var connection in outConnections)
            {
                if(connection.targetNode != null)
                {
                    ((ImageGraphNode)connection.targetNode).TrySortParentConnectionsByRelativePosition();
                }
            }
            base.OnNodeReleased();
        }
#endif
    }
}
