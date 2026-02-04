// using System.IO;
// using UnityEditor;
// using UnityEngine;

// public class AsmRefGenerator : AssetPostprocessor
// {
//     private const string _asmRefFileName = "UFeel.asmref";
//     private const string _asmRefContent = "{ \"reference\": \"UFeel\" }";

//     private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//     {
//         bool refreshNeeded = false;
//         foreach (string asset in importedAssets)
//         {
//             if (IsValidScript(asset))
//             {
//                 string folder = Path.GetDirectoryName(asset);
//                 string asmRefPath = Path.Combine(folder, _asmRefFileName);

//                 if (!File.Exists(asmRefPath))
//                 {
//                     File.WriteAllText(asmRefPath, _asmRefContent);
//                     refreshNeeded = true;
//                     Debug.Log($"[AsmRefGenerator] Created {_asmRefFileName} in {folder}");
//                 }
//             }
//         }
//         if (refreshNeeded)
//         {
//             AssetDatabase.Refresh();
//         }
//     }

//     private static bool IsValidScript(string assetPath)
//     {
//         if (!assetPath.EndsWith(".cs")) return false;

//         if (assetPath.Contains("/Editor/")) return false;

//         if (assetPath.StartsWith("Packages/")) return false;

//         string directory = Path.GetDirectoryName(assetPath);
//         if (File.Exists(Path.Combine(directory, "UFeel.asmdef"))) return false;

//         return true;
//     }
// }
