using UnityEngine;

public static class MathFunc {
    public static int Modulo(int a, int b) {
        return a - b * Mathf.FloorToInt((float)a / b);
    }
}