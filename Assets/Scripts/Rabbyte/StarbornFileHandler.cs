using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Rabbyte
{
    public static class StarbornFileHandler
    {
        static readonly string tempDir = Path.Combine(Application.temporaryCachePath, "SBCache");
        static readonly string treeDir = Path.Combine(tempDir, "Current");
        static readonly string resourcesDir = Path.Combine(treeDir, "Resources");
        public static readonly string audioDir = Path.Combine(treeDir, "Audio");
        static readonly string charDir = Path.Combine(treeDir, "Characters");
        static readonly string dialogueDir = Path.Combine(treeDir, "Dialogue");
        static readonly string metadataDir = Path.Combine(treeDir, ".meta");

        static SBCFile lastReadCharacter = new(true);
        static SBDFile lastReadDialogue = new();
        static SimpleSBDFile lastSReadDialogue = new();

        public static void Test()
        {
            //Path.Combine();
            //ZipFile.CreateFromDirectory();
            //File.WriteAllText()
        }

        public static void ClearCache()
        {
            //if (!IsCacheLocked())
            //{
                if (Directory.Exists(treeDir))
                {
                    Directory.Delete(treeDir, true);
                }
            //}
        }

        public static void ClearCharacterCache()
        {
            //if (!IsCacheLocked())
            //{
            if (Directory.Exists(charDir))
            {
                Directory.Delete(charDir, true);
            }
            //}
        }

        public static void WriteCharacter(SBCFile character, string charName = "")
        {
            if (!Directory.Exists(charDir))
                Directory.CreateDirectory(charDir);

            string charPath = Path.Combine(charDir, charName + ".json");
            string charJson = character.Serialize();
            File.WriteAllText(charPath, charJson);
        }

        public static void WriteDialogue(SBDFile dialogue, string dialogueName = "")
        {
            if (!Directory.Exists(dialogueDir))
                Directory.CreateDirectory(dialogueDir);

            string dialoguePath = Path.Combine(dialogueDir, dialogueName + ".json");
            string dialogueJson = dialogue.Serialize();
            File.WriteAllText(dialoguePath, dialogueJson);
        }

        public static void WriteSimpleDialogue(SimpleSBDFile dialogue, string dialogueName = "")
        {
            if (!Directory.Exists(dialogueDir))
                Directory.CreateDirectory(dialogueDir);

            string dialoguePath = Path.Combine(dialogueDir, dialogueName + ".json");
            string dialogueJson = dialogue.Serialize();
            File.WriteAllText(dialoguePath, dialogueJson);
        }

        public static SBCFile ReadCharacter(string filename)
        {
            //string chartName = $"chart{index}";
            string charPath = Path.Combine(charDir, filename + ".json");
            //if (!File.Exists(chartPath))
            //{
               // throw new FileNotFoundException($"Chart {index} not found in RIQ file");
            //}

            string charJson = File.ReadAllText(charPath);
            //Debug.Log($"Jukebox loaded chart {chartPath} ({chartJson.Length} bytes)");

            lastReadCharacter = JsonConvert.DeserializeObject<SBCFile>(charJson);
            lastReadCharacter.removeExpression(); //Because the new SBC will always add a blank character file, we need to remove it from there
            return lastReadCharacter;
        }

        public static SBDFile ReadDialogue(string filename)
        {
            //string chartName = $"chart{index}";
            string dialoguePath = Path.Combine(dialogueDir, filename + ".json");
            //if (!File.Exists(chartPath))
            //{
            // throw new FileNotFoundException($"Chart {index} not found in RIQ file");
            //}

            string dialogueJson = File.ReadAllText(dialoguePath);
            //Debug.Log($"Jukebox loaded chart {chartPath} ({chartJson.Length} bytes)");

            lastReadDialogue = JsonConvert.DeserializeObject<SBDFile>(dialogueJson);
            //lastReadDialogue.removeExpression(); //Because the new SBC will always add a blank character file, we need to remove it from there
            return lastReadDialogue;
        }

        public static SimpleSBDFile ReadSimpleDialogue(string filename)
        {
            //string chartName = $"chart{index}";
            string dialoguePath = Path.Combine(dialogueDir, filename + ".json");
            //if (!File.Exists(chartPath))
            //{
            // throw new FileNotFoundException($"Chart {index} not found in RIQ file");
            //}

            string dialogueJson = File.ReadAllText(dialoguePath);
            //Debug.Log($"Jukebox loaded chart {chartPath} ({chartJson.Length} bytes)");

            lastSReadDialogue = JsonConvert.DeserializeObject<SimpleSBDFile>(dialogueJson);
            lastSReadDialogue.fileName = filename;
            lastSReadDialogue.RemoveLineAtIndex(0); //Because the new SBC will always add a blank character file, we need to remove it from there

            foreach(KeyValuePair<string, List<Emotion>> character in lastSReadDialogue.GetCharacters())
            {
                //Debug.Log(character.Key + ": " + character.Value.Count);
                character.Value.RemoveAt(0);
            }
            //Debug.Log(lastSReadDialogue.GetBackgrounds().Count);
            
            return lastSReadDialogue;
        }

        public static string ExtractCharacter(string path)
        {
            ZipFile.ExtractToDirectory(path, charDir, true);
            return charDir;
        }

        public static string ExtractDialogue(string path)
        {
            ZipFile.ExtractToDirectory(path, dialogueDir, true);
            return dialogueDir;
        }

        public static void PackCharacter(string destPath)
        {
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
            ZipFile.CreateFromDirectory(charDir, destPath, System.IO.Compression.CompressionLevel.Optimal, false);
        }

        public static void PackDialogue(string destPath)
        {
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
            ZipFile.CreateFromDirectory(dialogueDir, destPath, System.IO.Compression.CompressionLevel.Optimal, false);
        }

        public static void CacheAudio(string filename, byte[] bytes, string folder = "")
        {
            if (!Directory.Exists(audioDir))
                Directory.CreateDirectory(audioDir);
            if(folder != "")
            {
                if (!Directory.Exists(Path.Combine(audioDir, folder)))
                    Directory.CreateDirectory(Path.Combine(audioDir, folder));

                File.WriteAllBytes($"{Path.Combine(audioDir, folder, filename)}", bytes);
            }
            else
                File.WriteAllBytes($"{Path.Combine(audioDir,filename)}", bytes);
        }
    }
}