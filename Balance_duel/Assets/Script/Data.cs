using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Data", order = 2)]
[SerializeField]
public class Data : ScriptableObject
{
    public List<SkinData> dataList ;
    public SkinData skinData;

    private void OnEnable() 
    {
        if (dataList != null && dataList.Count > 0)
        {
            skinData = dataList[0]; 
        }
           
    }

}
[System.Serializable]
public class SkinData 
{
    public Sprite fullBody;
    public Sprite head;
    public Sprite body;
    public Sprite leg;
    public Sprite Weapon;
}
