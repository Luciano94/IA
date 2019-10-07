using UnityEngine;

public static class UlongRandom {
    public static ulong GetRandom() {
        return (ulong)Random.Range(0, int.MaxValue)*(ulong)Random.Range(0, int.MaxValue);
    }
}
