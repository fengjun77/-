using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Player player;

    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("弹出提示")]
    [SerializeField] private GameObject popTextPrefab;

    [Header("异常状态颜色")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chilledColor;
    [SerializeField] private Color[] shockColor;

    [Header("受击效果")]
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField] private GameObject critHitFXPrefab;

    private GameObject myHealthBar;

    protected virtual void Start()
    {
        player = GetComponent<Player>();
        sr = GetComponentInChildren<SpriteRenderer>();

        originalMat = sr.material;

        myHealthBar = GetComponentInChildren<HealthBar_UI>().gameObject;
    }

    private void Update()
    {
        
    }

    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1, 3);
        Vector3 offset = new Vector3(randomX, randomY, 0);
        GameObject newText = Instantiate(popTextPrefab, transform.position + offset, Quaternion.identity);
        
        newText.GetComponent<TextMeshPro>().text = _text;
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            myHealthBar.SetActive(false);
        }
        else
        {
            myHealthBar.SetActive(true);
        }
    }

    protected IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(.15f);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if(sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

    public void IgniteFxFor(float _seconds)
    {
        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ChillFxFor(float _seconds)
    {
        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }



    private void ChillColorFx()
    {
        if (sr.color != chilledColor[0])
            sr.color = chilledColor[0];
        else
            sr.color = chilledColor[1];
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    public void CreateHitFX(Transform _target,bool _crit)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        Vector3 hitFXRotation = new Vector3(0,0,zRotation);

        GameObject hitPrefab = hitFXPrefab;

        if(_crit)
        {
            hitFXPrefab = critHitFXPrefab;

            float yRotation = 0;
            zRotation = Random.Range(-45,45);

            if (GetComponent<Entity>().facingDir == -1)
                yRotation = 180;

            hitFXRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newHitFX = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity,_target);

        newHitFX.transform.Rotate(hitFXRotation);

        Destroy(newHitFX,.5f);
    }
}
