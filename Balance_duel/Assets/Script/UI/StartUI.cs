using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
   void Start()
        {
            UIManager.Instance.OpenUI<HomeCanvas>();
        }
}
