using UnityEngine;
using UnityEditor;
using System.IO;

public class ProcessAIImagesV3 : EditorWindow
{
    [MenuItem("Tools/Process AI Images V3")]
    public static void Process()
    {
        string[] spritePaths = {
            "Assets/Sprites/AI_Assets/basic_tower_1781625173629.png",
            "Assets/Sprites/AI_Assets/ballista_tower_1781625195365.png",
            "Assets/Sprites/AI_Assets/catapult_tower_1781625185615.png",
            "Assets/Sprites/AI_Assets/enemy_goblin_1781625206931.png"
        };

        foreach(string path in spritePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();

                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    Color bgColor = tex.GetPixel(0, 0); // Bottom Left
                    Color bgColor2 = tex.GetPixel(tex.width - 1, tex.height - 1); // Top Right
                    
                    bool modified = false;
                    Color[] pixels = tex.GetPixels();

                    for (int i = 0; i < pixels.Length; i++)
                    {
                        Color c = pixels[i];
                        // If green is dominant (like a neon green screen)
                        if (c.g > 0.5f && c.g > c.r * 1.5f && c.g > c.b * 1.5f)
                        {
                            pixels[i] = new Color(0, 0, 0, 0);
                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        tex.SetPixels(pixels);
                        tex.Apply();
                        File.WriteAllBytes(Path.Combine(Application.dataPath, path.Replace("Assets/", "")), tex.EncodeToPNG());
                        Debug.Log("V3: Removed green background for " + path);
                    }
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("✅ AI Image Processing V3 Complete!");
    }
}
