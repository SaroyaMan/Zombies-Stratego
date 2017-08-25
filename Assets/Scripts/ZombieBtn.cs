using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBtn : MonoBehaviour {

    [SerializeField] private Zombie zombieObject;
    [SerializeField] private Sprite dragSprite;
    [SerializeField] private int zombiePrice;
 

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

    public int ZombiePrice {
        get {
            return zombiePrice;
        }
    }
}
