using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour {


    [SerializeField] private int target = 0;
    [SerializeField] private float navigationUpdate;
    //[SerializeField] private int healthPoints;

    private Transform zombie;
    //private Collider2D zombieCollider;
    private float navigationTime = 0;
    private bool isDead;
    private Animator anim;

    public bool IsDead {
        get {
            return isDead;
        }
    }
    // Use this for initialization
    private void Start () {
        zombie = GetComponent<Transform>();
        //zombieCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        //GameManager.Instance.RegisterEnemy(this);
    }

    // Update is called once per frame
    private void Update () {
		
	}
}
