namespace ViewSample.FormsApp.Usecase
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using Rester;

    using ViewSample.FormsApp.Components.Device;
    using ViewSample.FormsApp.Components.Dialog;
    using ViewSample.FormsApp.Models;
    using ViewSample.FormsApp.Services;

    public class NetworkOperator
    {
        private readonly IApplicationDialog dialogs;

        private readonly IDeviceManager deviceManager;

        private readonly NetworkService networkService;

        public NetworkOperator(
            IApplicationDialog dialogs,
            IDeviceManager deviceManager,
            NetworkService networkService)
        {
            this.dialogs = dialogs;
            this.deviceManager = deviceManager;
            this.networkService = networkService;
        }

        public async ValueTask<IResult<T>> ExecuteVerbose<T>(Func<NetworkService, ValueTask<IRestResponse<T>>> func)
        {
            return await Execute(func, true);
        }

        public async ValueTask<IResult<T>> Execute<T>(Func<NetworkService, ValueTask<IRestResponse<T>>> func)
        {
            return await Execute(func, false);
        }

        private async ValueTask<IResult<T>> Execute<T>(Func<NetworkService, ValueTask<IRestResponse<T>>> func, bool verbose)
        {
            while (true)
            {
                if (!deviceManager.GetNetworkState().IsConnected())
                {
                    if (verbose)
                    {
                        await dialogs.Information("Network is not connected.");
                    }
                    return Result.Failed<T>();
                }

                IRestResponse<T> response;
                using (dialogs.Loading("Connecting"))
                {
                    response = await func(networkService);
                }

                switch (response.RestResult)
                {
                    case RestResult.Success:
                        return Result.Success(response.Content);
                    case RestResult.Cancel:
                        if (!verbose)
                        {
                            return Result.Failed<T>();
                        }
                        else if (!await dialogs.Confirm("Canceled.\r\nRetry ?"))
                        {
                            return Result.Failed<T>();
                        }
                        break;
                    case RestResult.RequestError:
                    case RestResult.HttpError:
                        if (verbose)
                        {
                            var message = new StringBuilder();
                            message.AppendLine("Network error.");
                            if (response.StatusCode > 0)
                            {
                                message.AppendLine($"StatusCode={(int)response.StatusCode}");
                            }
                            message.AppendLine("Retry ?");
                            if (!await dialogs.Confirm(message.ToString()))
                            {
                                return Result.Failed<T>();
                            }
                        }
                        else
                        {
                            return Result.Failed<T>();
                        }
                        break;
                    default:
                        if (verbose)
                        {
                            await dialogs.Information("Unknown error.");
                        }
                        return Result.Failed<T>();
                }
            }
        }

        public async ValueTask<bool> ExecuteVerbose(Func<NetworkService, ValueTask<IRestResponse>> func)
        {
            return await Execute(func, true);
        }

        public async ValueTask<bool> Execute(Func<NetworkService, ValueTask<IRestResponse>> func)
        {
            return await Execute(func, false);
        }

        private async ValueTask<bool> Execute(Func<NetworkService, ValueTask<IRestResponse>> func, bool verbose)
        {
            while (true)
            {
                if (!deviceManager.GetNetworkState().IsConnected())
                {
                    if (verbose)
                    {
                        await dialogs.Information("Network is not connected.");
                    }
                    return false;
                }

                IRestResponse response;
                using (dialogs.Loading("Connecting"))
                {
                    response = await func(networkService);
                }

                switch (response.RestResult)
                {
                    case RestResult.Success:
                        return true;
                    case RestResult.Cancel:
                        if (!verbose)
                        {
                            return false;
                        }
                        else if (!await dialogs.Confirm("Canceled.\r\nRetry ?"))
                        {
                            return false;
                        }
                        break;
                    case RestResult.RequestError:
                    case RestResult.HttpError:
                        if (verbose)
                        {
                            var message = new StringBuilder();
                            message.AppendLine("Network error.");
                            if (response.StatusCode > 0)
                            {
                                message.AppendLine($"StatusCode={(int)response.StatusCode}");
                            }
                            message.AppendLine("Retry ?");
                            if (!await dialogs.Confirm(message.ToString()))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    default:
                        if (verbose)
                        {
                            await dialogs.Information("Unknown error.");
                        }
                        return false;
                }
            }
        }
    }
}
