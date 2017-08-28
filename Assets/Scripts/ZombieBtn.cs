using UnityEngine;
using UnityEngine.UI;

public class ZombieBtn : MonoBehaviour {

    [SerializeField] private Zombie zombieObject;
    [SerializeField] private Sprite dragSprite;

    public void Start() {
        Text[] texts = GetComponentsInChildren<Text>();
        GameView.Instance.SetText(texts[0], ZombieObject.Price.ToString() );
        GameView.Instance.SetText(texts[1], ZombieObject.Rank.ToString());
    }

    public Zombie ZombieObject {
        get {
            return zombieObject;
        }
    }

    public Sprite DragSprite {
        get {
            return dragSprite;
        }
    }
}