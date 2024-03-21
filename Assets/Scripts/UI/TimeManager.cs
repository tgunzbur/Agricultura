using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoSingleton<TimeManager> {
    [SerializeField] private Image dayImage;
    [SerializeField] private TMP_Text timeText;

    private float startRotation;

    private const float START_HOUR = 6;
    private const int DAY_DURATION = 300;

    private float currentTime;
    public bool TimePass {
        get;
        private set;
    }

    public Action OnDayChange;

    private void Awake() {
        SetInstance(this);

        startRotation = dayImage.transform.rotation.eulerAngles.z;
        TimePass = true;
    }

    private void FixedUpdate() {
        if (!TimePass) {
            return;
        }
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= DAY_DURATION) {
            currentTime -= DAY_DURATION;
            OnDayChange?.Invoke();
        }
        OnTimeChange();
    }

    private void OnTimeChange() {
        dayImage.transform.rotation = Quaternion.Euler(0, 0, startRotation + currentTime / DAY_DURATION * 360);
        timeText.text = TimeSpan.FromSeconds(currentTime * 86400 / DAY_DURATION + START_HOUR * 3600).ToString(@"hh\:mm");
    }

    public void Pause(bool state) {
        TimePass = !state;
    }

    public float GetTime() {
        return currentTime;
    }

    public void FromSerialized(float time) {
        currentTime = time;
        TimePass = true;
        OnTimeChange();
    }

    public void Reset() {
        currentTime = 0;
        TimePass = true;
        OnTimeChange();
    }
}