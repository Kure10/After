using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ResourceSpriteLoader))]
public class ResourceLoaderEditor : Editor
{
    string filePath = "Assets/Scripts/TESTING_EVENTS/Resources";
    ResourceSpriteLoader resourceSpriteLoader;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        this.resourceSpriteLoader = (ResourceSpriteLoader)target;

        EditorGUILayout.Space(15f);
        if (GUILayout.Button("Re-Generate"))
        {
            Regenerate();
        }

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Clear"))
        {
            this.resourceSpriteLoader.ClearEventResources();
        }

        EditorGUILayout.Space(8);
        if (GUILayout.Button("SetDefaultResource"))
        {
            SetDefault();
        }

    }

    private void SetDefault()
    {
        string defaultFilePath = filePath + "/" + "default";
        DirectoryInfo info = new DirectoryInfo(defaultFilePath);
        FileInfo[] fileInfo = info.GetFiles();
        string defaultSpritePath = defaultFilePath +"/" + fileInfo[0].Name;

        Sprite newDefaultSprite = LoadNewSprite(defaultSpritePath);
        newDefaultSprite.name = fileInfo[0].Name;
        this.resourceSpriteLoader.SetDefault = newDefaultSprite;
    }

    private void Regenerate()
    {
        this.resourceSpriteLoader.ClearEventResources();

        // otevrit soubor a nahrat ostatni 

        DirectoryInfo info = new DirectoryInfo(filePath);
        FileInfo[] fileInfo = info.GetFiles();
        foreach (FileInfo item in fileInfo)
        {
            string currentFilePath = filePath + "/" + item.Name;

            if (currentFilePath.EndsWith("meta"))
                continue;

            Sprite newSprite = LoadNewSprite(currentFilePath);
            newSprite.name = item.Name;

            this.resourceSpriteLoader.AddEventResource(newSprite);
        }

    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite;
        Texture2D SpriteTexture = LoadTexture(FilePath);
        NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }
}
