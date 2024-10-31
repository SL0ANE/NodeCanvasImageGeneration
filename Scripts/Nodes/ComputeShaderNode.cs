using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sloane.ImageGraph
{
    public class ComputeShaderNode : IterableNode
    {
        public override int maxInConnections => m_PropertyNames.Count;
        public override int maxOutConnections => -1;

        [SerializeField]
        private ComputeShader m_ComputeShader;
        [SerializeField]
        private string m_KernelName = "CSMain";
        [SerializeField, AutoSortWithParentsConnections]
        private List<string> m_PropertyNames = new List<string>();
        private int m_KernelIndex;
        private bool m_HasSourceBuffer;

        public override void OnCreate(Graph assignedGraph)
        {
            m_PropertyNames.Add("_CurrentBuffer");
            base.OnCreate(assignedGraph);
        }

        protected override void OnIterateNode()
        {
            if(!m_HasSourceBuffer || m_ComputeShader == null) return;

            m_ComputeShader.SetInt(ShaderPropertyStorage.Iteration, m_CurrentIteration);
            m_ComputeShader.Dispatch(m_KernelIndex, Width / 8, Height / 8, 1);
        }

        protected override void BeforeIterateNode()
        {
            if (m_ComputeShader == null) return;

            m_KernelIndex = m_ComputeShader.FindKernel(m_KernelName);
            m_HasSourceBuffer = false;

            for (int i = 0; i < m_PropertyNames.Count; i++)
            {
                if (inConnections.Count <= i) break;

                var connection = inConnections[i];
                var node = connection.sourceNode as ImageGraphNode;
                if (node == null) continue;

                if(m_PropertyNames[i] == "_CurrentBuffer")
                {
                    m_Width = node.OutputTexture.width;
                    m_Height = node.OutputTexture.height;
                    m_HasSourceBuffer = true;
                    
                    Graphics.Blit(node.OutputTexture, OperatingTexture);
                }
                else m_ComputeShader.SetTexture(m_KernelIndex, m_PropertyNames[i], node.OutputTexture);
            }

            if(!m_HasSourceBuffer) return;

            m_ComputeShader.SetInt(ShaderPropertyStorage.Width, Width);
            m_ComputeShader.SetInt(ShaderPropertyStorage.Height, Height);
            m_ComputeShader.SetInt(ShaderPropertyStorage.IterationTime, m_IterationsTime);

            m_ComputeShader.SetTexture(m_KernelIndex, ShaderPropertyStorage.CurrentBuffer, OperatingTexture);
            m_ComputeShader.SetTexture(m_KernelIndex, ShaderPropertyStorage.PreviousBuffer, PrevOperatingTexture);
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            m_ComputeShader = (ComputeShader)EditorGUILayout.ObjectField("Compute Shader", m_ComputeShader, typeof(ComputeShader), false);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            m_KernelName = EditorGUILayout.TextField("Kernel Name", m_KernelName);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            int length = EditorGUILayout.IntField("Property Count", m_PropertyNames.Count);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (length != m_PropertyNames.Count)
            {
                if (length < 0)
                {
                    length = 0;
                }

                if (length > m_PropertyNames.Count)
                {
                    for (int i = m_PropertyNames.Count; i < length; i++)
                    {
                        m_PropertyNames.Add(string.Empty);
                    }
                }
                else
                {
                    m_PropertyNames.RemoveRange(length, m_PropertyNames.Count - length);
                }
            }

            for (int i = 0; i < m_PropertyNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);

                m_PropertyNames[i] = EditorGUILayout.TextField("", m_PropertyNames[i]);
                GUILayout.Space(40);
                if (inConnections.Count <= i)
                {
                    EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(20, 20), DefaultTexture, null, ScaleMode.ScaleToFit);
                }
                else
                {
                    EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(20, 20), ((ImageGraphNode)inConnections[i].sourceNode).PreviewTexture, null, ScaleMode.ScaleToFit);
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            if (EditorGUI.EndChangeCheck()) UpdateNode();
        }
#endif
    }
}