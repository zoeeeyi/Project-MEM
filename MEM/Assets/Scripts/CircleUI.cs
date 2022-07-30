using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircleUI : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI amount;

    public int currentValue, maxValue;


    // Start is called before the first frame update
    void Start()
    {
        fill.fillAmount = Normalise();
        amount.text = $"{ currentValue}/{ maxValue}";


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            Add(10);
        }
    }


    public void Add(int val)
    {
        currentValue += val;

        if(currentValue > maxValue)
        {
            currentValue = maxValue;
        }
        fill.fillAmount = Normalise();
        amount.text = $"{ currentValue}/{ maxValue}";

    }



private float Normalise() {
        return (float)currentValue / maxValue;

    }



}

