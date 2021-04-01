using Godot;
using System;

[Tool]
public class PlayButton : TextureButton
{
    public override void _Ready()
    {
        // sizing and positioning "play" right arrow
        var RightArrow = (TextureRect)GetNode("RightArrow");
        float RightArrowWidth = RectSize.x / 3;
        float RightArrowPositionCenter = (RectSize.x / 2) - (RightArrowWidth / 2);
        float RightArrowPositionOffset = ((RightArrowPositionCenter * 1.11f) - RightArrowPositionCenter);
        RightArrow.RectSize = new Vector2(RightArrowWidth, RightArrowWidth);        
        RightArrow.RectPosition = new Vector2(RightArrowPositionCenter + RightArrowPositionOffset, RightArrowPositionCenter);        
    }
}
