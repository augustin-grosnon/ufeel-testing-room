using System.IO;
using UnityEditor;
using UnityEngine;

public class AsmRefGenerator : AssetPostprocessor
{
    private const string AsmRefFileName = "UFeel.asmref";
    private const string AsmRefContent = "{ \"reference\": \"UFeel\" }";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (IsValidScript(asset))
            {
                string folder = Path.GetDirectoryName(asset);
                string asmRefPath = Path.Combine(folder, AsmRefFileName);

                if (!File.Exists(asmRefPath))
                {
                    File.WriteAllText(asmRefPath, AsmRefContent);
                    AssetDatabase.Refresh();
                    Debug.Log($"[AsmRefGenerator] Created {AsmRefFileName} in {folder}");
                }
            }
        }
    }

    private static bool IsValidScript(string assetPath)
    {
        if (!assetPath.EndsWith(".cs")) return false;

        if (assetPath.Contains("/Editor/")) return false;

        if (assetPath.StartsWith("Packages/")) return false;

        string directory = Path.GetDirectoryName(assetPath);
        if (File.Exists(Path.Combine(directory, "UFeel.asmdef"))) return false;

        return true;
    }
}
