using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//using OggVorbis;

public static class Converters
{
    public static byte[] StringToBytes(string code)
    {
        return Convert.FromBase64String(code);
    }

    public static AudioClip ByteToAudioClip(AudioByte bytes)
    {
        Debug.Log(bytes);
        if(bytes != null)
        {
            switch(bytes.type)
            {
                case "WAV":
                    //return WavUtility.ToAudioClip(bytes.data, 0);
                    break;
                case "MPEG":
                    break;
                case "OGGVORBIS":
                    //return VorbisPlugin.ToAudioClip(bytes.data, "clip");
                    break;
            }
        }
        return null;
    }
}
