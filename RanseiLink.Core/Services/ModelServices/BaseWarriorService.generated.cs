﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;


public partial interface IBaseWarriorService : IModelService<BaseWarrior> {}

public partial class BaseWarriorService : BaseDataModelService<BaseWarrior>, IBaseWarriorService
{
    public static BaseWarriorService Load(string dataFile) => new BaseWarriorService(dataFile);
    private BaseWarriorService(string dataFile) : base(dataFile, 0, 251, () => new BaseWarrior(), 252) {}

    public BaseWarriorService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BaseBushouRomPath)) {}

    public BaseWarrior Retrieve(WarriorId id) => Retrieve((int)id);

}