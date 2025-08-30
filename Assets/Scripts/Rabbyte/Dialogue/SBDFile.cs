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

        public SBDFile()
        {

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

        public SimpleSBDFile(bool load = false) : base()
        {
            if (!load)
                AddLine();
        }

        public SimpleSBDFile(SimpleSBDFile copy) : this(true)
        {
            fileName = copy.fileName;
            displayName = copy.displayName;
            chapter = copy.chapter;
            volume = copy.volume;
            type = copy.type;
            music = copy.music;

            foreach(BetaDialogueSequence line in copy.lines)
                AddLineByClass(line);

            foreach(KeyValuePair<string, byte[]> background in copy.GetBackgrounds())
                AddBackground(background.Key, background.Value);

            foreach (KeyValuePair<string, byte[]> foreground in copy.GetForegrounds())
                AddBackground(foreground.Key, foreground.Value);

            foreach (KeyValuePair<string, List<Emotion>> character in copy.GetCharacters())
                characters.Add(character.Key, character.Value);
        }

        public bool AddCharacter(SBCFile data)
        {
            if (characters.ContainsKey(data.filename))
                return false;

            characters.Add(data.filename, data.expressions);
            return true;
        }

        public void RemoveCharacter(string name)
        {
            if (characters.ContainsKey(name))
                characters.Remove(name);
        }

        public Dictionary<string, List<Emotion>> GetCharacters() => characters;

        public bool AddBackground(string name, byte[] data)
        {
            if (backgrounds.ContainsKey(name) || HasExistingBGBytes(data))
                return false;

            backgrounds.Add(name, data);
            return true;
        }

        public void RemoveBackground(string name)
        {
            if (backgrounds.ContainsKey(name))
                backgrounds.Remove(name);
        }

        public Dictionary<string, byte[]> GetBackgrounds() => backgrounds;

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

        public bool AddForeground(string name, byte[] data)
        {
            if (foregrounds.ContainsKey(name) || HasExistingFGBytes(data))
                return false;

            foregrounds.Add(name, data);
            return true;
        }

        public void RemoveForeground(string name)
        {
            if (foregrounds.ContainsKey(name))
                foregrounds.Remove(name);
        }

        public Dictionary<string, byte[]> GetForegrounds() => foregrounds;

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
        public List<BetaDialogueSequence> GetLines() => lines;
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

        public void AddLine()
        {
            lines.Add(new BetaDialogueSequence(lines.Count));
        }

        public void AddLineByClass(BetaDialogueSequence dialogue)
        {
            lines.Add(dialogue);
        }

        public void AddLineByValues(int id, string name = "", string text = "", string background = "", bool autoplay = false)
        {
            BetaDialogueSequence dialogue = new BetaDialogueSequence(id);
            dialogue.name = name;
            dialogue.text = text;
            dialogue.background = background;
            dialogue.autoSkip = autoplay;
            lines.Add(dialogue);
        }

        public void InsertLineByValues(int id, string name = "", string text = "", string background = "", bool autoplay = false)
        {
            BetaDialogueSequence dialogue = new BetaDialogueSequence(id);
            dialogue.name = name;
            dialogue.text = text;
            dialogue.background = background;
            dialogue.autoSkip = autoplay;
            lines.Insert(id, dialogue);
        }

        public void DuplicateCharacters(List<CharacterPack> copy, List<CharacterPack> paste)
        {
            paste.Clear();
            foreach(CharacterPack character in copy)
            {
                CharacterPack chara = new CharacterPack();
                chara.alignment = character.alignment;
                chara.character = character.character;
                chara.emotion = character.emotion;
                chara.flipX = character.flipX;
                chara.isSpeaking = character.isSpeaking;
                chara.offset = character.offset;
                paste.Add(chara);
            }
        }
        #endregion

        #region Utils
        public bool Equals(SimpleSBDFile otherFile)
        {
            if (fileName != otherFile.fileName)
                return false;
            if (displayName != otherFile.displayName)
                return false;
            if (chapter != otherFile.chapter)
                return false;
            if (volume != otherFile.volume)
                return false;
            if(music != null || otherFile.music != null)
            {
                if (!music.SequenceEqual(otherFile.music))
                    return false;
            }

            bool equalBackgrounds = backgrounds.Count == otherFile.GetBackgrounds().Count && !backgrounds.Except(otherFile.GetBackgrounds()).Any();
            if (!equalBackgrounds)
                return false;

            bool equalForegrounds = foregrounds.Count == otherFile.GetForegrounds().Count && !foregrounds.Except(otherFile.GetForegrounds()).Any();
            if (!equalForegrounds)
                return false;

            bool equalCharacters = characters.Count == otherFile.GetCharacters().Count && !characters.Except(otherFile.GetCharacters()).Any();
            if (!equalCharacters)
                return false;

            if (lines.Count != otherFile.GetLines().Count)
                return false;

            foreach(BetaDialogueSequence line in lines)
            {
                BetaDialogueSequence otherLine = otherFile.GetLines()[lines.IndexOf(line)];
                if (!line.Equals(otherLine))
                    return false;
            }

            return true;
        }
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

        public bool Equals(BetaDialogueSequence copyDialogue)
        {
            if (name != copyDialogue.name)
                return false;
            if (text != copyDialogue.text)
                return false;
            if (audio != copyDialogue.audio)
                return false;
            if (background != copyDialogue.background)
                return false;
            if (foreground != copyDialogue.foreground)
                return false;
            if (autoSkip != copyDialogue.autoSkip)
                return false;

            if (characters.Count != copyDialogue.characters.Count)
                return false;

            foreach(CharacterPack character in characters)
            {
                CharacterPack otherCharacter = copyDialogue.characters[characters.IndexOf(character)];
                if (character.character != otherCharacter.character)
                    return false;
                if (character.emotion != otherCharacter.emotion)
                    return false;
                if (character.alignment != otherCharacter.alignment)
                    return false;
                if (character.offset != otherCharacter.offset)
                    return false;
                if (character.flipX != otherCharacter.flipX)
                    return false;
                if (character.isSpeaking != otherCharacter.isSpeaking)
                    return false;
            }

            return true;
        }
    }

    [System.Serializable]
    public enum StoryType
    {
        Main,
        Side
    }
}
