using UnityEngine;

public static class LongRandom {
    public static ulong GetRandom() {
        return (ulong)Random.Range(0, int.MaxValue)*(ulong)Random.Range(0, int.MaxValue);
    }
}
