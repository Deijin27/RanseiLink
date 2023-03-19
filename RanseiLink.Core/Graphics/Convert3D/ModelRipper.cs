using System.Collections.Generic;
using System.Numerics;

namespace RanseiLink.Core.Graphics;

public class ModelRipper
{
    private ModelRipper(NSMDL.Model model)
    {
        gpu = new GpuState(model.Materials[0]);
        this.model = model;
    }

    public static List<Group> Rip(NSMDL.Model model)
    {
        var state = new ModelRipper(model);
        state.Process(model.RenderCommands);
        return state.groups;
    }

    private void Process(IEnumerable<RenderCommand> commands)
    {
        foreach (var command in commands)
        {
            ProcessCommand(command);
        }
    }

    private readonly NSMDL.Model model;
    private readonly List<Group> groups = new List<Group>();
    private readonly GpuState gpu;

    private void ProcessCommand(RenderCommand command)
    {
        switch (command.OpCode)
        {
            case RenderOpCode.NOP:
                break;

            case RenderOpCode.END:
                break;

            case RenderOpCode.VISIBILITY:
                break;

            case RenderOpCode.MTX_RESTORE:
                gpu.Restore(command.Parameters[0]);
                break;

            case RenderOpCode.BIND_MATERIAL:
                gpu.CurrentMaterial = model.Materials[command.Parameters[0]];
                break;

            case RenderOpCode.DRAW_MESH:
                var dl = model.Polygons[command.Parameters[0]];
                var g = PolygonRipper.Rip(dl.Commands, gpu);
                g.Name = dl.Name;
                g.MaterialName = gpu.CurrentMaterial.Name;
                groups.Add(g);
                break;

            case RenderOpCode.MTX_MULT:
                var polymshIdx = command.Parameters[0];
                //var parentId = command.Parameters[1];
                //var unknown = command.Parameters[2];

                int restoreIndex = -1;
                int storeIndex = -1;
                switch (command.Flags)
                {
                    case 0: // 3 params
                        break;
                    case 1: // 4 params
                        storeIndex = command.Parameters[3];
                        break;
                    case 2: // 4 params
                        restoreIndex = command.Parameters[3];
                        break;
                    case 3: // 5 params
                        storeIndex = command.Parameters[3];
                        restoreIndex = command.Parameters[4];
                        break;
                    default:
                        break;
                }

                if (restoreIndex != -1)
                {
                    gpu.Restore(restoreIndex);
                }

                var data = model.Polymeshes[polymshIdx];

                gpu.MultiplyMatrix(data.TRSMatrix);

                if (storeIndex != -1)
                {
                    gpu.Store(storeIndex);
                }

                break;

            case RenderOpCode.UNKNOWN_7:
                break;

            case RenderOpCode.UNKNOWN_8:
                break;

            case RenderOpCode.SKIN:
                break;

            case RenderOpCode.UNKNOWN_10:
                break;

            case RenderOpCode.MTX_SCALE:
                if (command.Flags == 1)
                {
                    // down scale
                    gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.DownScale));
                }
                else
                {
                    // up scale
                    gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.UpScale));
                }
                break;

            case RenderOpCode.UNKNOWN_12:
                break;

            case RenderOpCode.UNKNOWN_13:
                break;

            default:
                break;
        }
    }
}