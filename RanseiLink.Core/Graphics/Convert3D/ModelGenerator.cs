using System.Collections.Generic;
using System.Numerics;

namespace RanseiLink.Core.Graphics
{
    public class ModelGenerator
    {
        /// <summary>
        /// Make sure to populate materials first, then run this.
        /// Populates the Polymeshes, RenderCommands, and Polygons
        /// </summary>
        public static void Generate(List<Group> groups, NSMDL.Model model)
        {
            var state = new ModelGenerator();
            state.gpu = new InverseGpuState();
            state.model = model;
            state.parentPolymeshStack = new Stack<int>();
            state.parentPolymeshStack.Push(0);
            state.gpu.CurrentMaterial = state.model.Materials[0];
            state.Process(groups);
        }

        private InverseGpuState gpu;
        private NSMDL.Model model;
        private Stack<int> parentPolymeshStack;

        private void Process(List<Group> groups)
        {
            // root set
            model.Polymeshes.Add(new NSMDL.Model.PolymeshData
            {
                Name = "set-" + model.Name.ToLowerInvariant(),
            });
            model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT_STORE(0, parentPolymeshStack.Peek(), 0, 0));
            parentPolymeshStack.Push(0);

            // first and only child of set
            model.Polymeshes.Add(new NSMDL.Model.PolymeshData
            {
                Name = model.Name.ToLowerInvariant()
            });
            model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT_STORE(1, parentPolymeshStack.Peek(), 0, 1));
            parentPolymeshStack.Push(1);

            // contents of child
            bool first = true;
            foreach (var group in groups)
            {
                ProcessGroup(group, first);
                first = false;
            }

            model.RenderCommands.Add(RenderCommandGenerator.END());
        }

        private void ProcessGroup(Group group, bool first)
        {
            int polymeshId = model.Polymeshes.Count;
            int polygonId = model.Polygons.Count;
            int materialId = model.Materials.FindIndex(x => x.Name == group.MaterialName);

            // create polymesh data
            model.Polymeshes.Add(new NSMDL.Model.PolymeshData
            {
                Name = $"polymsh{polymeshId}"
            });

            // do the commands

            if (first)
            {
                model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT(polymeshId, parentPolymeshStack.Peek(), 0));
                gpu.MultiplyMatrix(model.Polymeshes[polymeshId].TRSMatrix);
            }
            else
            {
                model.RenderCommands.Add(RenderCommandGenerator.MTX_MULT_RESTORE(polymeshId, parentPolymeshStack.Peek(), 0, 1));
                gpu.MultiplyMatrix(model.Polymeshes[polymeshId].TRSMatrix);
            }


            model.RenderCommands.Add(RenderCommandGenerator.VISIBILITY(polymeshId, true));

            model.RenderCommands.Add(RenderCommandGenerator.MTX_SCALE_UP());
            gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.UpScale));

            model.RenderCommands.Add(RenderCommandGenerator.BIND_MATERIAL(materialId));
            gpu.CurrentMaterial = model.Materials[materialId];

            // create polygon display commands
            model.RenderCommands.Add(RenderCommandGenerator.DRAW_MESH(polygonId));
            var dl = PolygonGenerator.Generate(group, gpu);
            model.Polygons.Add(new NSMDL.Model.Polygon
            {
                ItemTag = 0,
                Flag = NSMDL.Model.SHPFLAG.USE_NORMAL | NSMDL.Model.SHPFLAG.USE_TEXCOORD,
                Name = group.Name,
                Commands = dl,
            });

            model.RenderCommands.Add(RenderCommandGenerator.MTX_SCALE_DOWN());
            gpu.MultiplyMatrix(Matrix4x4.CreateScale(model.MdlInfo.DownScale));
        }
    }
}