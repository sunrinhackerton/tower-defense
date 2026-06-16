using UnityEngine;
using UnityEditor;
using System.IO;

public class ProcessAIImagesV2 : EditorWindow
{
    [MenuItem("Tools/Process AI Images V2")]
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
            string fullPath = Path.Combine(Application.dataPath, path.Replace("Assets/", ""));
            if (!File.Exists(fullPath)) continue;

            byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);

            Color bgColor = tex.GetPixel(0, tex.height - 1);
            bool modified = false;
            Color[] pixels = tex.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color c = pixels[i];
                // Check if it's the green background (using relative difference or exact match)
                if (Mathf.Abs(c.r - bgColor.r) < 0.15f && 
                    Mathf.Abs(c.g - bgColor.g) < 0.15f && 
                    Mathf.Abs(c.b - bgColor.b) < 0.15f)
                {
                    pixels[i] = new Color(0, 0, 0, 0);
                    modified = true;
                }
                // Also check for pure neon green just in case
                else if (c.g > 0.8f && c.r < 0.3f && c.b < 0.3f)
                {
                    pixels[i] = new Color(0, 0, 0, 0);
                    modified = true;
                }
            }

            if (modified)
            {
                tex.SetPixels(pixels);
                tex.Apply();
                File.WriteAllBytes(fullPath, tex.EncodeToPNG());
                Debug.Log("Removed background directly: " + path);
            }
        }

        AssetDatabase.Refresh();

        // Now set PPU
        string[] allPaths = {
            "Assets/Sprites/AI_Assets/basic_tower_1781625173629.png",
            "Assets/Sprites/AI_Assets/ballista_tower_1781625195365.png",
            "Assets/Sprites/AI_Assets/catapult_tower_1781625185615.png",
            "Assets/Sprites/AI_Assets/enemy_goblin_1781625206931.png",
            "Assets/Sprites/AI_Assets/grass_tile_1781625147606.png",
            "Assets/Sprites/AI_Assets/road_tile_1781625158050.png"
        };

        foreach(string path in allPaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = path.Contains("tile") ? 1000f : 1500f; // Scale down massively
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
                Debug.Log("Set PPU for: " + path);
            }
        }

        Debug.Log("✅ AI Image Processing V2 Complete!");
    }
}
