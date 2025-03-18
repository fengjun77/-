using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeSlot_UI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISaveManager
{
    //������
    [SerializeField] private string skillName;
    [TextArea]
    //��������
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    //�Ƿ���Խ���
    public bool unlocked;

    //��Ҫ�����ļ����б�(�������б��еļ���δ����������ü����޷�����)
    [SerializeField] private SkillTreeSlot_UI[] shouldBeUnlocked;
    //�ɽ����ļ����б�
    [SerializeField] private SkillTreeSlot_UI[] shouldBeLocked;
    //����ͼ��
    private Image skillImage;

    private UI ui;

    public int price;

    private void OnValidate()
    {
        gameObject.name = "���ܲ� - " + skillName;
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

        //��������Ӧ�ý����ļ���
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            //����м���û�н���
            if (!shouldBeUnlocked[i].unlocked)
            {
                Debug.Log("���ܽ����ļ���");
                return;
            }
        }

        //�����������ŵ�ͬ������
        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            //����м��ܽ����� �����ٽ����ļ���
            if (shouldBeLocked[i].unlocked)
            {
                Debug.Log("���ܽ����ļ���");
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
