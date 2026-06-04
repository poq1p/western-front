using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared.Damage.Components;

/// <summary>
///     При накоплении СУММАРНОГО урона всех типов на величину <see cref="DamageThreshold"/>
///     сущность получает стан на <see cref="StunDuration"/> секунд.
///     Общий счётчик сбрасывается после каждого триггера стана.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class StunOnDamageThresholdComponent : Component
{
    /// <summary>
    ///     Суммарный урон всех типов, необходимый для вызова стана.
    /// </summary>
    [DataField]
    public FixedPoint2 DamageThreshold = 20;

    /// <summary>
    ///     Продолжительность стана в секундах.
    /// </summary>
    [DataField]
    public float StunDuration = 10f;

    /// <summary>
    ///     Накопленный суммарный урон с момента последнего стана / старта.
    /// </summary>
    [DataField]
    public FixedPoint2 AccumulatedTotal = FixedPoint2.Zero;
}
