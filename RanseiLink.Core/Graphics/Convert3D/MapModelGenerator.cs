namespace RanseiLink.Core.Graphics;

public class MapModelGenerator : IModelGenerator
{
    public void Generate(List<Group> groups, NSMDL.Model model)
    {
        var state = new MapModelGeneratorState(model);
        state.Process(groups);
    }
}

public class GimmickModelGenerator : IModelGenerator
{
    public void Generate(List<Group> groups, NSMDL.Model model)
    {
        var state = new GimmickFlagModelGeneratorState(model);
        state.Process(groups);
    }
}


public class MapModelGeneratorState : ModelGeneratorState
{
    public MapModelGeneratorState(NSMDL.Model model) : base(model)
    {
    }

    public override void Process(List<Group> groups)
    {
        // root set
        NEW_POLYMESH("set-" + model.Name.ToLowerInvariant());
        MTX_MULT_STORE(0, PeekStack(), 0, 0);
        PushStack(0);

        // first and only child of set
        NEW_POLYMESH(model.Name.ToLowerInvariant());
        MTX_MULT_STORE(1, PeekStack(), 0, 1);
        PushStack(1);

        // contents of child
        bool first = true;
        foreach (var group in groups)
        {
            ProcessGroup(group, first);
            first = false;
        }

        END();
    }

    private void ProcessGroup(Group group, bool first)
    {
        int polymeshId = model.Polymeshes.Count;
        int materialId = model.Materials.FindIndex(x => x.Name == group.MaterialName);

        // create polymesh data
        NEW_POLYMESH($"polymsh{polymeshId}");

        // do the commands

        if (first)
        {
            MTX_MULT(polymeshId, PeekStack(), 0);
        }
        else
        {
            MTX_MULT_RESTORE(polymeshId, PeekStack(), 0, 1);
        }

        VISIBILITY(polymeshId, true);

        MTX_SCALE_UP();

        BIND_MATERIAL(materialId);

        DRAW_MESH(group);

        MTX_SCALE_DOWN();
    }
}



// Ones that are some other nonsense:
// 41 (crescent), 70 (water wheel), 71 (water gate), 72 (vine), 73 (wood trap pen 2 sides),
// 74 (wood trap pen 4 sides), 75 (water feature), 76 (stone door), 77 (rolling boulder),
// 78 (spinny gear), 79 (literally just a flat dirt hole wut), 80 (stone step with rainbow on top),
// 81 (flat crackable ice), 82 (floating ice), 83 (floaty stone platform), 84 (viperia barrier),
// 85 (terrera elevator), 86 (big ruins pillar), 87 (big ruins rubble), 88 (3d log), 
// 89 (viperia trapdoor), 90 (jump ramp), 91 (hot spring with steam), 92 (empty hot spring), 
// 93 (roulett box), 94 (water bucket), 95 (empty water bucket), 96 (flat oasis, what's different about this and the other one?)
// 97 (empty oasis)

/// <summary>
/// For gimmicks like OBJ000_00 (banner), 6 (hole), 7 (scarecrow), 9 (fire), 11 (fountain)
/// 12 (emerald), 13 (blossom tree), 15 (strange flower), 16 (icicle), 18 (unused flower),
/// 22 (orange tree), 23 (cube), 25 (ponigiri ball), 29 (zoroark box), 31 (metal spinny barrier),
/// 32 (grabby hook), 33 (blank banner), 34 (palm tree), 35 (ruins), 36 (ruins2), 38 (treasure box),
/// 39 (treasure chest), 44 (yellow orb**), 45 (purple orb**), 46 (purple spark**),
/// 47 (warp), 48 (blue wisp), 49 (purple wisp), 59 (metal spinny barrier 2), 
/// 
/// ** different scale (I wasn't paying too close attention, may have missed some)
/// 
/// </summary>
public class GimmickFlagModelGeneratorState : ModelGeneratorState
{
    public GimmickFlagModelGeneratorState(NSMDL.Model model) : base(model)
    {
    }

    public override void Process(List<Group> groups)
    {
        // no root! Is because there's only one child?

        // contents of child
        bool first = true;
        foreach (var group in groups)
        {
            ProcessGroup(group, first);
            first = false;
        }

        END();
    }

