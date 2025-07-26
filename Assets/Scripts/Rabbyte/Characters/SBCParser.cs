using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rabbyte
{
    public class SBCParser : JsonConverter<SBCFile>
    {
        public override SBCFile ReadJson(JsonReader reader, Type objectType, SBCFile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject obj = JObject.Load(reader);

            string name = obj.ContainsKey("name") ? obj["name"].Value<string>() : "";
            SBCFile file = new(name);

            JArray expressions = obj["expressions"].Value<JArray>();
            foreach(JToken emotion in expressions)
            {
                JObject emo = JObject.Load(emotion.CreateReader());
                string expression = emo["expression"].Value<string>();
                byte[] sprite = Convert.FromBase64String(emo["sprite"].Value<string>());
                float scale = emo["scale"].Value<float>();
                int[] offset = new int[2];
                //offset[0] = offset[1] = 0;
                JArray offsets = emo["offset"].Value<JArray>();
                for(int i = 0; i < offsets.Count; i++)
                {
                    offset[i] = offsets[i].Value<int>();
                }

                file.addExpression(expression, sprite, scale, offset[0], offset[1]);
            }

            return file;
        }
        public override void WriteJson(JsonWriter writer, SBCFile value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteValue(value.filename);

            writer.WritePropertyName("expressions");
            writer.WriteStartArray();
            foreach(Emotion emotion in value.expressions)
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
            writer.WriteEndObject();
        }
    }
}

