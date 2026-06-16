using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateCleanFigmaAssets : EditorWindow
{
    [MenuItem("Tools/Generate Clean Figma Assets")]
    public static void Generate()
    {
        string dir = "Assets/Sprites/Figma";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // 1. Grass (Solid #6CA951 with very subtle darker crosses)
        Texture2D grass = new Texture2D(64, 64);
        Color grassBase = new Color(108f/255f, 169f/255f, 81f/255f);
        Color grassDark = new Color(90f/255f, 150f/255f, 65f/255f);
        for(int x=0; x<64; x++) for(int y=0; y<64; y++) grass.SetPixel(x, y, grassBase);
        
        // Draw some subtle crosses
        DrawCross(grass, 16, 16, grassDark);
        DrawCross(grass, 48, 48, grassDark);
        DrawCross(grass, 16, 48, grassDark);
        DrawCross(grass, 48, 16, grassDark);
        grass.Apply();
        SaveTexture(grass, dir + "/CleanGrass.png");

        // 2. Road (Solid #565656) 
        Texture2D road = new Texture2D(8, 8);
        Color roadBase = new Color(86f/255f, 86f/255f, 86f/255f);
        for(int x=0; x<8; x++) for(int y=0; y<8; y++) road.SetPixel(x, y, roadBase);
        road.Apply();
        SaveTexture(road, dir + "/CleanRoad.png");

        // 3. BuildSite (Solid #AAAAAA)
        Texture2D buildSite = new Texture2D(64, 64);
        Color bsBase = new Color(170f/255f, 170f/255f, 170f/255f);
        Color bsBorder = new Color(140f/255f, 140f/255f, 140f/255f);
        for(int x=0; x<64; x++) 
        {
            for(int y=0; y<64; y++) 
            {
                if (x < 2 || x > 61 || y < 2 || y > 61)
                    buildSite.SetPixel(x, y, bsBorder); // slight border
                else
                    buildSite.SetPixel(x, y, bsBase);
            }
        }
        buildSite.Apply();
        SaveTexture(buildSite, dir + "/CleanBuildSite.png");

        // 4. White Dash (Solid White)
        Texture2D dash = new Texture2D(8, 8);
        for(int x=0; x<8; x++) for(int y=0; y<8; y++) dash.SetPixel(x, y, Color.white);
        dash.Apply();
        SaveTexture(dash, dir + "/CleanDash.png");

        AssetDatabase.Refresh();
        
        // Import settings
        ForceSprite(dir + "/CleanGrass.png", 64);
        ForceSprite(dir + "/CleanRoad.png", 8);
        ForceSprite(dir + "/CleanBuildSite.png", 64);
        ForceSprite(dir + "/CleanDash.png", 8);
        
        Debug.Log("✅ Clean Figma Assets Generated!");
    }

    static void DrawCross(Texture2D tex, int cx, int cy, Color c)
    {
        tex.SetPixel(cx, cy, c);
        tex.SetPixel(cx-1, cy, c);
        tex.SetPixel(cx+1, cy, c);
        tex.SetPixel(cx, cy-1, c);
        tex.SetPixel(cx, cy+1, c);
    }

    static void SaveTexture(Texture2D tex, string path)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }

    static void ForceSprite(string path, int ppu)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = ppu;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }
    }
}
