using Core.Nds;
using Core.Services;
using System;
using System.Collections.Generic;

namespace Tests.Mocks
{
    public class MockCoreAppServices : ICoreAppServices
    {
        public ISettingsService Settings => throw new NotImplementedException();

        public IModService ModService => throw new NotImplementedException();

        public string RootFolder => throw new NotImplementedException();

        public Dictionary<ModInfo, IDataService> DataServiceReturn = new Dictionary<ModInfo, IDataService>();
        public Queue<ModInfo> DataServiceCalls = new Queue<ModInfo>();
        public IDataService DataService(ModInfo mod)
        {
            DataServiceCalls.Enqueue(mod);
            return DataServiceReturn[mod];
        }

        public INds NdsReturn { get; set; }
        public Queue<string> NdsCalls = new Queue<string>();
        public INds Nds(string path)
        {
            NdsCalls.Enqueue(path);
            return NdsReturn;
        }
    }
}
