using System.Threading.Tasks;

namespace WorkPopupLevel
{
    public interface ICustomDialog
    {
        ValueTask<bool> Confirm(string title, string message, string ok, string cancel);

        ValueTask Information(string title, string message, string ok);

        ValueTask<int> Select(int selected, string[] items);
    }
}
