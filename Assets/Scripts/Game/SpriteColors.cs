using UnityEngine;

public class SpriteColors
{
    public static Sprite Red
    {
        get
        {
            Texture2D tex = Resources.Load("Textures/Candy_red") as Texture2D;
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
        }
    }   
    public static Sprite Yellow
    {
        get
        {
            Texture2D tex = Resources.Load("Textures/Candy_yellow") as Texture2D;
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
        }
    }
    public static Sprite Green
    {
        get
        {
            Texture2D tex = Resources.Load("Textures/Candy_green") as Texture2D;
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
        }
    }
    public static Sprite LightBlue
    {
        get
        {
            Texture2D tex = Resources.Load("Textures/Candy_lightblue") as Texture2D;
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
        }
    }
    public static Sprite Blue
    {
        get
        {
            Texture2D tex = Resources.Load("Textures/Candy_blue") as Texture2D;
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
        }
    }
    public static Sprite Purple
    {
        get
        {
            Texture2D tex = Resources.Load("Textures/Candy_purple") as Texture2D;
            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 512);
        }
    }
}
