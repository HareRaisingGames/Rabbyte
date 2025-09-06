using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using Rabbyte;
using System;

public static class AudioUtils
{
    public static AudioType GetAudioType(string path, out string specificType)
    {
        if (!File.Exists(path)) throw new System.IO.FileNotFoundException("path", $"file does not exist at path {path}");

        AudioType audioType = AudioType.UNKNOWN;
        specificType = "Unknown";
        // determine audio type based on file contents, not extension
        using (FileStream fs = File.OpenRead(path))
        {
            byte[] buffer = new byte[4];
            fs.Read(buffer, 0, 4);
            string head = System.Text.Encoding.UTF8.GetString(buffer);
            string sub;
            switch (head)
            {
                case "OggS":
                    audioType = AudioType.OGGVORBIS;
                    specificType = "OggVorbis";
                    break;
                case "RIFF":
                    fs.Position = 8;
                    fs.Read(buffer, 0, 4);
                    sub = System.Text.Encoding.UTF8.GetString(buffer);
                    if (sub == "WAVE")
                    {
                        audioType = AudioType.WAV;
                        specificType = "WAV";
                    }
                    break;
                case "FORM":
                    fs.Position = 8;
                    fs.Read(buffer, 0, 4);
                    sub = System.Text.Encoding.UTF8.GetString(buffer);
                    if (sub == "AIFF")
                    {
                        audioType = AudioType.AIFF;
                        specificType = "AIFF";
                    }
                    else if (sub == "AIFC")
                    {
                        audioType = AudioType.AIFF;
                        specificType = "AIFC";
                    }
                    break;
                default:
                    fs.Position = 0;
                    byte[] buffer3 = new byte[3];
                    fs.Read(buffer3, 0, 3);
                    sub = System.Text.Encoding.UTF8.GetString(buffer3);
                    if (sub == "ID3")
                    {
                        audioType = AudioType.MPEG;
                        specificType = "mp3";
                    }
                    else if (buffer3[0] == 0xFF && (buffer3[1] & 0x0A) == 0x0A)
                    {
                        // this condition can literally trip out of chance
                        audioType = AudioType.MPEG;
                        specificType = "mp3";
                    }
                    break;
            }
        }
        return audioType;
    }
    ///<summary>
    /// Notes
    /// * Figure out how to tell the audio file
    /// * Convert specific audio file
    ///</summary>
    ///
    public static async Task<AudioClip> LoadClip(AudioByte bytes)
    {
        AudioClip clip = null;
        string path = Path.Combine(StarbornFileHandler.audioDir, bytes.name);
        AudioType type = (AudioType)System.Enum.Parse(typeof(AudioType), bytes.type);
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, type))
        {
            request.SendWebRequest();
            try
            {
                while (!request.isDone) await Task.Delay(5);

                if (request.isNetworkError || request.isHttpError) Debug.Log($"{request.error}");
                else
                {
                    clip = DownloadHandlerAudioClip.GetContent(request);
                    clip.name = bytes.name.Remove(bytes.name.Length - 4);
                }
            }
            catch (Exception err)
            {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }

        return clip;
    }
}
