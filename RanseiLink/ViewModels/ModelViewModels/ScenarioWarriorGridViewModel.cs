using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.ViewModels;

public class ScenarioWarriorGridItemsService : IModelDataService<ScenarioId, ScenarioWarriorViewModel[]>
{
    private readonly IEditorContext _editorContext;
    private readonly IScenarioWarriorService _scenarioWarriorService;
    private readonly ScenarioWarriorViewModelFactory _vmFactory;
    public ScenarioWarriorGridItemsService(IEditorContext context, ScenarioWarriorViewModelFactory vmFactory)
    {
        _scenarioWarriorService = context.DataService.ScenarioWarrior;
        _editorContext = context;
        _vmFactory = vmFactory;
    }

    //private IScenarioWarrior[] miniCache;

    public ScenarioWarriorViewModel[] Retrieve(ScenarioId id)
    {
        var list = new ScenarioWarriorViewModel[200]; 
        using var disposableService = _scenarioWarriorService.Disposable();

        for (int i = 0; i < 200; i++)
        {
            list[i] = _vmFactory(id, _editorContext, disposableService.Retrieve(id, i));
        }
        return list;
    }

    public void Save(ScenarioId id, ScenarioWarriorViewModel[] model)
    {
        //using var disposableService = _scenarioWarriorService.Disposable();

        //for (int i = 0; i < 200; i++)
        //{

        //}
    }
}
