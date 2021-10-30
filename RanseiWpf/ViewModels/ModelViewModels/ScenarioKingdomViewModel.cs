using Core;
using Core.Enums;
using Core.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RanseiWpf.ViewModels
{
    public class ScenarioKingdomItem : ViewModelBase
    {
        private readonly IScenarioKingdom _model;
        public ScenarioKingdomItem(IScenarioKingdom model, KingdomId kingdom)
        {
            Kingdom = kingdom;
            _model = model;
        }
        public KingdomId Kingdom { get; }
        
        public uint BattlesToUnlock
        {
            get => _model.GetBattlesToUnlock(Kingdom);
            set => RaiseAndSetIfChanged(BattlesToUnlock, value, v => _model.SetBattlesToUnlock(Kingdom, v));
        }
    }
    public class ScenarioKingdomViewModel : ViewModelBase, IViewModelForModel<IScenarioKingdom>
    {
        private IScenarioKingdom _model;
        public IScenarioKingdom Model
        {
            get => _model;
            set
            {
                _model = value;
                KingdomItems = EnumUtil.GetValues<KingdomId>().Select(i => new ScenarioKingdomItem(value, i)).ToList();
            }
        }


        private List<ScenarioKingdomItem> _KingdomItems;
        public List<ScenarioKingdomItem> KingdomItems
        {
            get => _KingdomItems;
            set => RaiseAndSetIfChanged(ref _KingdomItems, value);
        }
    }
}
