using System.Numerics;

namespace RanseiLink.Core.Graphics;

public interface IModelGenerator
{
    public void Generate(List<Group> groups, NSMDL.Model model);
}

public abstract class ModelGeneratorState
{
    public ModelGeneratorState(NSMDL.Model model)
    {
        this.model = model;
        gpu = new InverseGpuState(model.Materials[0]);
        parentPolymeshStack = new Stack<int>();
        parentPolymeshStack.Push(0);
    }

    protected InverseGpuState gpu;
    protected NSMDL.Model model;
    protected Stack<int> parentPolymeshStack;

    public abstract void Process(List<Group> groups);

    protected void NOP()
    {
        model.RenderCommands.Add(RenderCommandGenerator.NOP());
    }

    protected void END()
    {
        model.RenderCommands.Add(RenderCommandGenerator.END());
    }

    protected void VISIBILITY(int polymeshId, bool isVisible)
    {
        model.RenderCommands.Add(RenderCommandGenerator.VISIBILITY(polymeshId, isVisible));
    }

    protected void MTX_RESTORE(int stackIndex)
    {
        model.RenderCommands.Add(RenderCommandGenerator.MTX_RESTORE(stackIndex));
    }

    protected void BIND_MATERIAL(int materialIndex)
    {
        model.RenderCommands.Add(RenderCommandGenerator.BIND_MATERIAL(materialIndex));
        gpu.CurrentMaterial = model.Materials[materialIndex];
    }

    protected void DRAW_MESH(Group group)
    {
        model.RenderCommands.Add(RenderCommandGenerator.DRAW_MESH(model.Polygons.Count));
        var dl = PolygonGenerator.Generate(group, gpu);
        model.Polygons.Add(new NSMDL.Model.Polygon
        (
            itemTag: 0,
            flag: NSMDL.Model.SHPFLAG.USE_NORMAL | NSMDL.Model.SHPFLAG.USE_TEXCOORD,
            name: group.Name,
            commands: dl
        ));
    }

    protected void MTX_MULT(int polymeshId, int parentId, int unknown)
    {
        model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT(polymeshId, parentId, unknown));
        gpu.MultiplyMatrix(model.Polymeshes[polymeshId].TRSMatrix);
    }

    protected void MTX_MULT_STORE(int polymeshId, int parentId, int unknown, int storeIndex)
    {
        model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT_STORE(polymeshId, parentId, unknown, storeIndex));
        gpu.MultiplyMatrix(model.Polymeshes[polymeshId].TRSMatrix);
    }

    protected void MTX_MULT_RESTORE(int polymeshId, int parentId, int unknown, int restoreIndex)
    {
        model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT_STORE(polymeshId, parentId, unknown, restoreIndex));
        gpu.MultiplyMatrix(model.Polymeshes[polymeshId].TRSMatrix);
    }

    protected void MTX_MULT_STORE_RESTORE(int polymeshId, int parentId, int unknown, int storeIndex, int restoreIndex)
    {
        model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT_STORE_RESTORE(polymeshId, parentId, unknown, storeIndex, restoreIndex));
        gpu.MultiplyMatrix(model.Polymeshes[polymeshId].TRSMatrix);
    }

    protected void MTX_SCALE_DOWN()
    {
        model.RenderCommands.Add(RenderCommandGenerator.MTX_SCALE_DOWN());
        gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.DownScale));
    }

    protected void MTX_SCALE_UP()
    {
        model.RenderCommands.Add(RenderCommandGenerator.MTX_SCALE_UP());
        gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.UpScale));
    }

    protected void NEW_POLYMESH(string name)
    {
        model.Polymeshes.Add(new NSMDL.Model.PolymeshData(name));
    } 
}