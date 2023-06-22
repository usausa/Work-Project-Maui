namespace WorkControl.Controls.Handlers;

public partial class SimpleViewHandler
{
    public static IPropertyMapper<ISimpleView, SimpleViewHandler> Mapper = new PropertyMapper<ISimpleView, SimpleViewHandler>(ViewMapper)
    {
        [nameof(ISimpleView.Color)] = MapColor,
    };

    public static CommandMapper<ISimpleView, SimpleViewHandler> CommandMapper = new(ViewCommandMapper)
    {
        [nameof(ISimpleView.PlatformCallRequested)] = MapPlatformCallRequested
    };

    public SimpleViewHandler()
        : base(Mapper, CommandMapper)
    {
    }
}
