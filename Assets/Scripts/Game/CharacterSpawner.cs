using System.Collections.Generic;
using UnityEngine;

//최대 영웅 배치는 클레릭을 제외하고 6마리
public class CharacterSpawner : MonoBehaviour
{
    public Transform[] spawnTransform = new Transform[6];
    public static Dictionary<string, Player>players = new();
    void Awake()
    {
        for(int i =0;i<spawnTransform.Length;i++)
        {
            spawnTransform[i] = transform.GetChild(i);
        }
        StageManager.mReadyEvent += OnReady;
    }
    public void OnReady()
    {
        foreach(var data in BaseManager.Char.m_Set_Character)
        {
            if(data.Value != null)
            {
                string temp = data.Value.Data.charcterName;

                //중복 생성 방지
                if(players.ContainsKey(temp))
                {
                    //이미 존재중이면 파괴하고 재 생성
                    if(players[temp].CH_Data != data.Value.Data)
                    {
                       Destroy(players[temp].gameObject);
                       MakeHero(data.Value, data.Key);
                    }
                }
                //최초 생성
                else
                {
                    MakeHero(data.Value, data.Key);
                }
            }
        }
    }
    //영웅 생성 로직
    void MakeHero(Character_Holder data, int i)
    {
        string temp = data.Data.charcterName;
        var go = Instantiate(Resources.Load<GameObject>("Character/"+temp));
        players.Add(temp,go.GetComponent<Player>());
        go.transform.position = spawnTransform[i].position;
        go.transform.LookAt(Vector3.zero);
    }
}
