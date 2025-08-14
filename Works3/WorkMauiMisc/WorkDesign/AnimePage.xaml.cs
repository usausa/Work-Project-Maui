namespace WorkDesign;

public partial class AnimePage : ContentPage
{
    private CancellationTokenSource? _cts;
    private bool _breathing;
    private bool _rainbow;
    private readonly Color _initialColor;

    public AnimePage()
	{
		InitializeComponent();
        _initialColor = AnimatedBox.Color;
    }

    private void SetStatus(string text) => StatusLabel.Text = $"Status: {text}";

    private CancellationToken PrepareCts()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        return _cts.Token;
    }

    private async void OnMoveClicked(object sender, EventArgs e)
    {
        var token = PrepareCts();
        try
        {
            SetStatus("Move start");
            double width = this.Width;
            double targetX = Math.Min(width - 160, 200);
            await AnimatedBox.TranslateTo(targetX, 0, 600, Easing.SinOut);
            await AnimatedBox.TranslateTo(0, 0, 500, Easing.SinIn);
            SetStatus("Move done");
        }
        catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
        {
            SetStatus("Move canceled");
        }
    }

    private async void OnBounceMoveClicked(object sender, EventArgs e)
    {
        var token = PrepareCts();
        try
        {
            SetStatus("Bounce move");
            double targetX = 180;
            // 自作バウンス (Animationクラスでイージングカスタム)
            var animation = new Animation();
            animation.WithConcurrent(
                (p) => AnimatedBox.TranslationX = p,
                0, targetX,
                Easing.CubicOut
            );
            animation.WithConcurrent(
                (p) => AnimatedBox.TranslationY = p,
                0, -40,
                new Easing(t => Math.Sin(t * Math.PI)) // 上に持ち上げて戻る
            );
            TaskCompletionSource<bool> tcs = new();
            animation.Commit(this, "BounceMove", 16, 700,
                finished: (v, c) =>
                {
                    if (!c)
                        tcs.TrySetResult(true);
                    else
                        tcs.TrySetCanceled(token);
                });
            await tcs.Task;
            await AnimatedBox.TranslateTo(0, 0, 400, Easing.BounceOut);
            SetStatus("Bounce done");
        }
        catch (TaskCanceledException)
        {
            SetStatus("Bounce canceled");
            AnimatedBox.AbortAnimation("BounceMove");
        }
    }

    private async void OnRotateClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("Rotate 360");
        await AnimatedBox.RotateTo(360, 800, Easing.Linear);
        AnimatedBox.Rotation = 0;
        SetStatus("Rotate done");
    }

    private async void OnRelRotateClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("Relative rotate +90");
        await AnimatedBox.RelRotateTo(90, 400, Easing.SinInOut);
        SetStatus($"Rotation={AnimatedBox.Rotation:0}");
    }

    private async void OnScalePulseClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("Scale pulse");
        await AnimatedBox.ScaleTo(1.3, 250, Easing.CubicOut);
        await AnimatedBox.ScaleTo(1.0, 250, Easing.CubicIn);
        SetStatus("Scale pulse done");
    }

    private async void OnFadeBlinkClicked(object sender, EventArgs e)
    {
        var token = PrepareCts();
        try
        {
            SetStatus("Fade blink (3x)");
            for (int i = 0; i < 3; i++)
            {
                token.ThrowIfCancellationRequested();
                await AnimatedBox.FadeTo(0.2, 180, Easing.Linear);
                await AnimatedBox.FadeTo(1.0, 180, Easing.Linear);
            }
            SetStatus("Blink done");
        }
        catch (OperationCanceledException)
        {
            SetStatus("Blink canceled");
        }
    }

    private async void OnColorShiftClicked(object sender, EventArgs e)
    {
        var token = PrepareCts();
        try
        {
            SetStatus("Color shift");
            await AnimatedBox.ColorTo(AnimatedBox.Color, Colors.CornflowerBlue, c => AnimatedBox.Color = c, 600, Easing.SinInOut, token);
            await AnimatedBox.ColorTo(AnimatedBox.Color, Colors.MediumVioletRed, c => AnimatedBox.Color = c, 600, Easing.SinInOut, token);
            await AnimatedBox.ColorTo(AnimatedBox.Color, _initialColor, c => AnimatedBox.Color = c, 600, Easing.SinInOut, token);
            SetStatus("Color shift done");
        }
        catch (OperationCanceledException)
        {
            SetStatus("Color shift canceled");
        }
    }

    private async void OnRainbowLoopClicked(object sender, EventArgs e)
    {
        if (_rainbow)
        {
            _rainbow = false;
            SetStatus("Rainbow stop requested");
            return;
        }
        PrepareCts();
        _rainbow = true;
        SetStatus("Rainbow loop start");
        var colors = new[]
        {
            Colors.Red, Colors.Orange, Colors.Yellow, Colors.LawnGreen,
            Colors.DodgerBlue, Colors.MediumPurple, Colors.HotPink
        };
        try
        {
            int idx = 0;
            while (_rainbow)
            {
                var next = colors[(idx + 1) % colors.Length];
                await AnimatedBox.ColorTo(AnimatedBox.Color, next, c => AnimatedBox.Color = c, 500, Easing.Linear, _cts!.Token);
                idx++;
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        finally
        {
            SetStatus("Rainbow loop ended");
            _rainbow = false;
        }
    }

    private async void OnParallelComboClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("Parallel combo");
        var t1 = AnimatedBox.TranslateTo(120, -40, 600, Easing.SinOut);
        var t2 = AnimatedBox.RotateTo(270, 600, Easing.CubicInOut);
        var t3 = AnimatedBox.ScaleTo(1.4, 600, Easing.CubicOut);
        await Task.WhenAll(t1, t2, t3);
        await AnimatedBox.ScaleTo(1.0, 300);
        await AnimatedBox.RotateTo(0, 300);
        await AnimatedBox.TranslateTo(0, 0, 300);
        SetStatus("Parallel done");
    }

    private async void OnSequenceComboClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("Sequence combo");
        await AnimatedBox.TranslateTo(0, -80, 400, Easing.CubicOut);
        await AnimatedBox.ScaleTo(0.5, 300, Easing.SinInOut);
        await AnimatedBox.RotateTo(180, 400, Easing.SinInOut);
        await AnimatedBox.RotateTo(0, 300);
        await AnimatedBox.ScaleTo(1.0, 300);
        await AnimatedBox.TranslateTo(0, 0, 400, Easing.BounceOut);
        SetStatus("Sequence done");
    }

    private async void OnBreathStartClicked(object sender, EventArgs e)
    {
        if (_breathing)
        {
            SetStatus("Already breathing");
            return;
        }
        _breathing = true;
        SetStatus("Breathing loop start");
        _ = Task.Run(async () =>
        {
            while (_breathing)
            {
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await AnimatedBox.ScaleTo(1.15, 800, Easing.SinInOut);
                        await AnimatedBox.ScaleTo(1.00, 800, Easing.SinInOut);
                    });
                }
                catch { }
            }
        });
        _ = Task.Run(async () =>
        {
            while (_breathing)
            {
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await AnimatedBox.FadeTo(0.7, 800, Easing.SinInOut);
                        await AnimatedBox.FadeTo(1.0, 800, Easing.SinInOut);
                    });
                }
                catch { }
            }
        });
    }

    private void OnBreathStopClicked(object sender, EventArgs e)
    {
        _breathing = false;
        SetStatus("Breathing loop stop requested");
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        _cts?.Cancel();
        _rainbow = false;
        SetStatus("Cancel requested");
    }

    private async void OnResetClicked(object sender, EventArgs e)
    {
        OnCancelClicked(sender, e);
        _breathing = false;
        _rainbow = false;
        AnimatedBox.AbortAnimation("BounceMove");
        AnimatedBox.AbortAnimation("ColorAnim");
        AnimatedBox.AbortAnimation("Chain");
        AnimatedBox.Rotation = 0;
        AnimatedBox.Scale = 1;
        AnimatedBox.TranslationX = 0;
        AnimatedBox.TranslationY = 0;
        AnimatedBox.Opacity = 1;
        AnimatedBox.Color = _initialColor;
        await Task.Delay(50);
        SetStatus("Reset");
    }

    private async void OnFlipYClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("FlipY via SkewY");
        //await AnimatedBox.RelRotateTo();
        SetStatus("FlipY done");
    }

    private async void OnFlipXClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("FlipX via SkewX");
        //await AnimatedBox.RelRotateXSkewAsync();
        SetStatus("FlipX done");
    }

    private async void OnSpringMoveClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("Spring move");
        double target = 150;
        // 擬似バネ: 位置 + 縮小/拡大
        await Task.WhenAll(
            AnimatedBox.TranslateTo(target, 0, 500, Easing.SinOut),
            AnimatedBox.ScaleTo(0.85, 300, Easing.CubicOut)
        );
        await AnimatedBox.ScaleTo(1.05, 250, Easing.CubicIn);
        await AnimatedBox.ScaleTo(1.0, 200);
        await AnimatedBox.TranslateTo(0, 0, 400, Easing.SpringOut);
        SetStatus("Spring move done");
    }

    private async void OnElasticScaleClicked(object sender, EventArgs e)
    {
        PrepareCts();
        SetStatus("Elastic scale");
        await AnimatedBox.ScaleTo(1.6, 600, Easing.SpringOut);
        await AnimatedBox.ScaleTo(1.0, 500, Easing.SpringIn);
        SetStatus("Elastic scale done");
    }

    private async void OnChainExampleClicked(object sender, EventArgs e)
    {
        var token = PrepareCts();
        SetStatus("Chain start");
        try
        {
            await AnimatedBox.ScaleTo(1.3, 250).ContinueWith(async t =>
            {
                token.ThrowIfCancellationRequested();
                await AnimatedBox.RotateTo(90, 250);
                await AnimatedBox.RotateTo(180, 250);
            }).Unwrap().ContinueWith(async t =>
            {
                token.ThrowIfCancellationRequested();
                await AnimatedBox.TranslateTo(100, -60, 400, Easing.SinOut);
            }).Unwrap();
            await AnimatedBox.ColorTo(AnimatedBox.Color, Colors.Gold, c => AnimatedBox.Color = c, 500, Easing.CubicInOut, token);
            await AnimatedBox.ScaleTo(1.0, 250);
            await AnimatedBox.RotateTo(0, 250);
            await AnimatedBox.TranslateTo(0, 0, 300);
            await AnimatedBox.ColorTo(AnimatedBox.Color, _initialColor, c => AnimatedBox.Color = c, 400, Easing.Linear, token);
            SetStatus("Chain done");
        }
        catch (OperationCanceledException)
        {
            SetStatus("Chain canceled");
        }
    }

    private void OnStressClicked(object sender, EventArgs e)
    {
        SetStatus("Stress: launching 5 fire-and-forget animations");
        for (int i = 0; i < 5; i++)
        {
            _ = AnimatedBox.RelRotateTo(72, 500 + (uint)(i * 80));
        }
    }
}

