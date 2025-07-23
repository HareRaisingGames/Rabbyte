using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Rabbyte
{ 
    [JsonConverter(typeof(SBCParser))]
    public class SBCFile
    {
        public string filename;
        public List<Emotion> expressions = new List<Emotion>();
        private int _curExp = -1;
        public int curExp
        {
            set
            {
                _curExp = value;
            }
        }

        public SBCFile(string name = "")
        {
            filename = name;
        }

        public void addExpression(string expression = "", byte[] sprite = null, float scale = 1, int x = 0, int y = 0)
        {
            Emotion emotion = new Emotion(expression, sprite, scale, x, y);
            expressions.Add(emotion);
            _curExp = expressions.IndexOf(emotion);
        }

        public void removeExpression()
        {
            expressions.RemoveAt(_curExp);
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
