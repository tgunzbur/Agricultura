using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agricultura;
using Agricultura.Data;
using UnityEngine;

public class AchievementsManager : MonoSingleton<AchievementsManager> {
    public HashSet<Achievement> Achievements;

    private void Awake() {
        SetInstance(this);
    }

    private void AddAchievement(AchievementData data, int progression = 0) {
        switch (data.AchievementType) {
            case AchievementType.CollectPlant:
                CollectPlantAchievement collectAchievement = new (data, progression);
                GameManager.Get().OnCollectPlant += collectAchievement.OnCollectPlant;
                Achievements.Add(collectAchievement);
                break;
            case AchievementType.CookFood:
                CookFoodAchievement cookAchievement = new (data, progression);
                GameManager.Get().OnCookFood += cookAchievement.OnCookFood;
                Achievements.Add(cookAchievement);
                break;
            case AchievementType.SellItem:
                SellItemAchievement sellAchievement = new (data, progression);
                GameManager.Get().OnSellItem += sellAchievement.OnSellItem;
                Achievements.Add(sellAchievement);
                break;
            default:
                break;
        }
    }

    public SerializedAchievements ToSerialized() {
        return new SerializedAchievements() {
            Achievements = Achievements.Select(achievement => achievement.ToSerialized()).ToList()
        };
    }

    public void FromSerialized(SerializedAchievements serializedAchievements) {
        Achievements = new HashSet<Achievement>();
        foreach (SerializedAchievement serializedAchievement in serializedAchievements.Achievements) {
            AddAchievement(DataManager.GetData<AchievementData>(serializedAchievement.AchievementId), serializedAchievement.Progression);
        }
    }

    public void Reset() {
        Achievements = new HashSet<Achievement>();
        foreach (AchievementData achievementData in DataManager.GetAll<AchievementData>(DataType.AchievementData)) {
            AddAchievement(achievementData);
        }
    }
}