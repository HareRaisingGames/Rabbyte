using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rabbyte
{
    public class CharacterSprite : Image
    {
        SBCFile character;
        public string expression
        {
            set
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(character.GetEmotionByName(value).sprite);
                Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                this.sprite = sprite;
            }
        }
        protected CharacterSprite(SBCFile character)
        {
            this.character = character;
        }
        protected override void Awake()
        {
            base.Awake();
        }
    }
}
