using UnityEngine;

public class Deposit : MonoBehaviour {
    private int resources = 0;

    public void DepositResources(int amount) {
        if (amount > 0) {
            resources += amount;
        }
    }
}
