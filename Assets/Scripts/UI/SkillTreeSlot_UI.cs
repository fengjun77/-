using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeSlot_UI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISaveManager
{
    //技能名
    [SerializeField] private string skillName;
    [TextArea]
    //技能描述
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    //是否可以解锁
    public bool unlocked;

    //需要解锁的技能列表(如果这个列表中的技能未被解锁，则该技能无法解锁)
    [SerializeField] private SkillTreeSlot_UI[] shouldBeUnlocked;
    //可解锁的技能列表
    [SerializeField] private SkillTreeSlot_UI[] shouldBeLocked;
    //技能图标
    private Image skillImage;

    private UI ui;

    public int price;

    private void OnValidate()
    {
        gameObject.name = "技能槽 - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;

        if (unlocked)
            skillImage.color = Color.white;
    }

    public void UnlockSkillSlot()
    {
        if (unlocked == true || PlayerManager.instance.HaveMoney(price) == false)
            return;

        //遍历所有应该解锁的技能
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            //如果有技能没有解锁
            if (!shouldBeUnlocked[i].unlocked)
            {
                Debug.Log("不能解锁改技能");
                return;
            }
        }

        //遍历所有锁着的同级技能
        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            //如果有技能解锁了 则不能再解锁改技能
            if (shouldBeLocked[i].unlocked)
            {
                Debug.Log("不能解锁改技能");
                return;
            }
        }

        unlocked = true;
        skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillName , skillDescription,price);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName,out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
            _data.skillTree.Add(skillName,unlocked);
    }
}
