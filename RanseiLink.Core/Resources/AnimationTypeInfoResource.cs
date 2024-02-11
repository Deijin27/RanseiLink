using RanseiLink.Core.Graphics;

namespace RanseiLink.Core.Resources;

public enum AnimationTypeId
{
    Castlemap,
    KuniImage2,
    IconCastle,
    IconInst
}

public class AnimationTypeInfo
{
    private readonly string _animationLinkFormat;
    private readonly string? _backgroundLinkFormat;

    public AnimationTypeInfo(
        AnimationTypeId id,
        string animationLinkFormat,
        PositionRelativeTo prt,
        int maxId,
        string? backgroundLinkFormat,
        RLAnimationFormat? exportFormat = null
    )
    {
        Id = id;
        _animationLinkFormat = FileUtil.NormalizePath(animationLinkFormat);
        Prt = prt;
        MaxId = maxId;
        _backgroundLinkFormat = FileUtil.NormalizePath(backgroundLinkFormat);
        ExportFormat = exportFormat;
        Width = -1;
        Height = -1;
    }

    public AnimationTypeInfo(
        AnimationTypeId id,
        string animationLinkFormat,
        PositionRelativeTo prt,
        int maxId,
        int width,
        int height,
        RLAnimationFormat? exportFormat = null
    )
    {
        Id = id;
        _animationLinkFormat = FileUtil.NormalizePath(animationLinkFormat);
        Prt = prt;
        MaxId = maxId;
        _backgroundLinkFormat = null;
        ExportFormat = exportFormat;
        Width = width;
        Height = height;
    }

    public AnimationTypeId Id { get; }
    public PositionRelativeTo Prt { get; }
    public int MaxId { get; }
    public RLAnimationFormat? ExportFormat { get; }
    public int Width { get; }
    public int Height { get; }

    public string AnimationRelativePath(int id)
    {
        return string.Format(_animationLinkFormat, id);
    }

    public string? BackgroundRelativePath(int id)
    {
        if (_backgroundLinkFormat == null)
        {
            return null;
        }
        return string.Format(_backgroundLinkFormat, id);
    }
}

public static class AnimationTypeInfoResource
{
    private static readonly Dictionary<AnimationTypeId, AnimationTypeInfo> _types = new();
    private static void Register(AnimationTypeInfo type) => _types.Add(type.Id, type);

    public static IEnumerable<AnimationTypeInfo> All => _types.Values;

    static AnimationTypeInfoResource()
    {
        Register(new(
            id: AnimationTypeId.Castlemap,
            animationLinkFormat: "graphics/strategy/castlemap_anime/03_05_parts_castlemap_{0:D2}_lo.G2DR",
            prt: PositionRelativeTo.Centre,
            maxId: 16,
            backgroundLinkFormat: "graphics/strategy/castlemap/03_05_BG_castlemap_{0:D2}_lo.G2DR"
            ));
        Register(new(
            id: AnimationTypeId.KuniImage2,
            animationLinkFormat: "graphics/strategy/kuniimage2/03_00_parts_kuniimage_{0:D2}_up_anim.G2DR",
            prt: PositionRelativeTo.TopLeft,
            maxId: 16,
            backgroundLinkFormat: "graphics/strategy/kuniimage2/03_00_parts_kuniimage_{0:D2}_up.G2DR"
            ));
        Register(new(
            id: AnimationTypeId.IconCastle,
            animationLinkFormat: "graphics/strategy/icon_castle/03_01_parts_castleicon_{0:D2}_lo.G2DR",
            prt: PositionRelativeTo.Centre,
            maxId: 16,
            width: 64,
            height: 64,
            exportFormat: RLAnimationFormat.OneImagePerCell
            ));
        Register(new(
            id: AnimationTypeId.IconInst,
            animationLinkFormat: "graphics/strategy/icon_inst/03_05_parts_shisetsuicon_{0:D2}_lo.G2DR",
            prt: PositionRelativeTo.Centre,
            maxId: 83,
            width: 64,
            height: 64,
            exportFormat: RLAnimationFormat.OneImagePerCell
            ));
    }

    public static AnimationTypeInfo Get(AnimationTypeId id) => _types[id];
}
