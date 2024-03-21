using System.Collections;
using System.Collections.Generic;
using Agricultura;
using UnityEngine;

public class AchievementUIManager : MonoSingleton<AchievementUIManager> {
    [SerializeField] private GameObject window;
    [SerializeField] private Transform achievementsParent;
    [SerializeField] private AchievementUI achievementPrefab;

    private DynamicUIListPoolable<Achievement> achievementsList;
    private void Awake() {
        SetInstance(this);
        achievementsList = new DynamicUIListPoolable<Achievement>(achievementsParent, achievementPrefab.gameObject, UpdateAchievement);
    }

    public void ShowAchievements(IEnumerable<Achievement> achievements) {
        achievementsList.ReplaceAllItems(achievements);
        window.SetActive(true);
    }

    public void HideAchievements() {
        window.SetActive(false);
    }

    private void UpdateAchievement(GameObject item, Achievement data) {
        item.GetComponent<AchievementUI>().UpdateAchievement(data);
    }
}