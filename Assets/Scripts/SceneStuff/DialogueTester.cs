using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rabbyte;
using System.Threading.Tasks;
using System;
using System.IO;
using System.IO.Compression;
using SFB;
using UnityEngine.Networking;
using System.Linq;

public class DialogueTester : MonoBehaviour
{
    public Dropdown dropdown;
    public Image image;
    public Image charImage;
    public SimpleSBDFile file = new();

    List<string> bgFileNames = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(Application.temporaryCachePath);
        foreach (KeyValuePair<string, List<Emotion>> character in file.GetCharacters())
            Debug.Log(character);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenWindows()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Image Files", "jpg", "png")
        };
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, false, async (string[] paths) =>
        {
            if (paths.Length > 0)
            {
                try
                {
                    string filename = paths[0].Split("\\")[paths[0].Split("\\").Length - 1];
                    filename = filename.Remove(filename.Length - 4);
                    byte[] imageData = File.ReadAllBytes(paths[0]);
                    if(!file.GetBackgrounds().ContainsKey(paths[0]) || !file.HasExistingBGBytes(imageData))
                    {
                        bgFileNames.Add(filename);
                        file.AddBackground(filename, imageData);
                        dropdown.ClearOptions();
                        dropdown.AddOptions(bgFileNames);
                        dropdown.value = bgFileNames.Count - 1;
                        ChangeBG();
                    }
                    return;
                }
                catch (System.Exception e)
                {

                    return;
                }
            }
            await Task.Yield();
        });
    }

    public void OpenAudio()
    {
        var extensions = new[]
{
            new ExtensionFilter("Audio Files", "mp3", "wav", "ogg")
        };
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, false, async (string[] paths) =>
        {
            if (paths.Length > 0)
            {
                try
                {
                    //var filename = paths[0].Split("\\")[paths[0].Split("\\").Length - 1];
                    //filename = filename.Remove(filename.Length - 4);

                    //byte[] audioData = File.ReadAllBytes(paths[0]);
                    //Debug.Log(audioData);
                    /*AudioSource audio = new GameObject("Audio").AddComponent<AudioSource>();
                    
                     = Converters.ByteToAudioClip(bytes);
                    Debug.Log(clip);
                    audio.clip = clip;
                    audio.Play();*/
                    AudioByte bytes = new AudioByte(paths[0]);
                    StarbornFileHandler.CacheAudio(bytes.name, bytes.data);
                    AudioSource audio = new GameObject("Audio").AddComponent<AudioSource>();
                    //audio.clip = await AudioUtils.LoadClip(bytes);
                    audio.Play();
                }
                catch (System.Exception e)
                {

                }
                await Task.Yield();
            }
        });
    }

    public void ChangeBG()
    {
        string textName = dropdown.options[dropdown.value].text;
        if(file.GetBackgrounds().ContainsKey(textName))
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(file.getBackgroundFromName(textName));
            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            image.sprite = sprite;
        }
    }

    public void SaveWindows()
    {
        StandaloneFileBrowser.SaveFilePanelAsync("Save Dialogue File", "", "dialogue", "sbd", (string path) => {
            Debug.Log(path.Length);
            if (path.Length != 0)
            {
                var filename = path.Split("\\")[path.Split("\\").Length - 1];
                filename = filename.Remove(filename.Length - 4);
                StarbornFileHandler.WriteSimpleDialogue(file, filename);
                StarbornFileHandler.PackDialogue(path);
                //copy = new(file);
                return;
            }

        });
    }

    public void LoadWindows()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Character Files", "sbd")
        };
        StandaloneFileBrowser.OpenFilePanelAsync("Load Dialogue File", "", extensions, false, async (string[] paths) =>
        {
            if (paths.Length > 0)
            {
                try
                {
                    var filename = paths[0].Split("\\")[paths[0].Split("\\").Length - 1];
                    filename = filename.Remove(filename.Length - 4);
                    var path = StarbornFileHandler.ExtractDialogue(paths[0]);
                    file = StarbornFileHandler.ReadSimpleDialogue(filename);

                    //foreach (KeyValuePair<string, List<Emotion>> character in file.GetCharacters())
                        //Debug.Log(character.Value);

                    bgFileNames.Clear();
                    foreach (KeyValuePair<string, byte[]> background in file.GetBackgrounds())
                    {
                        bgFileNames.Add(background.Key);
                        //file.backgrounds.Add(filename, imageData);
                        dropdown.ClearOptions();
                        dropdown.AddOptions(bgFileNames);
                        dropdown.value = 0;
                        ChangeBG();
                    }
                    //Texture2D tex = new Texture2D(2, 2);
                    //tex.LoadImage(file.expressions[0].sprite);
                    //Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    //image.sprite = sprite;
                    //Debug.Log(file.expressions.Count);
                    //copy = new(file);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                    return;
                }
            }
            await Task.Yield();
        });
    }

    public void LoadCharacter()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Character Files", "sbc")
        };
        StandaloneFileBrowser.OpenFilePanelAsync("Load Character", "", extensions, false, async (string[] paths) =>
        {
            if (paths.Length > 0)
            {
                try
                {
                    var filename = paths[0].Split("\\")[paths[0].Split("\\").Length - 1];
                    filename = filename.Remove(filename.Length - 4);
                    var path = StarbornFileHandler.ExtractCharacter(paths[0]);
                    var character = StarbornFileHandler.ReadCharacter(filename);
                    file.AddCharacter(character);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(character.expressions[0].sprite);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    charImage.sprite = sprite;
                }
                catch (System.Exception e)
                {

                    return;
                }
            }
            await Task.Yield();
        });
    }
}
