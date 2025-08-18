using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rabbyte;

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
