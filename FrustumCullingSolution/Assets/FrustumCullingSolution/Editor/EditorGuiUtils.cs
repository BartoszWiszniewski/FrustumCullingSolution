#if UNITY_EDITOR
using UnityEngine;

namespace FrustumCullingSolution.Editor
{
    public static class EditorGuiUtils
    {
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        public static Texture2D MakeBackground(Color col)
        {
            return MakeTex(2, 2, col);
        }
        
        public static string ToLabelText(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return propertyName;

            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append(char.ToUpper(propertyName[0]));

            for (int i = 1; i < propertyName.Length; i++)
            {
                if (char.IsUpper(propertyName[i]) && !char.IsUpper(propertyName[i - 1]))
                {
                    result.Append(' ');
                }
                result.Append(propertyName[i]);
            }

            return result.ToString();
        }
    }
}
#endif