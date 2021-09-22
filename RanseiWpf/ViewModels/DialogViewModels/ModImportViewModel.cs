using Core.Services;
using RanseiWpf.DragDrop;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace RanseiWpf.ViewModels
{
    public class ModImportViewModel : ViewModelBase
    {
        public ModImportViewModel()
        {
            ModInfo = new ModInfo();
            ModDropHandler = new ModDropHandler();
            ModDropHandler.FileDropped += f =>
            {
                try
                {
                    ModInfo modInfo;
                    using (ZipArchive zip = ZipFile.OpenRead(f))
                    {
                        ZipArchiveEntry entry = zip.GetEntry(ModService.ModInfoFileName);
                        if (entry == null)
                        {
                            throw new Exception("Failed to load mod because ModInfoFile not found.");
                        }
                        using (Stream modInfoStream = entry.Open())
                        {
                            if (!ModInfo.TryLoadFrom(XDocument.Load(modInfoStream), out modInfo))
                            {
                                int hi = 0;
                                throw new Exception("Failed to load mod because failed to load ModInfo from ModInfoFile.");
                            }
                        }
                    }

                    ModInfo = modInfo;
                    File = f;
                    OkEnabled = true;
                }
                catch
                {

                }
            };
        }

        public ModDropHandler ModDropHandler { get; }

        private string _file;
        public string File
        {
            get => _file;
            set => RaiseAndSetIfChanged(ref _file, value);
        }

        private bool _okEnabled = false;
        public bool OkEnabled
        {
            get => _okEnabled;
            set => RaiseAndSetIfChanged(ref _okEnabled, value);
        }

        private ModInfo _modInfo;
        public ModInfo ModInfo
        {
            get => _modInfo;
            set => RaiseAndSetIfChanged(ref _modInfo, value);
        }
    }
}
