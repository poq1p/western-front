using Robust.Shared.Serialization;

namespace Content.Shared.Damage.Events;

[Serializable, NetSerializable]
public sealed class StunDamageThresholdScreenEffectEvent : EntityEventArgs
{

    public float Duration;

    public StunDamageThresholdScreenEffectEvent(float duration)
    {
        Duration = duration;
    }
}
