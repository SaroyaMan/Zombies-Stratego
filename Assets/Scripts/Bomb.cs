using System.Collections;
using UnityEngine;

public class Bomb : PlayerSoldier {

    public IEnumerator Explode() {
        CurrentTile.IsInUse = false;
        CurrentTile = null;
        anim.Play("Explode");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.BombExplode);
        yield return new WaitForSeconds(1.5f);
        if(CurrentSide == GameManager.Instance.PcSide) SoldierManager.Instance.EnemyList.Remove(this);
        else SoldierManager.Instance.LocalPlayerList.Remove(this);
        GameManager.Instance.UpdateStats();

        Destroy(gameObject);
    }

    public override void SoldierPlacedInEditMode(bool isSoundActivated) {
        if(++StrategyEditor.NumOfBombs == Globals.MAX_BOMBS) {
            GameView.DisableButton("Btn_Bomb");
        }
        if(isSoundActivated)
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.BombBought);
    }
}