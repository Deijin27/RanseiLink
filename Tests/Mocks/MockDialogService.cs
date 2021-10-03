using Core.Randomization;
using Core.Services;
using RanseiWpf.Services;
using System;
using System.Windows;

namespace Tests.Mocks
{
    public class MockDialogService : IDialogService
    {
        public int ShowMessageBoxCallCount = 0;
        public MessageBoxResult ShowMessageBoxReturn = MessageBoxResult.OK;
        public MessageBoxResult ShowMessageBox(MessageBoxArgs options)
        {
            ShowMessageBoxCallCount++;
            return ShowMessageBoxReturn;
        }

        public bool CreateMod(out ModInfo modInfo, out string romPath)
        {
            throw new NotImplementedException();
        }

        public bool CreateModBasedOn(ModInfo baseMod, out ModInfo newModInfo)
        {
            throw new NotImplementedException();
        }

        public bool ExportMod(ModInfo info, out string folder)
        {
            throw new NotImplementedException();
        }

        public bool ImportMod(out string file)
        {
            throw new NotImplementedException();
        }

        public bool EditModInfo(ModInfo info)
        {
            throw new NotImplementedException();
        }

        public bool CommitToRom(ModInfo info, out string romPath)
        {
            throw new NotImplementedException();
        }

        public bool RequestRomFile(out string result)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmDelete(ModInfo info)
        {
            throw new NotImplementedException();
        }

        public bool Randomize(IRandomizer randomizer)
        {
            throw new NotImplementedException();
        }

        public bool RequestFolder(out string result)
        {
            throw new NotImplementedException();
        }

        public bool RequestModFile(out string result)
        {
            throw new NotImplementedException();
        }
    }
}
