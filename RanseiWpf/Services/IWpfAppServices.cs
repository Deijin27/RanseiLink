using Core.Services;

namespace RanseiWpf.Services
{
    public interface IWpfAppServices
    {
        ICoreAppServices CoreServices { get; }
        IDialogService DialogService { get; }
    }
}