    private void ProcessGroup(Group group, bool first)
    {
        int polymeshId = model.Polymeshes.Count;
        int materialId = model.Materials.FindIndex(x => x.Name == group.MaterialName);

        // create polymesh data
        NEW_POLYMESH(first ? "normal" : "reverse");

        // do the commands

        if (first)
        {
            MTX_MULT(polymeshId, PeekStack(), 0);
        }
        else
        {
            MTX_MULT_RESTORE(polymeshId, PeekStack(), 0, 1);
        }


        VISIBILITY(polymeshId, true);

        UNKNOWN_7(polymeshId);

        MTX_SCALE_UP();

        BIND_MATERIAL(materialId);

        DRAW_MESH(group);

        MTX_SCALE_DOWN();
    }
}

/// <summary>
/// (this seems to be for 2d sprites that sit on the ground)
/// For gimmicks like OBJ005_00 (button), 8 (trap), 10 (hotspring), 14 (grass),
/// 19 (2d waterbucked), 21 (oasis), 24 (weird blue square), 43 (extinguished fire), 
/// 51/2/3/4 (grass), 55/6/7/8 (button), 
/// (this has different UpScale too, ffs)
/// </summary>
public class GimmickButtonModelGeneratorState : ModelGeneratorState
{
    public GimmickButtonModelGeneratorState(NSMDL.Model model) : base(model)
    {
    }

    public override void Process(List<Group> groups)
    {
        // no root! Is because there's only one child?

        // contents of child
        bool first = true;
        foreach (var group in groups)
        {
            ProcessGroup(group, first);
            first = false;
        }

        END();
    }

    private void ProcessGroup(Group group, bool first)
    {
        int polymeshId = model.Polymeshes.Count;
        int materialId = model.Materials.FindIndex(x => x.Name == group.MaterialName);

        // create polymesh data
        NEW_POLYMESH(first ? "normal" : "reverse");

        // do the commands

        if (first)
        {
            MTX_MULT(polymeshId, PeekStack(), 0);
        }
        else
        {
            MTX_MULT_RESTORE(polymeshId, PeekStack(), 0, 1);
        }


        VISIBILITY(polymeshId, true);

        UNKNOWN_8(polymeshId);

        MTX_SCALE_UP();

        BIND_MATERIAL(materialId);

        DRAW_MESH(group);

        MTX_SCALE_DOWN();
    }
}

/// <summary>
/// (I think this might be, ones that have different sprite based on direction they face)
/// For gimmicks like OBJ001_00 (tree), 2 (tree2), 3 (tree3), 4 (rock), 17 (bell),
/// 20 (doll), 26 (crystal), 27 (misdreavus statue), 30 (camera), 37 (2d log), 
/// 40 (red arrow), 42 (ice rock), 
/// </summary>
public class GimmickModelGeneratorState : ModelGeneratorState
{
    public GimmickModelGeneratorState(NSMDL.Model model) : base(model)
    {
    }

    public override void Process(List<Group> groups)
    {
        // root set
        NEW_POLYMESH("unit");
        MTX_MULT_STORE(0, PeekStack(), 0, 0);
        PushStack(0);

        // contents of child
        bool first = true;
        foreach (var group in groups)
        {
            ProcessGroup(group, first);
            first = false;
        }

        END();
    }

    private void ProcessGroup(Group group, bool first)
    {
        int polymeshId = model.Polymeshes.Count;
        int materialId = model.Materials.FindIndex(x => x.Name == group.MaterialName);

        // create polymesh data
        NEW_POLYMESH(first ? "normal" : "reverse");

        // do the commands

        if (first)
        {
            MTX_MULT(polymeshId, PeekStack(), 0);
        }
        else
        {
            MTX_MULT_RESTORE(polymeshId, PeekStack(), 0, 1);
        }


        VISIBILITY(polymeshId, true);

        UNKNOWN_7(polymeshId);
        
        MTX_SCALE_UP();

        BIND_MATERIAL(materialId);

        DRAW_MESH(group);

        MTX_SCALE_DOWN();
    }
}