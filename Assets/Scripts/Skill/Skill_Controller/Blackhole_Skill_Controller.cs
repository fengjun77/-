using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;

    private bool canCreatedHotKey = true;
    private bool canAttack;
    public int attckAmount = 4;
    private int lastTargetIndex = 0;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;
    private float blackholeDuration;

    private bool playerCanDisapear = true;

    private List<Transform> targets = new List<Transform>();
    //记录已经创建的热键
    private List<GameObject> createdHotKey = new List<GameObject>();

    public bool playerCanExitState {  get; private set; }

    public void SetupBlackhole(float _maxSize,float _growSpeed,float _shrinkSpeed,int _attackAmount,float _cloneAttackCooldown,float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        attckAmount = _attackAmount;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeDuration = _blackholeDuration;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeDuration -= Time.deltaTime;

        if(blackholeDuration < 0)
        {
            blackholeDuration = Mathf.Infinity;

            if(targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackholeAbility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            //Lerp,由快到慢变化
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        DestroyHotKey();
        canAttack = true;
        canCreatedHotKey = false;

        if(playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.MakeTransparent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && canAttack && attckAmount > 0 )
        {
            cloneAttackTimer = cloneAttackCooldown;

            int index = lastTargetIndex % targets.Count;
            lastTargetIndex++;

            float xOffset = (Random.Range(0, 100) > 50) ? 1.2f : -1.2f;


            SkillManager.instance.clone.CreateClone(targets[index], new Vector3(xOffset, 0));

            attckAmount--;

            if (attckAmount <= 0)
            {
                Invoke("FinishBlackholeAbility",.5f);
            }
        }
    }

    private void FinishBlackholeAbility()
    {
        DestroyHotKey();
        playerCanExitState = true;
        canShrink = true;
        canAttack = false;
    }

    private void DestroyHotKey()
    {
        if (createdHotKey.Count <= 0)
            return;

        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent <Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
            return;

        if(!canCreatedHotKey)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode chooseKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(chooseKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(chooseKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
