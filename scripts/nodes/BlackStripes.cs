using Godot;
using System;

public class BlackStripes : CanvasLayer
{
    public override void _Ready()
    {
        TextureRect leftStripe = (TextureRect)GetNode("LeftStripe");
        TextureRect rightStripe = (TextureRect)GetNode("RightStripe");
        Vector2 stripeSize = new Vector2(Screen.Position.x, Screen.Size.y);

        if (leftStripe.Texture != null) {
            leftStripe.RectSize = stripeSize;
        }
        if (rightStripe.Texture != null) {
            rightStripe.RectSize = stripeSize;
            rightStripe.RectPosition = new Vector2(GetViewport().GetVisibleRect().Size.x - stripeSize.x, 0);
        }
    }
}
