using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Rabbyte
{
    public class SBDParser : JsonConverter<SBDFile>
    {
        public override SBDFile ReadJson(JsonReader reader, Type objectType, SBDFile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject obj = JObject.Load(reader);
            SBDFile file = new();

            JToken bgs = obj["backgrounds"].Value<JToken>();
            JObject backgrounds = JObject.Load(bgs.CreateReader());

            foreach(JProperty property in backgrounds.Properties())
            {
                byte[] bg = Convert.FromBase64String(backgrounds[property.Name].Value<string>());
                file.AddBackground(property.Name, bg);
            }

            JToken fgs = obj["foregrounds"].Value<JToken>();
            JObject foregrounds = JObject.Load(fgs.CreateReader());

            foreach (JProperty property in foregrounds.Properties())
            {
                byte[] fg = Convert.FromBase64String(foregrounds[property.Name].Value<string>());
                file.AddForeground(property.Name, fg);
            }

            file.volume = obj.ContainsKey("volume") ? obj["volume"].Value<int>() : 0;

//            JArray backgrounds = obj["backgrounds"].Value<JArray>();
//            foreach (JToken bg in backgrounds)
//            {

//            }

            return file;
        }

        public override void WriteJson(JsonWriter writer, SBDFile value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("backgrounds");
            writer.WriteStartObject();
            foreach (KeyValuePair<string, byte[]> bg in value.GetBackgrounds())
            {
                writer.WritePropertyName(bg.Key);
                writer.WriteValue(bg.Value);
            }
            writer.WriteEndObject();

            writer.WritePropertyName("foregrounds");
            writer.WriteStartObject();
            foreach (KeyValuePair<string, byte[]> fg in value.GetForegrounds())
            {
                writer.WritePropertyName(fg.Key);
                writer.WriteValue(fg.Value);
            }
            writer.WriteEndObject();

            //Optional Value
            if (value.volume.HasValue)
            {
                writer.WritePropertyName("volume");
                writer.WriteValue(value.volume);
            }

            writer.WritePropertyName("lines");
            writer.WriteStartArray();
            //foreach (BetaDialogueSequence dialogue in value.GetLines())
            //{
                //writer.WriteStartObject();

                //writer.WriteEndObject();
            //}
            writer.WriteEndArray();


            writer.WriteEndObject();
        }
    }

    public class SimpleSBDParser : JsonConverter<SimpleSBDFile>
    {
        public override SimpleSBDFile ReadJson(JsonReader reader, Type objectType, SimpleSBDFile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject obj = JObject.Load(reader);
            SimpleSBDFile file = new();

            JToken bgs = obj["backgrounds"].Value<JToken>();
            JObject backgrounds = JObject.Load(bgs.CreateReader());

            foreach (JProperty property in backgrounds.Properties())
            {
                byte[] bg = Convert.FromBase64String(backgrounds[property.Name].Value<string>());
                file.AddBackground(property.Name, bg);
            }

            JToken fgs = obj["foregrounds"].Value<JToken>();
            JObject foregrounds = JObject.Load(fgs.CreateReader());

            foreach (JProperty property in foregrounds.Properties())
            {
                byte[] fg = Convert.FromBase64String(foregrounds[property.Name].Value<string>());
                file.AddForeground(property.Name, fg);
            }

            file.volume = obj.ContainsKey("volume") ? obj["volume"].Value<int>() : 0;
            file.chapter = obj["chapter"].Value<int>();
            file.music = obj.ContainsKey("music") ? Convert.FromBase64String(obj["music"].Value<string>()) : null;

            string type = obj["type"].Value<string>();
            file.type = (StoryType)Enum.Parse(typeof(StoryType), type);

            file.description = obj.ContainsKey("description") ? obj["description"].Value<string>() : "";

            file.displayName = obj["displayName"].Value<string>();

            JToken chars = obj["characters"].Value<JToken>();
            JObject characters = JObject.Load(chars.CreateReader());

            foreach(JProperty property in characters.Properties())
            {
                SBCFile newChar = new SBCFile(false);
                newChar.filename = property.Name;
                JArray expressions = characters[property.Name].Value<JArray>();

                foreach (JToken emotion in expressions)
                {
                    JObject emo = JObject.Load(emotion.CreateReader());
                    string expression = emo["expression"].Value<string>();
                    byte[] sprite = Convert.FromBase64String(emo["sprite"].Value<string>());
                    float scale = emo["scale"].Value<float>();
                    int[] offset = new int[2];
                    //offset[0] = offset[1] = 0;
                    JArray offsets = emo["offset"].Value<JArray>();
                    for (int i = 0; i < offsets.Count; i++)
                    {
                        offset[i] = offsets[i].Value<int>();
                    }

                    newChar.addExpression(expression, sprite, scale, offset[0], offset[1]);
                }

                file.AddCharacter(newChar);
            }

            JArray lines = obj["lines"].Value<JArray>();
            foreach(JToken line in lines)
            {
                JObject dialogue = JObject.Load(line.CreateReader());
                BetaDialogueSequence betaDialogueSequence = new BetaDialogueSequence(dialogue["id"].Value<int>());

                betaDialogueSequence.name = dialogue.ContainsKey("name") ? dialogue["name"].Value<string>() : "";
                betaDialogueSequence.text = dialogue["text"].Value<string>();
                betaDialogueSequence.audio = dialogue.ContainsKey("audio") ? Convert.FromBase64String(dialogue["audio"].Value<string>()) : null;
                betaDialogueSequence.autoSkip = dialogue["autoSkip"].Value<bool>();
                betaDialogueSequence.background = dialogue.ContainsKey("background") ? dialogue["background"].Value<string>() : "";
                betaDialogueSequence.foreground = dialogue.ContainsKey("foreground") ? dialogue["foreground"].Value<string>() : "";

                if (dialogue.ContainsKey("characters"))
                {
                    JArray characterInfo = dialogue["characters"].Value<JArray>();
                    foreach (JToken character in characterInfo)
                    {
                        CharacterPack charPack = new CharacterPack();
                        JObject pack = JObject.Load(character.CreateReader());
                        charPack.character = pack["character"].Value<string>();
                        charPack.emotion = pack["expression"].Value<string>();
                        charPack.alignment = (Alignment)System.Enum.Parse(typeof(Alignment), pack["alignment"].Value<string>());
                        charPack.offset = pack.ContainsKey("offset") ? pack["offset"].Value<float>() : 0;
                        charPack.flipX = pack["flipX"].Value<bool>();
                        charPack.flipX = pack["isSpeaking"].Value<bool>();

                        betaDialogueSequence.characters.Add(charPack);
                    }
                }

                file.AddLineByClass(betaDialogueSequence);

            }

            return file;
        }

        public override void WriteJson(JsonWriter writer, SimpleSBDFile value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("backgrounds");
            writer.WriteStartObject();
            foreach (KeyValuePair<string, byte[]> bg in value.GetBackgrounds())
            {
                writer.WritePropertyName(bg.Key);
                writer.WriteValue(bg.Value);
            }
            writer.WriteEndObject();

            writer.WritePropertyName("foregrounds");
            writer.WriteStartObject();
            foreach (KeyValuePair<string, byte[]> fg in value.GetForegrounds())
            {
                writer.WritePropertyName(fg.Key);
                writer.WriteValue(fg.Value);
            }
            writer.WriteEndObject();

            writer.WritePropertyName("characters");
            writer.WriteStartObject();
            foreach (KeyValuePair<string, List<Emotion>> character in value.GetCharacters())
            {
                writer.WritePropertyName(character.Key);
                writer.WriteStartArray();
                foreach (Emotion emotion in character.Value)
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("expression");
                    writer.WriteValue(emotion.expression);
                    writer.WritePropertyName("sprite");
                    writer.WriteValue(emotion.sprite);
                    writer.WritePropertyName("scale");
                    writer.WriteValue(emotion.scale);
                    writer.WritePropertyName("offset");
                    writer.WriteStartArray();
                    writer.WriteValue(emotion.offset[0]);
                    writer.WriteValue(emotion.offset[1]);
                    writer.WriteEndArray();

                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                //writer.WriteValue(character.Value);
            }
            writer.WriteEndObject();

            //Optional Value
            if (value.volume.HasValue)
            {
                writer.WritePropertyName("volume");
                writer.WriteValue(value.volume);
            }

            if(value.description != "" && value.description != null)
            {
                writer.WritePropertyName("description");
                writer.WriteValue(value.description);
            }

            writer.WritePropertyName("chapter");
            writer.WriteValue(value.chapter);

            writer.WritePropertyName("displayName");
            writer.WriteValue(value.displayName);

            writer.WritePropertyName("type");
            writer.WriteValue(value.type.ToString());

            if (value.music != null)
            {
                writer.WritePropertyName("music");
                writer.WriteValue(value.music);
            }

            writer.WritePropertyName("lines");
            writer.WriteStartArray();
            foreach (BetaDialogueSequence dialogue in value.GetLines())
            {
                writer.WriteStartObject();

                //Optional Values
                if (!string.IsNullOrEmpty(dialogue.name))
                {
                    writer.WritePropertyName("name");
                    writer.WriteValue(dialogue.name);
                }
                
                writer.WritePropertyName("text");
                writer.WriteValue(dialogue.text);
                
                if(dialogue.audio != null)
                {
                    writer.WritePropertyName("audio");
                    writer.WriteValue(dialogue.audio);
                }

                writer.WritePropertyName("id");
                writer.WriteValue(dialogue.id);

                if (!string.IsNullOrEmpty(dialogue.background))
                {
                    writer.WritePropertyName("background");
                    writer.WriteValue(dialogue.background);
                }

                if (!string.IsNullOrEmpty(dialogue.foreground))
                {
                    writer.WritePropertyName("foreground");
                    writer.WriteValue(dialogue.foreground);
                }

                writer.WritePropertyName("autoSkip");
                writer.WriteValue(dialogue.autoSkip);

                if(dialogue.characters != null && dialogue.characters.Count != 0)
                {
                    writer.WritePropertyName("characters");
                    writer.WriteStartArray();
                    foreach(CharacterPack pack in dialogue.characters)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("character");
                        writer.WriteValue(pack.character);
                        writer.WritePropertyName("expression");
                        writer.WriteValue(pack.emotion);
                        writer.WritePropertyName("alignment");
                        writer.WriteValue(pack.alignment.ToString());
                        if(pack.offset != 0)
                        {
                            writer.WritePropertyName("offset");
                            writer.WriteValue(pack.offset);
                        }
                        writer.WritePropertyName("flipX");
                        writer.WriteValue(pack.flipX);
                        writer.WritePropertyName("isSpeaking");
                        writer.WriteValue(pack.isSpeaking);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
                
                //End of line object
                writer.WriteEndObject();
            }
            //End of line array
            writer.WriteEndArray();


            writer.WriteEndObject();
        }
    }
}
