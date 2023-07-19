namespace Template.MobileApp.Usecase;

public class SampleUsecase
{
    private readonly NetworkOperator networkOperator;

    public SampleUsecase(
        NetworkOperator networkOperator)
    {
        this.networkOperator = networkOperator;
    }

    //--------------------------------------------------------------------------------
    // Simple
    //--------------------------------------------------------------------------------

    public ValueTask<IResult<ServerTimeResponse>> GetServerTimeAsync() =>
        networkOperator.ExecuteVerbose(static n => n.GetServerTimeAsync());

    //--------------------------------------------------------------------------------
    // Test
    //--------------------------------------------------------------------------------

    public ValueTask<IResult<object>> GetTestErrorAsync(int code) =>
        networkOperator.ExecuteVerbose(n => n.GetTestErrorAsync(code));

    public ValueTask<IResult<object>> GetTestDelayAsync(int timeout) =>
        networkOperator.ExecuteVerbose(n => n.GetTestDelayAsync(timeout));

    //--------------------------------------------------------------------------------
    // Data
    //--------------------------------------------------------------------------------

    public ValueTask<IResult<DataListResponse>> GetDataListAsync() =>
        networkOperator.ExecuteVerbose(static n => n.GetDataListAsync());

    //--------------------------------------------------------------------------------
    // Download/Upload
    //--------------------------------------------------------------------------------

    // TODO
}
