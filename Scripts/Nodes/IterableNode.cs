using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Sloane.ImageGraph
{
    public abstract class IterableNode : OperationNode
    {

        [SerializeField]
        protected int m_IterationsTime = 1;
        protected int m_CurrentIteration;
        private RenderTexture m_PrevOperatingTexture;
        protected RenderTexture PrevOperatingTexture
        {
            get
            {
                UpdatePrevOperatingTexture();
                return m_PrevOperatingTexture;
            }
        }
        protected sealed override void OnUpdateNode()
        {
            BeforeIterateNode();

            if(m_Width == 0 || m_Height == 0)
            {
                return;
            }

            m_CurrentIteration = 0;

            for (int i = 0; i < m_IterationsTime; i++)
            {
                Graphics.Blit(OperatingTexture, m_PrevOperatingTexture);
                OnIterateNode();

                m_CurrentIteration++;
            }
            RenderTexture.ReleaseTemporary(m_PrevOperatingTexture);
            m_PrevOperatingTexture = null;
        }

        protected void UpdatePrevOperatingTexture()
        {
            if (m_PrevOperatingTexture == null)
            {
                m_PrevOperatingTexture = RenderTexture.GetTemporary(m_DefaultTextureDescriptor);
            }
            else if (m_PrevOperatingTexture.width != Width || m_PrevOperatingTexture.height != Height)
            {
                m_PrevOperatingTexture.Release();
                m_PrevOperatingTexture = RenderTexture.GetTemporary(m_DefaultTextureDescriptor);
            }
        }

        public override void OnDestroy()
        {
            if (m_PrevOperatingTexture != null)
            {
                m_PrevOperatingTexture.Release();
                m_PrevOperatingTexture = null;
            }

            base.OnDestroy();
        }

        protected abstract void OnIterateNode();
        protected abstract void BeforeIterateNode();

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            m_IterationsTime = EditorGUILayout.IntField("Iterations Time", m_IterationsTime);
            if (EditorGUI.EndChangeCheck()) UpdateNode();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
#endif
    }
}
