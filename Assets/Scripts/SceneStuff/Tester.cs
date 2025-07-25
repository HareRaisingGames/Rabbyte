using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rabbyte;
using System.Threading.Tasks;
using System;
using System.IO;
using SFB;
public class Tester : MonoBehaviour
{
    public Image image;
    public SBCFile file = new();

    // Start is called before the first frame update
    void Start()
    {
        file.addExpression();
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
                    byte[] imageData = File.ReadAllBytes(paths[0]);
                    file.setImage(imageData);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageData);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    image.sprite = sprite;
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

    public void SaveWindows()
    {
        StandaloneFileBrowser.SaveFilePanelAsync("Save Character", "", "char", "sbc", (string path) => {
            Debug.Log(path.Length);
            if(path.Length != 0)
            {
                StarbornFileHandler.WriteCharacter(file);
                StarbornFileHandler.PackCharacter(path);
                return;
            }
            
        });
    }
}
