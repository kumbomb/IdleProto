using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Collections;

//Down : 눌러있는 동안 호출 
//Up : 눌려있다가 떼졌을 때 호출
public class LvUpBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image mExpSlider;
    [SerializeField] TextMeshProUGUI expText, atkText, hpText, getExpText;

    bool isClick = false;
    float timer = 0f;
    Coroutine clickCoroutine;


    private void Start() {
        InitExp();
    }

    private void Update()
    {
        if(isClick)
        {
            timer += Time.deltaTime;
            if(timer >= 0.01f)
            {
                EXPUP();
                timer = 0f;
            }
        }
    }

    public void EXPUP()
    {
        BaseManager.Hero.EXPUP();
        InitExp();
        transform.DORewind();
        transform.DOPunchScale(new Vector3(0.1f,0.1f,0.1f), 0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EXPUP();
        clickCoroutine = StartCoroutine(Co_ClickBtn());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClick = false;
        if(clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
        }
        timer = 0f;
    }

    IEnumerator Co_ClickBtn()
    {
        yield return new WaitForSeconds(1f);
        isClick =true;
    }

    void InitExp()
    {
        mExpSlider.fillAmount = BaseManager.Hero.GetEXPPer();
        expText.text = $"{(BaseManager.Hero.GetEXPPer() * 100f).ToString("F2")}%";
        atkText.text = $"+{StringMethod.ToCurrencyString(BaseManager.Hero.Next_Atk())}";
        hpText.text = $"+{StringMethod.ToCurrencyString(BaseManager.Hero.Next_Hp())}";
        getExpText.text = $"<color=#00FF00>EXP</color> +{BaseManager.Hero.Next_Exp().ToString("F2")}%";
    }

}
