using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Damage;

public sealed class StunDamageThresholdOverlay : Overlay
{
    private static readonly ProtoId<ShaderPrototype> CircleMaskShader = "GradientCircleMask";

    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    private readonly ShaderInstance _shader;

    public float Intensity = 0f;

    public TimeSpan EndTime = TimeSpan.Zero;

    public float Duration = 10f;

    public StunDamageThresholdOverlay()
    {
        IoCManager.InjectDependencies(this);
        _shader = _prototypeManager.Index(CircleMaskShader).InstanceUnique();
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_entityManager.TryGetComponent(_playerManager.LocalEntity, out EyeComponent? eyeComp))
            return false;

        return args.Viewport.Eye == eyeComp.Eye;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var now = _timing.RealTime;

        var remaining = (float)(EndTime - now).TotalSeconds;
        if (remaining <= 0f)
        {
            Intensity = 0f;
            return;
        }

        var fadeOutStart = Duration * 0.3f;
        if (remaining < fadeOutStart)
            Intensity = remaining / fadeOutStart;
        else
            Intensity = 1f;

        var viewport = args.WorldAABB;
        var handle = args.WorldHandle;
        var distance = args.ViewportBounds.Width;

        var outerRadius = (0.6f - Intensity * 0.45f) * distance;
        var innerRadius = (0.05f - Intensity * 0.04f) * distance;


        var darkness = Math.Clamp(0.5f + Intensity * 0.48f, 0f, 0.98f);

        _shader.SetParameter("time", 0.0f);
        _shader.SetParameter("color", new Vector3(0f, 0f, 0f));
        _shader.SetParameter("darknessAlphaOuter", darkness);
        _shader.SetParameter("outerCircleRadius", outerRadius);
        _shader.SetParameter("outerCircleMaxRadius", outerRadius + 0.15f * distance);
        _shader.SetParameter("innerCircleRadius", innerRadius);
        _shader.SetParameter("innerCircleMaxRadius", innerRadius + 0.03f * distance);

        handle.UseShader(_shader);
        handle.DrawRect(viewport, Color.White);
        handle.UseShader(null);
    }
}
