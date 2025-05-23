namespace Template.MobileApp.Shell;

public sealed class ProgressController : IProgressController
{
    private readonly IProgressStrategyUpdate view;

    private readonly IndicatorProgressStrategy indicatorStrategy;

    private readonly MessageProgressStrategy messageStrategy;

    private readonly RateProgressStrategy rateStrategy;

    public ProgressController(
        IProgressStrategyUpdate view,
        IndicatorProgressStrategy indicatorStrategy,
        MessageProgressStrategy messageStrategy,
        RateProgressStrategy rateStrategy)
    {
        this.view = view;
        this.indicatorStrategy = indicatorStrategy;
        this.messageStrategy = messageStrategy;
        this.rateStrategy = rateStrategy;
    }

    void IProgressController.Default()
    {
        view.UpdateStrategy(null);
    }

    void IProgressController.Indicator()
    {
        view.UpdateStrategy(indicatorStrategy);
    }

    IMessageProgress IProgressController.Message()
    {
        var strategy = messageStrategy;
        view.UpdateStrategy(strategy);
        return strategy;
    }

    IRateProgress IProgressController.Rate()
    {
        var strategy = rateStrategy;
        view.UpdateStrategy(strategy);
        return strategy;
    }
}
