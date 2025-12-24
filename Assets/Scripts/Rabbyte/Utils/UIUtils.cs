using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Rabbyte
{
    public static class UIUtils
    {
        public static void SetImageFixedPosition(Image image, Vector2 baseDimensions = default(Vector2))
        {
            if (baseDimensions == default(Vector2))
                baseDimensions = new Vector2(800, 450);

            if (image.sprite == null) return;

            bool heightIsBigger = image.sprite.texture.height >= image.sprite.texture.width;
            Vector2 defaultSize = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
            image.SetNativeSize();
            if (heightIsBigger)
            {
                float widthAspect = baseDimensions.x / image.sprite.texture.width;
                image.rectTransform.sizeDelta = defaultSize * widthAspect;
                /*float heightAspect = baseDimensions.y / image.sprite.texture.height;
                image.rectTransform.sizeDelta = defaultSize * heightAspect;*/

            }
            else
            {
                float heightAspect = baseDimensions.y / image.sprite.texture.height;
                image.rectTransform.sizeDelta = defaultSize * heightAspect;
                /*float widthAspect = baseDimensions.x / image.sprite.texture.width;
                image.rectTransform.sizeDelta = defaultSize * widthAspect;*/
            }
        }
    }
}

