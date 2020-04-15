using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text StateText;
    int coins;
    // Start is called before the first frame update
    void Start()
    {
        coins = 0;
        TMPro.TMP_Text[] tt_arr = (TMPro.TMP_Text[])GameObject.FindObjectsOfType(typeof(TMPro.TMP_Text));
        for (int i = 0; i < tt_arr.Length; i++)
        {
            if(tt_arr[i].CompareTag("stateTag"))
            {
                StateText = tt_arr[i];
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pickedCoinUp()
    {
        coins++;
        StateText.text = coins.ToString();
    }

    public void setWallet(int numCoins)
    {
        coins = numCoins;
    }

    public int getWallet()
    {
        return coins;
    }
}
