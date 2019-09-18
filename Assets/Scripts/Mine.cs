using UnityEngine;

public class Mine : MonoBehaviour {
    public int resources;
    public LayerMask runnedOutLayer;

    private void Awake() {
        if (resources <= 0) {
            gameObject.layer = 1 << runnedOutLayer;
        }
    }

    public int ExtractResources(int amount) {
        if (resources - amount <= 0) {
            gameObject.layer = 1 << runnedOutLayer;
        }
        if (resources >= amount) {
            resources -= amount;
            return amount;
        } else if (resources > 0) {
            int extracted = resources;
            resources = 0;
            return extracted;
        } else {
            return 0;
        }
    }
}
