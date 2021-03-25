using Godot;
using System;

[Tool]
public class Background : TextureRect
{
    public override void _Ready()
    {
        if (Texture != null)
        {
            RectSize = Screen.Size;
            RectPosition = Screen.Position;
        }
    }
}
