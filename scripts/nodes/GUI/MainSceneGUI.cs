using Godot;
using System;
using System.Threading.Tasks;

public enum Fade
{
    In,
    Out,
}

public class MainSceneGUI : Control
{
    const float FADE_DURATION = 0.5f;
    private bool _ready = false;
    private Game _game;
    private MarginContainer _topHalf;
    private MarginContainer _bottomHalf;
    private MarginContainer _logoContainer;
    private CenterContainer _titleContainer;
    private TextureButton _playButton;
    private Tween _fadeLogoTween;
    private Tween _fadeTitleTween;
    private Tween _fadeBottomHalfTween;

    public override async void _Ready()
    {
        _getNodes();
        _connectSignals();
        _InitGUI();
        await _game.Wait();  // wait on start to get full animations
        await _FadeInGUI();
        _ready = true;
    }

    private void _getNodes() {
        _game = (Game)GetNode("/root/Game");
        _topHalf = (MarginContainer)GetNode("TopHalf");
        _bottomHalf = (MarginContainer)GetNode("BottomHalf");
        _logoContainer = (MarginContainer)_topHalf.GetNode("VBoxContainer").GetNode("LogoContainer");
        _titleContainer = (CenterContainer)_topHalf.GetNode("VBoxContainer").GetNode("TitleContainer");
        _fadeLogoTween = (Tween)GetNode("FadeLogoTween");
        _fadeTitleTween = (Tween)GetNode("FadeTitleTween");
        _fadeBottomHalfTween = (Tween)GetNode("FadeBottomHalfTween");
        _playButton = (TextureButton)_bottomHalf
            .GetNode("VBoxContainer")
            .GetNode("PlayButtonContainer")
            .GetNode("MarginContainer")
            .GetNode("PlayButton");
    }

    private void _connectSignals() {
        _playButton.Connect("pressed", this, nameof(_OnPlayPressed));
    }

    private void _InitGUI() {
        // set main screen block positions
        _topHalf.RectPosition += Screen.Position;
        _bottomHalf.RectPosition += Screen.Position;
        var fixedBottomY = Screen.Size.y - Screen.MinRatioSize.y;
        _bottomHalf.RectPosition += new Vector2(0, fixedBottomY);

        // set main screen block scales
        var scaleComponentTop = Screen.Size.x / _topHalf.RectSize.x;
        var scaleComponentBottom = Screen.Size.x / _bottomHalf.RectSize.x;
        _topHalf.RectScale = new Vector2(scaleComponentTop, scaleComponentTop);
        _bottomHalf.RectScale = new Vector2(scaleComponentBottom, scaleComponentBottom);

        // hide containers
        Color transparent = new Color(1, 1, 1, 0);
        _logoContainer.Modulate = transparent;
        _titleContainer.Modulate = transparent;
        _bottomHalf.Modulate = transparent;
    }
    
    private async Task FadeNode(Tween tween, CanvasItem node, Fade fade) {
        Color initial = fade == Fade.Out ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
        Color final = fade == Fade.Out ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);

        tween.InterpolateProperty(node, "modulate", initial, final, FADE_DURATION);
        tween.Start();
        await ToSignal(tween, "tween_completed");
    }

    private async Task _FadeInGUI() {
        await FadeNode(_fadeLogoTween, _logoContainer, Fade.In);
        await FadeNode(_fadeTitleTween, _titleContainer, Fade.In);
        await FadeNode(_fadeBottomHalfTween, _bottomHalf, Fade.In);
    }

    private async void _FadeOutGUI() {
        await FadeNode(_fadeBottomHalfTween, _bottomHalf, Fade.Out);
        await FadeNode(_fadeTitleTween, _titleContainer, Fade.Out);
        await FadeNode(_fadeLogoTween, _logoContainer, Fade.Out);
    }
    
    private void _OnPlayPressed() {
        if (!_ready || _game.Started) {
            return;
        }
        _FadeOutGUI();
        _game.Start();
    }
}
