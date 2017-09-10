
using UnityEngine;

public class Flag: PlayerSoldier {

    [SerializeField] private Animator blueFlagAnimator;
    [SerializeField] private Animator redFlagAnimator;


    public override void SoldierPlacedInEditMode(bool isSoundActivated) {
        StrategyEditor.HasFlag = true;
        GameView.DisableButton("Btn_Flag");
        if(isSoundActivated)
            MakeNoise();
    }

    public override void MakeNoise() {
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.FlagBought);
    }

    public override void FlipSide() {
        base.FlipSide();
        OriginAnim = Anim.runtimeAnimatorController = CurrentSide == GameSide.LeftSide ?
            blueFlagAnimator.runtimeAnimatorController :
            redFlagAnimator.runtimeAnimatorController;
    }
}