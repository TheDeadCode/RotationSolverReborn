﻿namespace RebornRotations.PVPRotations.Melee;

[Rotation("Default", CombatType.PvP, GameVersion = "7.21")]
[SourceCode(Path = "main/RebornRotations/PVPRotations/Melee/MNK_Default.PVP.cs")]
[Api(4)]
public sealed class MNK_DefaultPvP : MonkRotation
{
    #region Configurations

    [RotationConfig(CombatType.PvP, Name = "Use Purify")]
    public bool UsePurifyPvP { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "Stop attacking while in Guard.")]
    public bool RespectGuard { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Player health threshold needed for Bloodbath use")]
    public float BloodBathPvPPercent { get; set; } = 0.75f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvP, Name = "Enemy health threshold needed for Smite use")]
    public float SmitePvPPercent { get; set; } = 0.25f;
    #endregion

    #region Standard PVP Utilities
    private bool DoPurify(out IAction? action)
    {
        action = null;
        if (!UsePurifyPvP)
        {
            return false;
        }

        List<int> purifiableStatusesIDs = new()
        {
            // Stun, DeepFreeze, HalfAsleep, Sleep, Bind, Heavy, Silence
            1343, 3219, 3022, 1348, 1345, 1344, 1347
        };

        return purifiableStatusesIDs.Any(id => Player.HasStatus(false, (StatusID)id)) && PurifyPvP.CanUse(out action);
    }
    #endregion

    #region oGCDs
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (DoPurify(out action))
        {
            return true;
        }

        if (InCombat && Player.GetHealthRatio() < 0.8 && RiddleOfEarthPvP.CanUse(out action))
        {
            return true;
        }

        if (Player.HasStatus(true, StatusID.EarthResonance) && Player.WillStatusEnd(1, true, StatusID.EarthResonance))
        {
            if (Player.GetHealthRatio() < 0.5 && EarthsReplyPvP.CanUse(out action))
            {
                return true;
            }

            if (Player.WillStatusEnd(1, true, StatusID.EarthResonance) && EarthsReplyPvP.CanUse(out action))
            {
                return true;
            }
        }

        if (Player.GetHealthRatio() < BloodBathPvPPercent && BloodbathPvP.CanUse(out action))
        {
            return true;
        }

        if (SwiftPvP.CanUse(out action))
        {
            return true;
        }

        return CurrentTarget?.GetHealthRatio() <= SmitePvPPercent && SmitePvP.CanUse(out action) || base.EmergencyAbility(nextGCD, out action);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (HasHostilesInRange && RisingPhoenixPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        return HasHostilesInRange && EarthsReplyPvP.CanUse(out action, usedUp: true) || base.AttackAbility(nextGCD, out action);
    }

    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? action)
    {
        action = null;
        return (!RespectGuard || !Player.HasStatus(true, StatusID.Guard)) && base.MoveForwardAbility(nextGCD, out action);
    }
    #endregion

    #region GCDs
    protected override bool GeneralGCD(out IAction? action)
    {
        action = null;
        if (RespectGuard && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (PhantomRushPvP.CanUse(out action))
        {
            return true;
        }

        if (FiresReplyPvP.CanUse(out action))
        {
            return true;
        }

        if (WindsReplyPvP.CanUse(out action))
        {
            return true;
        }

        if (FlintsReplyPvP.CanUse(out action, usedUp: true))
        {
            return true;
        }

        if (PouncingCoeurlPvP.CanUse(out action))
        {
            return true;
        }

        if (RisingRaptorPvP.CanUse(out action))
        {
            return true;
        }

        if (LeapingOpoPvP.CanUse(out action))
        {
            return true;
        }

        if (DemolishPvP.CanUse(out action))
        {
            return true;
        }

        if (TwinSnakesPvP.CanUse(out action))
        {
            return true;
        }

        return DragonKickPvP.CanUse(out action) || base.GeneralGCD(out action);
    }
    #endregion
}