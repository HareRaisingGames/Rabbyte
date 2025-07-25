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
        static readonly string audioDir = Path.Combine(treeDir, "Music");
        static readonly string charDir = Path.Combine(treeDir, "Characters");
        static readonly string dialogueDir = Path.Combine(treeDir, "Dialogue");
        static readonly string metadataDir = Path.Combine(treeDir, ".meta");

        static SBCFile lastReadCharacter;

        public static void Test()
        {
            //Path.Combine();
            //ZipFile.CreateFromDirectory();
            //File.WriteAllText()
        }

        //public static 

        public static void WriteCharacter(SBCFile character)
        {
            if (!Directory.Exists(charDir))
                Directory.CreateDirectory(charDir);

            string charName = character.filename;
            string charPath = Path.Combine(charDir, charName + ".json");
            string charJson = character.Serialize();
            File.WriteAllText(charPath, charJson);
        }

        public static SBCFile ReadCharacter(int index)
        {
            string chartName = $"chart{index}";
            string chartPath = Path.Combine(charDir, chartName + ".json");
            if (!File.Exists(chartPath))
            {
                throw new FileNotFoundException($"Chart {index} not found in RIQ file");
            }

            string charJson = File.ReadAllText(chartPath);
            //Debug.Log($"Jukebox loaded chart {chartPath} ({chartJson.Length} bytes)");

            lastReadCharacter = JsonConvert.DeserializeObject<SBCFile>(charJson);
            return lastReadCharacter;
        }

        public static string ExtractCharacter(string path)
        {
            ZipFile.ExtractToDirectory(path, charDir, true);
            return charDir;
        }

        public static void PackCharacter(string destPath)
        {
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
            ZipFile.CreateFromDirectory(charDir, destPath, System.IO.Compression.CompressionLevel.Optimal, false);
        }
    }
}