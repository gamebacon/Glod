using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Chunk))]
public class ChunkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Chunk chunk = (Chunk)target;
        if (GUILayout.Button("Display Noise Map Texture"))
        {
            Texture2D texture = chunk.GetNoiseMapTexture();
            if (texture != null)
                GUILayout.Label(texture);
        }
    }
}
