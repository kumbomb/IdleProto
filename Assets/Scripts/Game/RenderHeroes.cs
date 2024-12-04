using System.Collections.Generic;
using UnityEngine;

public class RenderHeroes : MonoBehaviour
{
    public Transform[] pos; 
    public GameObject[] addParticles;
    [SerializeField] Transform lookAtPivot;
    public Dictionary<string, bool> heroDic = new(); // 중복 배치 체크용
    public void GetParticles(bool mFlag)
    {
        for(int i=0;i<addParticles.Length;i++)
        {
            addParticles[i].SetActive(mFlag);
        }   
    }

    public void InitHero()
    {
        foreach(var item in BaseManager.Char.m_Set_Character)
        {
            if(item.Value != null)
            {
                string charName = item.Value.Data.charcterName;
                if(!heroDic.ContainsKey(charName) || heroDic[charName] == false)
                {
                    heroDic.Add(charName, true);
                    var go = Instantiate(Resources.Load<GameObject>("Character/"+charName));
                    go.GetComponent<Player>().enabled = false;
                    go.transform.SetParent(transform);
                    go.transform.position = pos[item.Key].position;
                    go.transform.LookAt(lookAtPivot);
                }
            }
        }
    }

}
