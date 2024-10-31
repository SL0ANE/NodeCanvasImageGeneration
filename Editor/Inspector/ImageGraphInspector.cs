#if UNITY_EDITOR

using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;
using NodeCanvas.Editor;
using System.IO;


namespace Sloane.ImageGraph
{

    [CustomEditor(typeof(ImageGraph), true)]
    public class ImageGraphInspector : GraphInspector
    {
        private ImageGraph Graph => (ImageGraph)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save Output Texture"))
            {
                Graph.UpdateGraphNodes();
                var outputNode = Graph.GetOutputNode();
                if (outputNode == null)
                {
                    Debug.LogWarning("No Output Node found.");
                    return;
                }
                var outputTexture = outputNode.OutputTexture as RenderTexture;
                if (outputTexture != null)
                {
                    var generatedTexture = new Texture2D(outputTexture.width, outputTexture.height, TextureFormat.RGBA32, false);

                    RenderTexture.active = outputTexture;
                    generatedTexture.ReadPixels(new Rect(0, 0, outputTexture.width, outputTexture.height), 0, 0);
                    RenderTexture.active = null;
                    string path = EditorUtility.SaveFilePanelInProject("Save Texture as PNG", "OutputTexture", "png", "Please enter a filename to save the texture to");
                    if (!string.IsNullOrEmpty(path))
                    {
                        byte[] bytes = generatedTexture.EncodeToPNG();
                        File.WriteAllBytes(path, bytes);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }
}

#endif