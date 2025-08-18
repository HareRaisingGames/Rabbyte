using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace Rabbyte
{
    [JsonConverter(typeof(SBDParser))]
    public class SBDFile
    {
        #region Metadata
        protected Dictionary<string, byte[]> backgrounds = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]> foregrounds = new Dictionary<string, byte[]>();
        protected List<string> characters = new List<string>();

        //If a user is going to edit the dialogue, how will the characters be brought back? I fear that using a dictionary is just going to make the file way too big
        //protected Dictionary<string, SBCFile> characters = new Dictionary<string, SBCFile>();

        //public List<DialogueSequence> lines = new List<DialogueSequence>();

        public int? chapter;
        public int? volume;

        public StoryType type = StoryType.Main;
        public string fileName;
        public string displayName;

        public void AddBackground(string name, byte[] data)
        {
            backgrounds.Add(name, data);
        }

        public void RemoveBackground(string name)
        {
            backgrounds.Remove(name);
        }

        public Dictionary<string, byte[]> GetBackgrounds()
        {
            return backgrounds;
        }

        //For loading a new background
        public byte[] getBackgroundFromName(string name)
        {
            if (backgrounds.ContainsKey(name))
                return backgrounds[name];
            return null;
        }

        //Gets the current background
        //For loading a background when switching through dialogue

        public void AddForeground(string name, byte[] data)
        {
            foregrounds.Add(name, data);
        }

        public Dictionary<string, byte[]> GetForegrounds()
        {
            return foregrounds;
        }

        public byte[] getForegroundFromName(string name)
        {
            if (foregrounds.ContainsKey(name))
                return foregrounds[name];
            return null;
        }
        #endregion

        #region Dialogue Data
        
        protected List<DialogueSequence> lines = new List<DialogueSequence>();
        public List<DialogueSequence> GetLines()
        {
            return lines;
        }
        private int _line;
        public DialogueSequence curLine
        {
            get
            {
                if (_line >= lines.Count)
                    return null;
                else
                    return lines[_line];
            }
        }

        public void ChangeDialogue(int change)
        {
            _line += change;
        }

        public void SetDialogue(int set)
        {
            _line = set;
        }
        #endregion

        #region Utils
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Arrays,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        }

        public bool HasExistingBGBytes(byte[] byteData) //This is just in case each file is under a different name but has the same byte data
        {
            foreach(KeyValuePair<string, byte[]> background in backgrounds)
            {
                if (background.Value.SequenceEqual(byteData)) return true;
            }
            return false;
        }

        public bool HasExistingFGBytes(byte[] byteData) //This is just in case each file is under a different name but has the same byte data
        {
            foreach (KeyValuePair<string, byte[]> foreground in foregrounds)
            {
                if (foreground.Value.SequenceEqual(byteData)) return true;
            }
            return false;
        }
        #endregion
    }

    [JsonConverter(typeof(SimpleSBDParser))]
    public class SimpleSBDFile
    {
        #region Metadata
        protected Dictionary<string, byte[]> backgrounds = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]> foregrounds = new Dictionary<string, byte[]>();
        //protected List<string> characters = new List<string>();

        //If a user is going to edit the dialogue, how will the characters be brought back? I fear that using a dictionary is just going to make the file way too big
        protected Dictionary<string, List<Emotion>> characters = new Dictionary<string, List<Emotion>>();

        //public List<DialogueSequence> lines = new List<DialogueSequence>();

        public int? chapter;
        public int? volume;

        public StoryType type = StoryType.Main;
        public string fileName;
        public string displayName;

        public byte[] music = null;

        public void AddCharacter(SBCFile data)
        {
            if(!characters.ContainsKey(data.filename))
                characters.Add(data.filename, data.expressions);
        }

        public void RemoveCharacter(string name)
        {
            if (!characters.ContainsKey(name))
                characters.Remove(name);
        }

        public Dictionary<string, List<Emotion>> GetCharacters()
        {
            return characters;
        }

        public void AddBackground(string name, byte[] data)
        {
            backgrounds.Add(name, data);
        }

        public void RemoveBackground(string name)
        {
            backgrounds.Remove(name);
        }

        public Dictionary<string, byte[]> GetBackgrounds()
        {
            return backgrounds;
        }

        //For loading a new background
        public byte[] getBackgroundFromName(string name)
        {
            if (backgrounds.ContainsKey(name))
                return backgrounds[name];
            return null;
        }

        //Gets the current background
        //For loading a background when switching through dialogue
        public byte[] getBackground()
        {
            if (backgrounds.ContainsKey(background))
                return backgrounds[background];
            return null;
        }

        public void AddForeground(string name, byte[] data)
        {
            foregrounds.Add(name, data);
        }

        public Dictionary<string, byte[]> GetForegrounds()
        {
            return foregrounds;
        }

        public byte[] getForegroundFromName(string name)
        {
            if (foregrounds.ContainsKey(name))
                return foregrounds[name];
            return null;
        }

        public byte[] getForeground()
        {
            if (foregrounds.ContainsKey(foreground))
                return foregrounds[foreground];
            return null;
        }
        #endregion

        #region Dialogue Data

        protected List<BetaDialogueSequence> lines = new List<BetaDialogueSequence>();
        public List<BetaDialogueSequence> GetLines()
        {
            return lines;
        }
        private int _line;
        public BetaDialogueSequence curLine
        {
            get
            {
                if (_line >= lines.Count)
                    return null;
                else
                    return lines[_line];
            }
        }

        //These won't fill in the file, rather these are what's going to be displayed in game
        public string background
        {
            get
            {
                return curLine != null ? curLine.background : "";
            }
            set
            {
                if (curLine != null) curLine.background = value;
            }
        }
        public string foreground
        {
            get
            {
                return curLine != null ? curLine.foreground : "";
            }
            set
            {
                if (curLine != null) curLine.foreground = value;
            }
        }
        public string name
        {
            get
            {
                return curLine != null ? curLine.name : "";
            }
            set
            {
                if (curLine != null) curLine.name = value;
            }
        }
        public string text
        {
            get
            {
                return curLine != null ? curLine.text : "";
            }
            set
            {
                if (curLine != null) curLine.text = value;
            }
        }
        public byte[] audio
        {
            get
            {
                return curLine != null ? curLine.audio : null;
            }
            set
            {
                if (curLine != null) curLine.audio = value;
            }
        }
        public bool autoSkip
        {
            get
            {
                return curLine != null ? curLine.autoSkip : false;
            }
            set
            {
                if (curLine != null) curLine.autoSkip = value;
            }
        }

        public List<CharacterPack> characterPack
        {
            get
            {
                return curLine != null ? curLine.characters : null;
            }
        }

        public void ChangeDialogue(int change)
        {
            _line += change;
        }

        public void SetDialogue(int set)
        {
            _line = set;
        }

        public void AddLineByClass(BetaDialogueSequence dialogue)
        {
            lines.Add(dialogue);
        }

        public void AddLineByValues(int id, string name = "", string text = "", byte[] data = null)
        {
            BetaDialogueSequence dialogue = new BetaDialogueSequence(id);
            dialogue.name = name;
            dialogue.text = text;
            dialogue.audio = data;
            lines.Add(dialogue);
        }
        #endregion

        #region Utils
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Arrays,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        }

        public bool HasExistingBGBytes(byte[] byteData) //This is just in case each file is under a different name but has the same byte data
        {
            foreach (KeyValuePair<string, byte[]> background in backgrounds)
            {
                if (background.Value.SequenceEqual(byteData)) return true;
            }
            return false;
        }

        public bool HasExistingFGBytes(byte[] byteData) //This is just in case each file is under a different name but has the same byte data
        {
            foreach (KeyValuePair<string, byte[]> foreground in foregrounds)
            {
                if (foreground.Value.SequenceEqual(byteData)) return true;
            }
            return false;
        }
        #endregion
    }

    [System.Serializable]
    public class DialogueSequence
    {
        //Character Sprites
        public List<CharacterPack> characters;

        //Backgrounds
        public string background;
        public string foreground;

        //Text line for each dialogue
        public DialogueText[] text;

        public DialogueSequence()
        {
            text = new DialogueText[9];
            string[] languageNames = System.Enum.GetNames(typeof(Language));

            for (int i = 0; i < text.Length; i++)
            {
                text[i] = new DialogueText(languageNames[i], i);
            }
        }
    }

    [System.Serializable]
    public class BetaDialogueSequence
    {
        //The beta version will only feature English, so we can figure out how to utilize audio bytes
        //Text line for each dialogue
        public string name;
        public string text;
        public byte[] audio = null;
        public int id;
        
        //Backgrounds
        public string background;
        public string foreground;

        //Character Sprites
        public List<CharacterPack> characters = new List<CharacterPack>();

        public bool autoSkip = false;


        public BetaDialogueSequence(int id)
        {
            this.id = id;
        }
    }

    [System.Serializable]
    public enum StoryType
    {
        Main,
        Side
    }
}
