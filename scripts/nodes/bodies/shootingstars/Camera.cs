using Godot;
using System;
using System.Threading.Tasks;

public class Camera : Camera2D
{
    [Export]
    private float _offsetScreenY = 0.85F;

    private Vector2 _screenSize;
    private float _offsetX;
    private float _offsetY;

    public override void _Ready()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;
        _offsetX = _screenSize.x / 2;
        _offsetY = -(_screenSize.y * _offsetScreenY);
        // setting camera offset
        Offset = new Vector2(_offsetX, _offsetY);
    }

    public async Task MoveOffset() {
        var offsetTween = GetNode<Tween>("OffsetTween");
        float originY = -(_screenSize.y * 1.25F);
        Offset = new Vector2(_offsetX, originY);
        Vector2 offsetDestination = new Vector2(_offsetX, _offsetY);
        offsetTween.InterpolateProperty(this, "offset", Offset, offsetDestination, 2, Tween.TransitionType.Quint);
        offsetTween.Start();
        await ToSignal(offsetTween, "tween_completed");
    }
}
