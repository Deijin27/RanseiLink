using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IMaxLinkService : IModelService<MaxLink>
    {
    }

    public class MaxLinkService : BaseNewableDataModelService<MaxLink>, IMaxLinkService
    {
        private MaxLinkService(string MaxLinkDatFile) : base(MaxLinkDatFile, 0, 251) { }

        public MaxLinkService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MaxLinkRomPath)) { }

        public MaxLink Retrieve(WarriorId id) => Retrieve((int)id);


        public override string IdToName(int id)
        {
            return ((WarriorId)id).ToString();
        }
    } 
}