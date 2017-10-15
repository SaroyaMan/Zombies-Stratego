using System.Collections;
using UnityEngine;

public class Bomb : PlayerSoldier {

    public IEnumerator Explode(bool isExplode) {
        if(isExplode) {
            anim.Play("Explode");
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.BombExplode);
            yield return new WaitForSeconds(1.5f);
        }
        else {
            SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.BombBought);
        }

        //if(Globals.IS_SINGLE_PLAYER && CurrentSide == GameManager.Instance.PcSide || !Globals.IS_SINGLE_PLAYER && MultiPlayerManager.Instance.PlayerSide != CurrentSide)
        //    SoldierManager.Instance.EnemyList.Remove(this);
        //else
        //    SoldierManager.Instance.LocalPlayerList.Remove(this);
        SoldierManager.Instance.RemoveSoldierFromList(this);

        GameManager.Instance.UpdateStats();
        GameManager.Instance.CloseInfo();
        Destroy(gameObject);
        //CurrentTile.IsInUse = false;
        CurrentTile = null;
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