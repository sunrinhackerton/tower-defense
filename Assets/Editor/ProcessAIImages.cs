using UnityEngine;
using UnityEditor;

public class ProcessAIImages : EditorWindow
{
    [MenuItem("Tools/Process AI Images (Scale & Transparent)")]
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
                // Make readable and adjust PPU
                importer.isReadable = true;
                importer.spritePixelsPerUnit = 800f; // Scale down
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();

                // Process transparent background
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    Color bgColor = tex.GetPixel(0, tex.height - 1); // Top-left pixel
                    float tolerance = 0.1f;
                    bool modified = false;

                    Color[] pixels = tex.GetPixels();
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        if (Mathf.Abs(pixels[i].r - bgColor.r) < tolerance &&
                            Mathf.Abs(pixels[i].g - bgColor.g) < tolerance &&
                            Mathf.Abs(pixels[i].b - bgColor.b) < tolerance)
                        {
                            pixels[i] = new Color(0, 0, 0, 0); // Transparent
                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        tex.SetPixels(pixels);
                        tex.Apply();
                        byte[] bytes = tex.EncodeToPNG();
                        System.IO.File.WriteAllBytes(Application.dataPath + path.Replace("Assets", ""), bytes);
                        Debug.Log("Removed background for: " + path);
                    }
                }
                
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SaveAndReimport();
            }
        }

        // Process Grass/Road Tiles (Just Scale down PPU, don't remove background)
        string[] mapTiles = {
            "Assets/Sprites/AI_Assets/grass_tile_1781625147606.png",
            "Assets/Sprites/AI_Assets/road_tile_1781625158050.png"
        };

        foreach(string path in mapTiles)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.spritePixelsPerUnit = 250f; // Map tiles need to be slightly larger
                importer.SaveAndReimport();
            }
        }

        Debug.Log("AI Image Processing Complete!");
    }
}
