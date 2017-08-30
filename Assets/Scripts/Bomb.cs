using System.Collections;
using UnityEngine;

public class Bomb : PlayerSoldier {

    public IEnumerator Explode() {
        CurrentTile.IsInUse = false;
        CurrentTile = null;
        anim.Play("Explode");
        yield return new WaitForSeconds(1.5f);
        SoldierManager.Instance.UnregisterPlayer(this);
        GameManager.Instance.UpdateStats();

        Destroy(gameObject);
    }
}