using System.Collections.Generic;

namespace RanseiLink.Core.Graphics;

public class MapModelGenerator : IModelGenerator
{
    public void Generate(List<Group> groups, NSMDL.Model model)
    {
        var state = new MapModelGeneratorState(model);
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
        MTX_MULT_STORE(0, parentPolymeshStack.Peek(), 0, 0);
        parentPolymeshStack.Push(0);

        // first and only child of set
        NEW_POLYMESH(model.Name.ToLowerInvariant());
        MTX_MULT_STORE(1, parentPolymeshStack.Peek(), 0, 1);
        parentPolymeshStack.Push(1);

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
            MTX_MULT(polymeshId, parentPolymeshStack.Peek(), 0);
        }
        else
        {
            MTX_MULT_RESTORE(polymeshId, parentPolymeshStack.Peek(), 0, 1);
        }

        VISIBILITY(polymeshId, true);

        MTX_SCALE_UP();

        BIND_MATERIAL(materialId);

        DRAW_MESH(group);

        MTX_SCALE_DOWN();
    }
}