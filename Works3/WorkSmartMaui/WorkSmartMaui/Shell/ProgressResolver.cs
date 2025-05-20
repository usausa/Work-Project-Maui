namespace WorkSmartMaui.Shell;

// TODO DI based
public sealed class ProgressResolver
{
    private static readonly ProgressConfig defaultConfig = new();

    private static ProgressView? progressView;

    private static ProgressController? progressController;

    private static IProgressStrategy? circleStrategy;

    public static void Configure(Action<ProgressConfig> action)
    {
        action(defaultConfig);
    }

    private static IWindow ResolveWindow() =>
        Application.Current!.Windows[0];

    public static IProgressView ResolveView()
    {
        progressView ??= new(defaultConfig, null);
        return progressView;
    }

    public static IProgressController ResolveController()
    {
        progressController ??= new ProgressController((ProgressView)ResolveView());
        return progressController;
    }

    private static IProgressStrategy ResolveCircleStrategy()
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
            overlay ??= new ProgressOverlay(ResolveWindow(), this);
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

        public void Circle()
        {
            // Delay
            view.Update(ResolveCircleStrategy());
        }
    }
}

