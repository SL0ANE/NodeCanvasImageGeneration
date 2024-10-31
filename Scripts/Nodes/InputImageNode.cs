using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEditor;
using UnityEngine;

namespace Sloane.ImageGraph
{
    public class InputImageNode : InputNode
    {

        [SerializeField]
        private Texture2D m_InputTexture;
        public override Texture OutputTexture
        {
            get
            {
                if (m_InputTexture == null) return DefaultTexture;

                return m_InputTexture;
            }
        }

        protected override void OnUpdateNode()
        {
            if(m_InputTexture == null) return;
            
            m_Width = m_InputTexture.width;
            m_Height = m_InputTexture.height;
            base.OnUpdateNode();
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            m_InputTexture = EditorGUILayout.ObjectField("Input Texture", m_InputTexture, typeof(Texture), false) as Texture2D;
            if(EditorGUI.EndChangeCheck()) UpdateNode();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
#endif
    }
}
