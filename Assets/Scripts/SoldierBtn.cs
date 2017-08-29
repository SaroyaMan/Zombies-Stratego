using UnityEngine;
using UnityEngine.UI;

public class SoldierBtn : MonoBehaviour {

    [SerializeField] private PlayerSoldier soldierObject;
    [SerializeField] private Sprite dragSprite;

    public void Start() {
        Text[] texts = GetComponentsInChildren<Text>();
        GameView.Instance.SetText(texts[0], soldierObject.Price.ToString() );
        GameView.Instance.SetText(texts[1], soldierObject.Rank.ToString());
    }

    public PlayerSoldier SoldierObject {
        get {
            return soldierObject;
        }
    }

    public Sprite DragSprite {
        get {
            return dragSprite;
        }
    }
}