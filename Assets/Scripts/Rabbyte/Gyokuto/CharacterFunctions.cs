using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rabbyte;

public static partial class LuaMethods
{
    static CharacterSprite[] GetAllCharactersInScene()
    {
        return Object.FindObjectsOfType(typeof(CharacterSprite)) as CharacterSprite[];
    }
    public static void SetExpressionToCharacter(string character, string expression)
    {
        CharacterSprite charat = null;
        CharacterSprite[] sprites = GetAllCharactersInScene();
        foreach(CharacterSprite sprite in sprites)
        {
            if(sprite.charName == character)
            {
                charat = sprite;
            }
        }

        if(charat != null)
        {
            if (charat.ExpressionExists(expression))
                charat.expression = expression;
        }
    }
}
