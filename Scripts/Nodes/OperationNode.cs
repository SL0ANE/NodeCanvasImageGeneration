using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sloane.ImageGraph
{
    public abstract class OperationNode : ImageGraphNode
    {
        public override int maxInConnections => 1;
        public override int maxOutConnections => 1;
        private RenderTexture m_OperatingTexture;
        protected RenderTextureDescriptor m_DefaultTextureDescriptor
        {
            get
            {
                var output = new RenderTextureDescriptor(Width, Height, RenderTextureFormat.ARGBFloat, 0, 1)
                {
                    enableRandomWrite = true
                };

                return output;
            }
        }
        protected RenderTexture OperatingTexture
        {
            get
            {
                UpdateOperationTexture();
                return m_OperatingTexture;
            }
        }
        public override Texture OutputTexture
        {
            get
            {
                if (m_OperatingTexture == null) return DefaultTexture;

                return m_OperatingTexture;
            }
        }

        protected void UpdateOperationTexture()
        {
            if (m_OperatingTexture == null)
            {
                m_OperatingTexture = RenderTexture.GetTemporary(m_DefaultTextureDescriptor);
            }
            else if (m_OperatingTexture.width != Width || m_OperatingTexture.height != Height)
            {
                m_OperatingTexture.Release();
                m_OperatingTexture = RenderTexture.GetTemporary(m_DefaultTextureDescriptor);
            }
        }

        public override void OnDestroy()
        {
            if (m_OperatingTexture != null)
            {
                m_OperatingTexture.Release();
                m_OperatingTexture = null;
            }
            base.OnDestroy();
        }
    }
}
