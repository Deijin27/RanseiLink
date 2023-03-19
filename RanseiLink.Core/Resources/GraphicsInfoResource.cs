using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources
{
    public static class GraphicsInfoResource
    {
        private const string _graphicsInfoXml = "RanseiLink.Core.Resources.GraphicsInfo.xml";
        static GraphicsInfoResource()
        {
            XDocument doc;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_graphicsInfoXml)!)
            {
                doc = XDocument.Load(stream);
            }

            var root = doc.Element("GraphicsInfo")!;
            foreach (var element in root.Elements())
            {
                var metaType = Enum.Parse<MetaSpriteType>(element.Name.ToString());
                GraphicsInfo info = metaType switch
                {
                    MetaSpriteType.STL => new StlConstants(metaType, element),
                    MetaSpriteType.SCBG => new ScbgConstants(metaType, element),
                    MetaSpriteType.PKMDL => new PkmdlConstants(metaType, element),
                    MetaSpriteType.Misc => new MiscConstants(metaType, element),
                    _ => throw new Exception("Invalid element in GraphicsInfo.xml"),
                };
                _all[info.Type] = info;
            }
        }

        private static readonly Dictionary<SpriteType, IGraphicsInfo> _all = new();

        public static IReadOnlyCollection<IGraphicsInfo> All => _all.Values;
        public static IGraphicsInfo Get(SpriteType type)
        {
            return _all[type];
        }
    }
}