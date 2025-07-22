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

        public void addExpression()
        {
            Emotion emotion = new Emotion();
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
    public Emotion(string[] args)
    {
        expression = "";
        sprite = null;
        scale = 1;
        offset = new int[2];

    }
}
