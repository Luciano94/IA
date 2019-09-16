using UnityEngine;

public class Mine : MonoBehaviour {
    public int resources;

    public int ExtractResources(int amount) {
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
