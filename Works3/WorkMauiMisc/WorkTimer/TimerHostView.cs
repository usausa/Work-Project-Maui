namespace WorkTimer;

using System;
using System.Threading.Tasks;

internal class TimerHostView : ContentView
{
    private CancellationTokenSource? _cts;
    private int _count;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
    private readonly Label _label;

    public TimerHostView()
    {
        _label = new Label
        {
            Text = "0",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        Content = new Frame
        {
            Content = _label,
            Padding = 10,
            CornerRadius = 8,
            BackgroundColor = Colors.LightBlue
        };
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null)
        {
            // 表示開始：タイマーをスタート
            StartPeriodicTimer();
        }
        else
        {
            // 画面ツリーから外れた：タイマーをキャンセル
            StopPeriodicTimer();
        }
    }

    private void StartPeriodicTimer()
    {
        // すでに動作中なら何もしない
        if (_cts != null && !_cts.IsCancellationRequested)
            return;

        _cts = new CancellationTokenSource();
        _count = 0;

        // バックグラウンドで非同期ループを開始
        _ = RunTimerAsync(_cts.Token);
    }

    private async Task RunTimerAsync(CancellationToken ct)
    {
        try
        {
            using var timer = new PeriodicTimer(_interval);

            while (await timer.WaitForNextTickAsync(ct))
            {
                // キャンセルされていれば抜ける
                if (ct.IsCancellationRequested)
                    break;

                _count++;
                // UIスレッドへ戻して更新
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _label.Text = _count.ToString();
                });
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセルが発生した場合はここに来る
        }
    }

    private void StopPeriodicTimer()
    {
        if (_cts == null)
            return;

        // キャンセルを通知して解放
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
}
