using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

using Smart.Collections.Generic;
using Smart.Forms.ViewModels;

namespace WorkList
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<DataEntity> Items { get; } = new();

        public ICommand SelectCommand { get; }
        public ICommand RemoveCommand { get; }

        public MainPageViewModel()
        {
            Items.AddRange(Enumerable.Range(1, 20).Select(x => new DataEntity { Id = x, Name = $"Name-{x}"}));

            SelectCommand = MakeDelegateCommand<DataEntity>(x =>
            {
                Debug.WriteLine($"{x.Id} {x.Name}");
            });
            RemoveCommand = MakeDelegateCommand<DataEntity>(x =>
            {
                if (Items.Count > 1)
                {
                    Items.RemoveAt(1);
                }
            });
        }
    }
}
