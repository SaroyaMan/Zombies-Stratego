using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZombieManager : Singleton<ZombieManager> {

    public ZombieBtn ZombieBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    private List<Zombie> zombieList = new List<Zombie>();
    private List<Collider2D> buildList = new List<Collider2D>();
    private Collider2D buildTile;

    private void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
    }
	
	private void Update () {
        HandleEscape();
        if(Input.GetMouseButtonDown(0)) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if(hit.collider.tag == "BuildSite") {   //Check if user clicked on build site
                buildTile = hit.collider;
                buildTile.tag = "BuildSiteFull";
                RegisterBuildSite(buildTile);
                PlaceZombie(hit);
            }
        }
        if(spriteRenderer.enabled) {
            FollowMouse();
        }
    }

    private void HandleEscape() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1)) {
            DisableDragSprite();
            ZombieBtnPressed = null;
        }
    }

    public void PlaceZombie(RaycastHit2D hit) {
        if(!EventSystem.current.IsPointerOverGameObject() && ZombieBtnPressed != null) {
            //GameManager.Instance.SoundFx.PlayOneShot(SoundManager.Instance.Towerbuilt);
            Zombie newZombie = Instantiate(ZombieBtnPressed.TowerObject);
            newZombie.transform.position = hit.transform.position;
            //BuyTower(ZombieBtnPressed.ZombiePrice);
            RegisterZombie(newZombie);
            DisableDragSprite();
            ZombieBtnPressed = null;
        }
    }

    public void RenameTagsBuildSites() {
        foreach(Collider2D collider in buildList) {
            collider.tag = "BuildSite";
        }
        buildList.Clear();
    }

    public void RegisterZombie(Zombie zombie) {
        zombieList.Add(zombie);
    }

    public void RegisterBuildSite(Collider2D buildTag) {
        buildList.Add(buildTag);
    }

    public void BuyTower(int price) {
        //GameManager.Instance.SubtractMoney(price);
    }

    public void SelectedZombie(ZombieBtn towerSelected) {
        //if(towerSelected.TowerPrice <= GameManager.Instance.TotalMoney) {
            ZombieBtnPressed = towerSelected;
            EnableDragSprite(ZombieBtnPressed.DragSprite);
        //}
    }

    public void FollowMouse() {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void EnableDragSprite(Sprite sprite) {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void DisableDragSprite() {
        spriteRenderer.enabled = false;
    }
}