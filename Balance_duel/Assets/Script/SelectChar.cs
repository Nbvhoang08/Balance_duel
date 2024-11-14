using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectChar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Data data;
    public List<Button> buttons ;
    public Image Char;
    private void Start ()
    {
       
            // Gán sự kiện click cho từng nút trong danh sách
            for (int i = 0; i < buttons.Count; i++)
            {
                int index = i; // Lưu trữ chỉ số hiện tại để sử dụng trong lambda
                buttons[i].onClick.AddListener(() => SelectCharacter(index));
            }
    }
    void Update()
    {
        updateChar();
    }
    public void updateChar()
    {
        if (data)
        {
            Char.sprite = data.skinData.fullBody;
        }
    }
    
    private void SelectCharacter(int index)
    {
        if (index >= 0 && index < data.dataList.Count)
        {
            data.skinData =data.dataList[index];        
        }
    }
}
