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
            throw new NotImplementedException();
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
                writer.WriteValue(emotion.offset);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}

