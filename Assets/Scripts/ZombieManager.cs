using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : Singleton<ZombieManager> {

    private SpriteRenderer spriteRenderer;
    private List<Zombie> zombieList = new List<Zombie>();
    private List<Collider2D> buildList = new List<Collider2D>();
    private Collider2D buildTile;

    private void Start () {
		
	}
	
	private void Update () {
		
	}
}
