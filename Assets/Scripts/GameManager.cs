

public class GameManager : Singleton<GameManager> {

    public GameMode GameMode = GameMode.Edit;
    int money = Globals.TOTAL_MONEY;

    public int Money { get { return money; } }

	private void Start () {
        GameView.Instance.SetText("Txt_CurrMoney" ,money.ToString() );
    }
	
    public void SubtractMoney(int price) {
        money -= price;
        GameView.Instance.SetText("Txt_CurrMoney", money.ToString());
    }
}