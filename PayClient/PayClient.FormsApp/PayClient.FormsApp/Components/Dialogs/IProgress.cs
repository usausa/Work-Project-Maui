namespace PayClient.FormsApp.Components.Dialogs
{
    using System;

    public interface IProgress : IDisposable
    {
        void Update(int percent);
    }
}
