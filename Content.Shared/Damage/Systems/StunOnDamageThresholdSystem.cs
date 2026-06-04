using Content.Shared.Administration.Logs;
using Content.Shared.Damage.Components;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Stunnable;

namespace Content.Shared.Damage.Systems;

/// <summary>
///     Система, вызывающая стан при накоплении СУММАРНОГО урона всех типов на 20 единиц.
///     Общий счётчик сбрасывается после каждого триггера стана.
/// </summary>
public sealed class StunOnDamageThresholdSystem : EntitySystem
{
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StunOnDamageThresholdComponent, DamageChangedEvent>(OnDamageChanged);
    }

    private void OnDamageChanged(EntityUid uid, StunOnDamageThresholdComponent component, DamageChangedEvent args)
    {
        // Нас интересует только нанесение урона, а не лечение
        if (!args.DamageIncreased || args.DamageDelta == null)
            return;

        // Суммируем весь положительный урон из дельты
        var totalDelta = FixedPoint2.Zero;
        foreach (var (_, delta) in args.DamageDelta.DamageDict)
        {
            if (delta > FixedPoint2.Zero)
                totalDelta += delta;
        }

        if (totalDelta <= FixedPoint2.Zero)
            return;

        component.AccumulatedTotal += totalDelta;

        if (component.AccumulatedTotal < component.DamageThreshold)
        {
            Dirty(uid, component);
            return;
        }

        // Порог достигнут — сбрасываем счётчик и применяем стан
        _adminLogger.Add(
            LogType.Damaged,
            LogImpact.Low,
            $"{ToPrettyString(uid):target} получил стан: накоплено {component.AccumulatedTotal} суммарного урона (порог {component.DamageThreshold})"
        );

        component.AccumulatedTotal = FixedPoint2.Zero;

        var stunTime = TimeSpan.FromSeconds(component.StunDuration);
        _stun.TryUpdateParalyzeDuration(uid, stunTime);

        Dirty(uid, component);
    }
}
