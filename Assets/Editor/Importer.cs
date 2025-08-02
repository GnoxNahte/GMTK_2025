using UnityEditor;
using UnityEngine;

// Source: https://discussions.unity.com/t/simple-guide-to-change-default-sprite-import-settings-filtermode-compression-ppu-via-script/834274
//Throw this inside YourGame/Assets/Editor/Importer.cs
public class Importer: AssetPostprocessor {

    const int PostProcessOrder = 0;
    public override int GetPostprocessOrder () => PostProcessOrder;

    void OnPreprocessTexture () {
        var textureImporter = assetImporter as TextureImporter;
        
        if (!textureImporter || !textureImporter.importSettingsMissing)
            return;

        textureImporter.filterMode = FilterMode.Point;
        //textureImporter.filterMode = FilterMode.Bilinear;
        //textureImporter.filterMode = FilterMode.Trilinear;

        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        //textureImporter.textureCompression = TextureImporterCompression.Compressed;
        //textureImporter.textureCompression = TextureImporterCompression.CompressedLQ;
        //textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
    
        //textureImporter.spritePixelsPerUnit = 16;
        textureImporter.spritePixelsPerUnit = 32;
        // textureImporter.spritePixelsPerUnit = 64;
        // textureImporter.spritePixelsPerUnit = 100;
        
        // textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
    }
}