public static class AnimationExtensions
{
    // Color アニメーション (変更なし)
    public static Task<bool> ColorTo(this VisualElement self, Color fromColor, Color toColor,
        Action<Color> callback, uint length = 250, Easing? easing = null, CancellationToken? token = null)
    {
        easing ??= Easing.Linear;
        var tcs = new TaskCompletionSource<bool>();

        var animation = new Animation(v =>
        {
            //if (token is { IsCancellationRequested: true })
            //{
            //    animation!.Abort();
            //    if (!tcs.Task.IsCompleted)
            //        tcs.TrySetCanceled(token.Value);
            //    return;
            //}
            var r = fromColor.Red + (toColor.Red - fromColor.Red) * v;
            var g = fromColor.Green + (toColor.Green - fromColor.Green) * v;
            var b = fromColor.Blue + (toColor.Blue - fromColor.Blue) * v;
            var a = fromColor.Alpha + (toColor.Alpha - fromColor.Alpha) * v;
            callback(new Color((float)r, (float)g, (float)b, (float)a));
        });

        animation.Commit(self, "ColorAnim", 16, length, easing,
            (v, c) =>
            {
                if (c)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(true);
            });

        if (token.HasValue)
        {
            token.Value.Register(() =>
            {
                self.AbortAnimation("ColorAnim");
                if (!tcs.Task.IsCompleted)
                    tcs.TrySetCanceled(token.Value);
            });
        }

        return tcs.Task;
    }

