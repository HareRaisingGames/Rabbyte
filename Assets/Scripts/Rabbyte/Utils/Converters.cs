using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class Converters
{
    public static byte[] StringToBytes(string code)
    {
        return Convert.FromBase64String(code);
    }
}
