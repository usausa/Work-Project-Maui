namespace ViewSample.FormsApp.Usecase
{
    using System.Threading.Tasks;

    public class SampleUsecase
    {
        private readonly NetworkOperator networkOperator;

        public SampleUsecase(
            NetworkOperator networkOperator)
        {
            this.networkOperator = networkOperator;
        }

        public async ValueTask PostPingAsync()
        {
            // TODO
            await networkOperator.ExecuteVerbose(n => n.PostPingAsync());
        }
    }
}
