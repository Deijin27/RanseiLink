#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class WarriorSkillViewModel : ViewModelBase, IBigViewModel
{
    private readonly ICachedMsgBlockService _msgService;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;

    public WarriorSkillViewModel(
        ICachedMsgBlockService msgService, 
        IBaseWarriorService baseWarriorService,
        ICachedSpriteProvider cachedSpriteProvider,
        IJumpService jumpService)
    {
        _msgService = msgService;
        _baseWarriorService = baseWarriorService;
        _cachedSpriteProvider = cachedSpriteProvider;

        _selectWarriorCommand = new RelayCommand<WarriorMiniViewModel>(wa => { if (wa != null) jumpService.JumpTo(WarriorWorkspaceModule.Id, wa.Id); });
    }

    public void SetModel(WarriorSkillId id, WarriorSkill model)
    {
        _id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public void SetModel(int id, object model)
    {
        SetModel((WarriorSkillId)id, (WarriorSkill)model);
    }

    private readonly ICommand _selectWarriorCommand;

    public List<WarriorMiniViewModel> WarriorsWithSkill
    {
        get
        {
            var list = new List<WarriorMiniViewModel>();
            foreach (var id in _baseWarriorService.ValidIds())
            {
                var warrior = _baseWarriorService.Retrieve(id);
                if (warrior.Skill == _id)
                {
                    list.Add(new WarriorMiniViewModel(_cachedSpriteProvider, _baseWarriorService, warrior, id, _selectWarriorCommand));
                }
            }
            return list;
        }
    }
}