using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace Rabbyte
{
    [JsonConverter(typeof(SBCParser))]
    public class SBCFile
    {
        public string filename;
        public List<Emotion> expressions = new List<Emotion>();
        private int _curExp = -1;

        public Emotion curEmotion
        {
            get
            {
                return expressions[_curExp];
            }
        }
        public int curExp
        {
            get
            {
                return _curExp;
            }
            set
            {
                _curExp = value;
            }
        }

        public SBCFile(string name = "")
        {
            filename = name;
            addExpression();
            _curExp = 0;
        }

        //Create a copy of the file
        public SBCFile(SBCFile copy)
        {
            filename = copy.filename;
            foreach(Emotion expression in copy.expressions)
            {
                addExpression(expression.expression, expression.sprite, expression.scale, expression.offset[0], expression.offset[1]);
            }
        }

        //Add a new expression
        public void addExpression(string expression = "", byte[] sprite = null, float scale = 1, int x = 0, int y = 0)
        {
            Emotion emotion = new Emotion(expression, sprite, scale, x, y);
            expressions.Add(emotion);
            _curExp = expressions.IndexOf(emotion);
        }

        //Remove the following expression
        public void removeExpression()
        {
            expressions.RemoveAt(_curExp);
        }

        //Set the current name
        public void setName(string name)
        {
            if (expressions.Count != 0)
            {
                Emotion emotionSet = expressions[_curExp];
                emotionSet.expression = name;
                expressions[_curExp] = emotionSet;
            }
        }

        //Set the current image
        public void setImage(byte[] image)
        {
            if (expressions.Count != 0)
            {
                Emotion emotionSet = expressions[_curExp];
                emotionSet.sprite = image;
                expressions[_curExp] = emotionSet;
            }
        }

        //Set the current scale
        public void setScale(float scale)
        {
            if (expressions.Count != 0)
            {
                Emotion emotionSet = expressions[_curExp];
                emotionSet.scale = scale;
                expressions[_curExp] = emotionSet;
            }
        }

        //Set the current offset
        public void setOffset(int x = 0, int y = 0)
        {
            if (expressions.Count != 0)
            {
                Emotion emotionSet = expressions[_curExp];
                emotionSet.offset[0] = x;
                emotionSet.offset[1] = y;
                expressions[_curExp] = emotionSet;
            }
        }

        //Check if two files are equal to each other
        public bool Equals(SBCFile otherFile)
        {
            if (filename != otherFile.filename)
                return false;

            if (expressions.Count != otherFile.expressions.Count)
                return false;

            foreach (Emotion emotion in expressions)
            {
                Emotion otherEmotion = otherFile.expressions[expressions.IndexOf(emotion)];

                if (emotion.expression != otherEmotion.expression || emotion.scale != otherEmotion.scale)
                    return false;

                if (emotion.sprite != null && otherEmotion.sprite != null)
                {
                    if (!emotion.sprite.SequenceEqual(otherEmotion.sprite))
                        return false;
                }
                else
                {
                    if (emotion.sprite == null && otherEmotion.sprite != null)
                        return false;
                    else if (emotion.sprite != null && otherEmotion.sprite == null)
                        return false;
                }


                for (int i = 0; i < emotion.offset.Length; i++)
                {
                    if (emotion.offset[i] != otherEmotion.offset[i])
                        return false;
                }
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
    }
}

public struct Emotion
{
    public string expression;
    public byte[] sprite;
    public float scale;
    public int[] offset;
    public Emotion(string expression = "", byte[] sprite = null, float scale = 1, int x = 0, int y = 0)
    {
        this.expression = expression;
        this.sprite = sprite;
        this.scale = scale;
        offset = new int[2];
        offset[0] = x;
        offset[1] = y;

    }
}
