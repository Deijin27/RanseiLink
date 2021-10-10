using Core.Models.Interfaces;

namespace RanseiWpf.ViewModels
{
    public class BaseWarriorViewModel : ViewModelBase, IViewModelForModel<IBaseWarrior>
    {
        public IBaseWarrior Model { get; set; }

        public uint WarriorName
        {
            get => Model.WarriorName;
            set => RaiseAndSetIfChanged(Model.WarriorName, value, v => Model.WarriorName = v);
        }
    }
}
