using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class AchievementUI : MonoBehaviour {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private TMP_Text progressionText;
        [SerializeField] private Slider progressionSlider;
        
        public void UpdateAchievement(Achievement data) {
            iconImage.sprite = data.Data.GetIcon();
            nameText.text = data.Data.Name;
            objectiveText.text = data.Data.GetObjectiveBeautify();
            progressionText.text = $"{data.Progression}/{data.Data.Quantity}";
            progressionSlider.value = data.Progression / (float)data.Data.Quantity;
        }
    }
}