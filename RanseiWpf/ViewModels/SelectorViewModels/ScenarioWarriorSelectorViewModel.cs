using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;
using System;

namespace RanseiWpf.ViewModels
{
    public class ScenarioWarriorSelectorViewModel : ScenarioSelectorViewModelBase<IScenarioWarrior, ScenarioWarriorViewModel>
    {
        private readonly IModelDataService<ScenarioId, int, IScenarioWarrior> Service;
        public ScenarioWarriorSelectorViewModel(IDialogService dialogService, IModelDataService<ScenarioId, int, IScenarioWarrior> service, Func<ScenarioId, ScenarioWarriorViewModel> newVm) 
            : base(dialogService, newVm, 0, 199)
        {
            Service = service;
            Init();
        }

        protected override IScenarioWarrior RetrieveModel(ScenarioId scenario, uint index)
        {
            return Service.Retrieve(scenario, (int)index);
        }

        protected override void SaveModel(ScenarioId scenario, uint index, IScenarioWarrior model)
        {
            Service.Save(scenario, (int)index, model);
        }
    }
}
