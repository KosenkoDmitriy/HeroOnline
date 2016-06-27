using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AnimClip_Importer
{
    static readonly string[] LoopClips = new string[] { "Alert", "Leisure", "Stand", "Walk", "Run" };

    [MenuItem("Assets/Import Animation Clips")]
    public static void ImportClips()
    {
        string selectedDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
        Debug.Log("Importing all clips in folder: " + selectedDir);

        string[] textFiles = Directory.GetFiles(selectedDir, "*.txt");

        foreach (string txtFileName in textFiles)
        {
            var assetObj = AssetDatabase.LoadAssetAtPath(txtFileName, typeof(TextAsset));
            TextAsset textAsset = assetObj as TextAsset;
            if (textAsset == null)
                continue;

            string modelFileName = Path.ChangeExtension(txtFileName, "fbx");
            ModelImporter modelImp = AssetImporter.GetAtPath(modelFileName) as ModelImporter;

            if (modelImp == null)
			{
				modelFileName = Path.ChangeExtension(txtFileName, "dae");
				modelImp = AssetImporter.GetAtPath(modelFileName) as ModelImporter;

				if (modelImp == null)
					continue;
			}

            List<ModelImporterClipAnimation> allClips = new List<ModelImporterClipAnimation>();

            string[] allLines = textAsset.text.Split('\n');
            foreach (string line in allLines)
            {
                string[] allFields = line.Split('\t');
                if (allFields.Length == 3)
                {
                    ModelImporterClipAnimation clip = new ModelImporterClipAnimation();

                    clip.name = allFields[0];
                    clip.firstFrame = int.Parse(allFields[1]);
                    clip.lastFrame = int.Parse(allFields[2]);

                    foreach (string name in LoopClips)
                    {
                        if (clip.name.StartsWith(name))
                            clip.wrapMode = WrapMode.Loop;
                    }

                    allClips.Add(clip);
                }
            }

            modelImp.animationType = ModelImporterAnimationType.Legacy;
            modelImp.clipAnimations = allClips.ToArray();

            AssetDatabase.ImportAsset(modelFileName);
            AssetDatabase.DeleteAsset(txtFileName);

            Debug.Log(string.Format("{0} - Animation clips imported!", Path.GetFileName(txtFileName)));
        }
    }
}
