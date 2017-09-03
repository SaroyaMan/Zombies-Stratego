
public class Flag: PlayerSoldier {

    public override void SoldierPlacedInEditMode(bool isSoundActivated) {
        StrategyEditor.HasFlag = true;
        GameView.DisableButton("Btn_Flag");
        if(isSoundActivated)
            MakeNoise();
    }

    public override void MakeNoise() {
        SoundManager.Instance.SFX.PlayOneShot(SoundManager.Instance.FlagBought);
    }
}