namespace WorkSmartMaui.Shell;

// TODO DI based
public sealed class ProgressResolver
{
    private static readonly ProgressConfig defaultConfig = new();

    private static ProgressView? progressView;

    private static ProgressController? progressController;

    private static MessageProgressStrategy? messageStrategy;
    private static RateProgressStrategy? rateStrategy;
    private static CircleProgressStrategy? circleStrategy;

    public static void Configure(Action<ProgressConfig> action)
    {
        action(defaultConfig);
    }

    public static IProgressView ResolveView() => ResolveViewInternal();

    public static IProgressController ResolveController() => ResolveControllerInternal();

    //--------------------------------------------------------------------------------

    private static IWindow ResolveWindowInternal() =>
        Application.Current!.Windows[0];

    private static ProgressView ResolveViewInternal()
    {
        progressView ??= new(defaultConfig, null);
        return progressView;
    }

    private static ProgressController ResolveControllerInternal()
    {
        progressController ??= new ProgressController(ResolveViewInternal());
        return progressController;
    }

    private static MessageProgressStrategy ResolveMessageProgressStrategyInternal()
    {
        messageStrategy ??= new MessageProgressStrategy(defaultConfig);
        return messageStrategy;
    }

    private static RateProgressStrategy ResolveRateProgressStrategyInternal()
    {
        rateStrategy ??= new RateProgressStrategy(defaultConfig);
        return rateStrategy;
    }

    private static CircleProgressStrategy ResolveCircleStrategyInternal()
    {
        circleStrategy ??= new CircleProgressStrategy();
        return circleStrategy;
    }

    //--------------------------------------------------------------------------------

    private sealed class ProgressView : IProgressView, IProgressDrawer, IProgressStrategyCallback
    {
        private readonly ProgressConfig config;

        private readonly IProgressStrategy? defaultStrategy;

        private ProgressOverlay? overlay;

        private IProgressStrategy? strategy;

        private bool visible;

        public ProgressView(ProgressConfig config, IProgressStrategy? defaultStrategy)
        {
            this.config = config;
            this.defaultStrategy = defaultStrategy;
        }

        private ProgressOverlay GetOverlay()
        {
            overlay ??= new ProgressOverlay(ResolveWindowInternal(), this);
            return overlay;
        }

        public void Show()
        {
            if (visible)
            {
                return;
            }

            var o = GetOverlay();
            if (o.Window.AddOverlay(o))
            {
                visible = true;
            }

            // Specific
            strategy = defaultStrategy;
            strategy?.Attach(this);
        }

        public void Hide()
        {
            if (!visible)
            {
                return;
            }

            var o = GetOverlay();
            if (o.Window.RemoveOverlay(o))
            {
                visible = false;
            }

            // Specific
            strategy?.Detach();
            strategy = null;
        }

        // Specific
        public void Update(IProgressStrategy? value)
        {
            strategy?.Detach();
            strategy = value;

            value?.Attach(this);

            GetOverlay().Invalidate();
        }

        // Specific
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = config.BackgroundColor;
            canvas.FillRectangle(dirtyRect);

            strategy?.Draw(canvas, dirtyRect);
        }

        public void Invalidate()
        {
            GetOverlay().Invalidate();
        }
    }

    //--------------------------------------------------------------------------------

    private sealed class ProgressController : IProgressController
    {
        private readonly ProgressView view;

        public ProgressController(ProgressView view)
        {
            this.view = view;
        }

        public void Clear()
        {
            view.Update(null);
        }

        public IMessageProgress Message()
        {
            // Delay
            var strategy = ResolveMessageProgressStrategyInternal();
            view.Update(strategy);
            return strategy;
        }

        public IRateProgress Rate()
        {
            // Delay
            var strategy = ResolveRateProgressStrategyInternal();
            view.Update(strategy);
            return strategy;
        }

        public void Circle()
        {
            // Delay
            view.Update(ResolveCircleStrategyInternal());
        }
    }
}

