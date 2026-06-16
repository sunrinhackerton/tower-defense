using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetGenerator : EditorWindow
{
    private static string savePath = "Assets/Sprites/";

    [MenuItem("Tools/Generate Flight Plan 7.0 Assets")]
    public static void GenerateAllAssets()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        GenerateGrassBackground();
        GenerateRoadTile();
        GenerateTowerBaseBasic();
        GenerateTowerHeadBasic();
        GenerateTowerBaseCatapult();
        GenerateTowerHeadCatapult();
        GenerateTowerBaseBallista();
        GenerateTowerHeadBallista();
        GenerateUIAssets();
        GenerateCoinIcon();

        AssetDatabase.Refresh();
        Debug.Log("🎉 Flight Plan 7.0 에셋 생성 완료! (경로: " + savePath + ")");
    }

    private static void SaveTexture(Texture2D tex, string fileName)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(savePath, fileName + ".png"), bytes);
    }

    private static void Fill(Texture2D tex, Color color)
    {
        Color[] pixels = new Color[tex.width * tex.height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
    }

    private static void DrawRect(Texture2D tex, int x, int y, int w, int h, Color color)
    {
        for (int py = y; py < y + h; py++)
        {
            for (int px = x; px < x + w; px++)
            {
                if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(px, py, color);
            }
        }
    }

    private static void DrawCircle(Texture2D tex, int cx, int cy, int radius, Color color)
    {
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }
    }

    private static void DrawRoundedRect(Texture2D tex, int cornerRadius, Color color)
    {
        Color clear = new Color(0, 0, 0, 0);
        Fill(tex, clear);

        int w = tex.width;
        int h = tex.height;
        int r = cornerRadius;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool isCorner = false;
                int cx = 0, cy = 0;

                if (x < r && y < r) { isCorner = true; cx = r; cy = r; }
                else if (x >= w - r && y < r) { isCorner = true; cx = w - r - 1; cy = r; }
                else if (x < r && y >= h - r) { isCorner = true; cx = r; cy = h - r - 1; }
                else if (x >= w - r && y >= h - r) { isCorner = true; cx = w - r - 1; cy = h - r - 1; }

                if (isCorner)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    if (dx * dx + dy * dy <= r * r)
                        tex.SetPixel(x, y, color);
                }
                else
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }
    }

    private static void GenerateGrassBackground()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color grassColor = ColorUtility.TryParseHtmlString("#6AA84F", out Color gc) ? gc : Color.green;
        Fill(tex, grassColor);
        // 무작위 십자가 장식 (img3.png 참고)
        Color[] decoColors = { Color.white, Color.yellow, new Color(1f, 0.5f, 0f), new Color(1f, 0.5f, 0.8f) };
        for (int i = 0; i < 5; i++)
        {
            int rx = Random.Range(5, 59);
            int ry = Random.Range(5, 59);
            Color c = decoColors[Random.Range(0, decoColors.Length)];
            DrawRect(tex, rx - 1, ry, 3, 1, c);
            DrawRect(tex, rx, ry - 1, 1, 3, c);
        }
        tex.Apply();
        SaveTexture(tex, "GrassBackground");
    }

    private static void GenerateRoadTile()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color roadColor = ColorUtility.TryParseHtmlString("#434343", out Color rc) ? rc : Color.gray;
        Fill(tex, roadColor);

        // 가장자리 흰색 점선 (img3.png 참고)
        for (int y = 0; y < 64; y += 8)
        {
            DrawRect(tex, 4, y, 2, 4, Color.white); // 왼쪽 점선
            DrawRect(tex, 64 - 6, y, 2, 4, Color.white); // 오른쪽 점선
        }
        tex.Apply();
        SaveTexture(tex, "RoadTile");
    }

    private static void GenerateTowerBaseBasic()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color baseColor = ColorUtility.TryParseHtmlString("#888888", out Color c) ? c : Color.gray;
        Fill(tex, baseColor);

        // 모서리 작은 나사 X자
        Color screwColor = ColorUtility.TryParseHtmlString("#333333", out Color sc) ? sc : Color.black;
        DrawRect(tex, 6, 6, 4, 4, screwColor);
        DrawRect(tex, 64 - 10, 6, 4, 4, screwColor);
        DrawRect(tex, 6, 64 - 10, 4, 4, screwColor);
        DrawRect(tex, 64 - 10, 64 - 10, 4, 4, screwColor);
        tex.Apply();
        SaveTexture(tex, "TowerBase_Basic");
    }

    private static void GenerateTowerHeadBasic()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color clear = new Color(0, 0, 0, 0);
        Fill(tex, clear);

        Color headColor = ColorUtility.TryParseHtmlString("#2B2B2B", out Color c) ? c : Color.black;
        DrawCircle(tex, 32, 26, 20, headColor);
        // 포신 (위쪽으로 향함)
        DrawRect(tex, 26, 42, 12, 18, headColor);
        DrawRect(tex, 24, 56, 16, 4, Color.black); // 포신 끝 마감
        
        // 빛 반사 하이라이트
        DrawCircle(tex, 38, 20, 5, new Color(1, 1, 1, 0.2f));

        tex.Apply();
        SaveTexture(tex, "TowerHead_Basic");
    }

    private static void GenerateTowerBaseCatapult()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color baseColor = ColorUtility.TryParseHtmlString("#888888", out Color c) ? c : Color.gray;
        Fill(tex, baseColor);

        Color screwColor = ColorUtility.TryParseHtmlString("#333333", out Color sc) ? sc : Color.black;
        DrawCircle(tex, 10, 32, 4, screwColor);
        DrawCircle(tex, 64 - 10, 32, 4, screwColor);
        DrawCircle(tex, 32, 10, 4, screwColor);
        DrawCircle(tex, 32, 64 - 10, 4, screwColor);
        tex.Apply();
        SaveTexture(tex, "TowerBase_Catapult");
    }

    private static void GenerateTowerHeadCatapult()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color clear = new Color(0, 0, 0, 0);
        Fill(tex, clear);

        Color headColor = ColorUtility.TryParseHtmlString("#3D3D3D", out Color c) ? c : Color.black;
        Color detailColor = ColorUtility.TryParseHtmlString("#666666", out Color dc) ? dc : Color.gray;

        // 중앙 십자 코어
        DrawRect(tex, 22, 10, 20, 44, headColor);
        DrawRect(tex, 10, 22, 44, 20, headColor);

        // 기어 이빨 느낌의 대각선 파츠 (간이 구현)
        DrawRect(tex, 16, 16, 10, 10, detailColor);
        DrawRect(tex, 64 - 26, 16, 10, 10, detailColor);
        DrawRect(tex, 16, 64 - 26, 10, 10, detailColor);
        DrawRect(tex, 64 - 26, 64 - 26, 10, 10, detailColor);

        // 중앙 코어
        DrawCircle(tex, 32, 32, 8, Color.black);
        DrawRect(tex, 30, 30, 4, 4, detailColor); // 나사 홈

        tex.Apply();
        SaveTexture(tex, "TowerHead_Catapult");
    }

    private static void GenerateTowerBaseBallista()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color baseColor = ColorUtility.TryParseHtmlString("#4A6076", out Color c) ? c : Color.blue;
        Fill(tex, baseColor);

        Color neonColor = ColorUtility.TryParseHtmlString("#FF00FF", out Color nc) ? nc : Color.magenta;
        // 왼쪽 네온 무늬
        DrawRect(tex, 4, 0, 4, 64, neonColor);
        DrawRect(tex, 4, 32, 12, 4, neonColor);
        DrawRect(tex, 4, 16, 12, 4, neonColor);
        DrawRect(tex, 4, 48, 12, 4, neonColor);
        
        // 우측 네온 무늬
        DrawRect(tex, 64 - 8, 0, 4, 64, neonColor);

        tex.Apply();
        SaveTexture(tex, "TowerBase_Ballista");
    }

    private static void GenerateTowerHeadBallista()
    {
        Texture2D tex = new Texture2D(64, 64);
        Color clear = new Color(0, 0, 0, 0);
        Fill(tex, clear);

        Color headColor = ColorUtility.TryParseHtmlString("#2B3A4A", out Color c) ? c : Color.black;
        Color neonColor = ColorUtility.TryParseHtmlString("#FF00FF", out Color nc) ? nc : Color.magenta;

        // 발리스타 몸체 (삼각형 끝)
        for (int y = 8; y < 56; y++)
        {
            int w = y < 20 ? y - 8 : (y > 40 ? 56 - y : 16);
            DrawRect(tex, 32 - w, y, w * 2, 1, headColor);
        }

        // 네온 라인
        DrawRect(tex, 28, 16, 2, 36, neonColor);
        DrawRect(tex, 34, 16, 2, 36, neonColor);

        tex.Apply();
        SaveTexture(tex, "TowerHead_Ballista");
    }

    private static void GenerateUIAssets()
    {
        // 검은 모서리 둥근 패널
        Texture2D panelTex = new Texture2D(128, 128);
        DrawRoundedRect(panelTex, 16, new Color(0.1f, 0.1f, 0.1f, 0.95f));
        panelTex.Apply();
        SaveTexture(panelTex, "UIPanelBackground");

        // UI 버튼 초록
        Texture2D btnGreen = new Texture2D(128, 64);
        Color green = ColorUtility.TryParseHtmlString("#2E9F64", out Color gc) ? gc : Color.green;
        DrawRoundedRect(btnGreen, 8, green);
        btnGreen.Apply();
        SaveTexture(btnGreen, "ButtonGreen");

        // UI 버튼 빨강
        Texture2D btnRed = new Texture2D(128, 64);
        Color red = ColorUtility.TryParseHtmlString("#FF0000", out Color rc) ? rc : Color.red;
        DrawRoundedRect(btnRed, 8, red);
        btnRed.Apply();
        SaveTexture(btnRed, "ButtonRed");
        
        // UI 버튼 회색
        Texture2D btnGrey = new Texture2D(128, 64);
        Color grey = ColorUtility.TryParseHtmlString("#555555", out Color g) ? g : Color.gray;
        DrawRoundedRect(btnGrey, 8, grey);
        btnGrey.Apply();
        SaveTexture(btnGrey, "ButtonGrey");
    }

    private static void GenerateCoinIcon()
    {
        Texture2D tex = new Texture2D(32, 32);
        Color clear = new Color(0, 0, 0, 0);
        Fill(tex, clear);

        Color gold = ColorUtility.TryParseHtmlString("#F1C232", out Color c) ? c : Color.yellow;
        Color goldDark = ColorUtility.TryParseHtmlString("#B45F06", out Color dc) ? dc : Color.red;

        DrawCircle(tex, 16, 16, 14, goldDark);
        DrawCircle(tex, 16, 16, 12, gold);
        
        // 동전 'C' 마크
        for(int y=10; y<22; y++)
        {
            for(int x=10; x<22; x++)
            {
                if ((x==10 || x==11) && y>12 && y<20) tex.SetPixel(x, y, goldDark);
                if ((y==10 || y==11 || y==20 || y==21) && x>12 && x<20) tex.SetPixel(x, y, goldDark);
            }
        }

        tex.Apply();
        SaveTexture(tex, "CoinIcon");
    }
}
