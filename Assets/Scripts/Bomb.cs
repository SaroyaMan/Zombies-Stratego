using System.Collections;
using UnityEngine;

public class Bomb : PlayerSoldier {

    public IEnumerator Explode() {
        CurrentTile.IsInUse = false;
        CurrentTile = null;
        anim.Play("Explode");
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.BombExplode);
        yield return new WaitForSeconds(1.5f);
        if(Globals.IS_SINGLE_PLAYER && CurrentSide == GameManager.Instance.PcSide || !Globals.IS_SINGLE_PLAYER && MultiPlayerManager.Instance.PlayerSide != CurrentSide)
            SoldierManager.Instance.EnemyList.Remove(this);
        else
            SoldierManager.Instance.LocalPlayerList.Remove(this);
        GameManager.Instance.UpdateStats();

        Destroy(gameObject);
    }

    public override void SoldierPlacedInEditMode(bool isSoundActivated) {
        if(++StrategyEditor.NumOfBombs == Globals.MAX_BOMBS) {
            GameView.DisableButton("Btn_Bomb");
        }
        if(isSoundActivated)
            MakeNoise();
    }

    public override void MakeNoise() {
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.BombBought);
    }
}