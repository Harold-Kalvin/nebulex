using Godot;
using System;

[Tool]
public class Background : TextureRect
{
    public override void _Ready()
    {
        if (Texture != null)
        {
            RectSize = GetViewport().GetVisibleRect().Size;
        }
    }
}
