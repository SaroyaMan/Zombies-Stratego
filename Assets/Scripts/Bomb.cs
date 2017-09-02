using System.Collections;
using UnityEngine;

public class Bomb : PlayerSoldier {

    public IEnumerator Explode() {
        CurrentTile.IsInUse = false;
        CurrentTile = null;
        anim.Play("Explode");
        yield return new WaitForSeconds(1.5f);
        if(CurrentSide == GameManager.Instance.PcSide) SoldierManager.Instance.EnemyList.Remove(this);
        else SoldierManager.Instance.LocalPlayerList.Remove(this);
        GameManager.Instance.UpdateStats();

        Destroy(gameObject);
    }
}