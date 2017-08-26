using UnityEngine;

public class GameController : MonoBehaviour {


    public void ZombieSelectedInEditMode(ZombieBtn zombieSelected) {
        ZombieManager.Instance.SelectedZombie(zombieSelected);
    }
}