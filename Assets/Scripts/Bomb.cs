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
        SoldierManager.Instance.RemoveSoldierFromList(this);

        GameManager.Instance.UpdateStats();
        GameManager.Instance.CloseInfo();
        Destroy(gameObject);
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

    public override void SetWarTag() {
        gameObject.tag = "BombInWar";
    }
}