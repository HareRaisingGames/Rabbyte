using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rabbyte
{
    public class CharacterSprite : Image
    {
        SBCFile _character;
        Vector2 spriteSize;
        float _offset;
        string _expression;
        bool _flipX;

        Vector2 _position;
        public SBCFile character
        {
            set
            {
                _character = value;
                if(_character.expressions.Count != 0) expression = _character.expressions[0].expression;
            }
        }
        public string expression
        {
            set
            {
                _expression = value;
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(_character.GetEmotionByName(value).sprite);
                Sprite _sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                sprite = _sprite;
                SetNativeSize();
                spriteSize = rectTransform.sizeDelta;
                rectTransform.sizeDelta = spriteSize * _character.GetEmotionByName(value).scale;
                rectTransform.eulerAngles = new Vector3(0, _flipX ? 180 : 0, 0);
                rectTransform.anchoredPosition = new Vector2(_position.x + offset[0] + _offset, _position.y + offset[1]);
            }
        }

        public Vector2 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                rectTransform.anchoredPosition = new Vector2(_position.x + offset[0] + _offset, _position.y + offset[1]);
            }
        }

        public int[] offset
        {
            get
            {
                return _character.GetEmotionByName(_expression).offset;
            }
        }

        public float xOffset
        {
            set
            {
                _offset = value;
                rectTransform.anchoredPosition = new Vector2(_position.x + offset[0] + _offset, _position.y + offset[1]);
            }
        }

        public bool flipX
        {
            set
            {
                this.rectTransform.eulerAngles = new Vector3(0, value ? 180 : 0, 0);
                _flipX = value;
            }
        }
        public CharacterSprite(SBCFile character)
        {
            _character = character;
        }
        protected override void Awake()
        {
            base.Awake();
            color = Color.white;
        }
    }
}
