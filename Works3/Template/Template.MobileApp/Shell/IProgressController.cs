namespace Template.MobileApp.Shell;

public interface IMessageProgress
{
    void Update(string value);
}

public interface IRateProgress
{
    void Update(double value);
}

public interface IProgressController
{
    void Default();

    void Indicator();

    IMessageProgress Message();

    IRateProgress Rate();
}
