using Content.Shared.Damage.Events;
using Robust.Client.Graphics;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Damage;


public sealed class StunDamageThresholdSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private StunDamageThresholdOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        _overlay = new StunDamageThresholdOverlay();

        SubscribeNetworkEvent<StunDamageThresholdScreenEffectEvent>(OnScreenEffect);
        SubscribeLocalEvent<LocalPlayerDetachedEvent>(OnPlayerDetached);
    }

    private void OnScreenEffect(StunDamageThresholdScreenEffectEvent args)
    {
        if (!_overlayMan.HasOverlay<StunDamageThresholdOverlay>())
            _overlayMan.AddOverlay(_overlay);

        _overlay.Duration = args.Duration;
        _overlay.EndTime = _timing.RealTime + TimeSpan.FromSeconds(args.Duration);
        _overlay.Intensity = 1f;
    }

    private void OnPlayerDetached(LocalPlayerDetachedEvent args)
    {
        _overlayMan.RemoveOverlay(_overlay);
        _overlay.Intensity = 0f;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_overlayMan.HasOverlay<StunDamageThresholdOverlay>())
            return;

        if (_overlay.Intensity <= 0f)
            _overlayMan.RemoveOverlay(_overlay);
    }
}
