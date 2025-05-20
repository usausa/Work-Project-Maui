namespace WorkSmartMaui.Shell;

public interface IProgressController
{
    void Clear();

    IMessageProgress Message();

    IRateProgress Rate();

    void Circle();
}