    // Flip (水平＝Y軸回転 / 垂直＝X軸回転)
    // BoxView は裏側も同じ見た目なので 0→180→360 などで戻す。
    public static async Task FlipYAsync(this VisualElement self, uint duration = 500, Easing? easing = null)
    {
        easing ??= Easing.CubicInOut;
        // 途中で少し奥行き感っぽく縮める（擬似的）
        await Task.WhenAll(
            self.RotateYTo(90, duration / 2, easing),
            ScalePulse(self, 1.0, 0.85, duration / 2, easing)
        );
        await Task.WhenAll(
            self.RotateYTo(180, duration / 2, easing),
            ScalePulse(self, 0.85, 1.0, duration / 2, easing)
        );
        // 連続で戻すなら 360 まで回して 0 に正規化
        await Task.WhenAll(
            self.RotateYTo(360, duration, easing),
            ScalePulse(self, 1.0, 0.9, duration / 2, easing, reverse: true)
        );
        self.RotationY = 0;
    }

    public static async Task FlipXAsync(this VisualElement self, uint duration = 500, Easing? easing = null)
    {
        easing ??= Easing.CubicInOut;
        await Task.WhenAll(
            self.RotateXTo(90, duration / 2, easing),
            ScalePulse(self, 1.0, 0.85, duration / 2, easing)
        );
        await Task.WhenAll(
            self.RotateXTo(180, duration / 2, easing),
            ScalePulse(self, 0.85, 1.0, duration / 2, easing)
        );
        await Task.WhenAll(
            self.RotateXTo(360, duration, easing),
            ScalePulse(self, 1.0, 0.9, duration / 2, easing, reverse: true)
        );
        self.RotationX = 0;
    }

    private static async Task ScalePulse(VisualElement v, double from, double to, uint duration, Easing easing, bool reverse = false)
    {
        if (!reverse)
        {
            v.Scale = from;
            await v.ScaleTo(to, duration, easing);
        }
        else
        {
            await v.ScaleTo(from, duration, easing);
        }
    }
}