using UnityEngine;

public class GameManager : Singleton<GameManager> {

    //public GameMode GameMode = GameMode.Edit;
    int money = Globals.TOTAL_MONEY;

    public int Money { get { return money; } }

	private void Start () {
        //money = PlayerPrefs.GetInt("Money", Globals.TOTAL_MONEY);
        GameView.Instance.SetText("Txt_CurrMoney" ,money.ToString() );
    }
	
    public void SubtractMoney(int price) {
        money -= price;
        //PlayerPrefs.SetInt("Money", money);
        GameView.Instance.SetText("Txt_CurrMoney", money.ToString());
    }

    public void SaveStrategy() {

    }
}