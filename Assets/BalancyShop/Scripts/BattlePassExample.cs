using System.Collections.Generic;
using Balancy;
using Balancy.Models;
using Balancy.Models.SmartObjects.Conditions;
using Balancy.SmartObjects;
using UnityEngine;

public class BattlePassExample : IConditionsListener
{
    private List<BattlePass> _availableBattlePasses;
    private BattlePass _activeBattlePass;//TODO store this value in profile

    private static BattlePassExample _instance;

    public static BattlePassExample GetInstance()
    {
        Debug.LogWarning("_instance " + _instance);
        return _instance;
    }

    public static void Refresh()
    {
        _instance = new BattlePassExample();
    }

    public BattlePassExample()
    {
        Init();
    }

    private void Init()
    {
        _availableBattlePasses = new List<BattlePass>();

        foreach (var battlePass in DataEditor.BattlePasses)
            LiveOps.General.ConditionSubscribe(battlePass.Condition, battlePass, this, _activeBattlePass == battlePass ? PassType.True : PassType.False);
    }

    public void Clear()
    {
        _availableBattlePasses.Clear();
        _availableBattlePasses = null;
        _activeBattlePass = null;
    }

    public void ConditionPassed(object data)
    {
        var bonus = data as BattlePass;
        _availableBattlePasses.Add(bonus);
        CheckPass();
    }

    public void ConditionFailed(object data)
    {
        var bonus = data as BattlePass;
        _availableBattlePasses.Remove(bonus);

        if (_activeBattlePass == bonus)
        {
            _activeBattlePass = null;
            CheckPass();
        }
    }

    private void CheckPass()
    {
        var bonus = _availableBattlePasses.Count > 0 ? _availableBattlePasses[0] : null;
        if (bonus != null && _activeBattlePass == null)
            _activeBattlePass = bonus;
    }

    public BattlePass GetActiveBattlePass()
    {
        return _activeBattlePass;
    }

    public void PrintActivePassInfo()
    {
        if (_activeBattlePass == null)
            Debug.LogError("No Battle Pass active");
        else
        {
            Debug.LogWarning($"==> {_activeBattlePass.Points.Length} Points");
            foreach (var point in _activeBattlePass.Points)
            {
                Debug.Log($"{point.Scores} : Free Reward = {point.FreeReward.Items.Length} ; Premium Reward = {point.PremiumReward.Items.Length}");
            }   
        }
    }
}