using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rabbyte;
using System.IO;

[System.Serializable]
public struct DialogueText
{
    public string name;
    public Language language;
    public string text;
    public AudioClip voice;
    public int id;

    public DialogueText(string language, int id)
    {
        this.name = "";
        this.language = (Language)System.Enum.Parse(typeof(Language), language);
        this.text = "";
        this.voice = null;
        this.id = id;
    }
}

[System.Serializable]
public struct CharacterPack
{
    public string character;
    public string emotion;
    public Alignment alignment;
    public float offset;
    public bool flipX;
    public bool isSpeaking;
    public SpriteTransition transition;
}

[System.Serializable]
public enum Language
{
    english,
    spanish,
    french,
    japanese,
    chinese,
    korean,
    german,
    russian,
    italian
}

[System.Serializable]
public enum Alignment
{
    left,
    center,
    right
}

public class AudioByte
{
    public byte[] data;
    public string type;
    public string name;

    public AudioByte(byte[] data = null, string type = "", string name = "")
    {
        this.data = data;
        this.name = name;
        this.type = type;
    }

    public AudioByte(string filename)
    {
        name = filename.Split("\\")[filename.Split("\\").Length - 1];
        data = File.ReadAllBytes(filename);
        type = AudioUtils.GetAudioType(filename, out _).ToString();
    }
}
