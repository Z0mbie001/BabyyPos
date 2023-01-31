using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaymentButtonController : MonoBehaviour
{
    [Header("Values")]
    public string paymentName;
    public float amount;

    [Header("Text fields")]
    public Text nameText;
    public Text amountText;

    //Updates button details
    public void UpdateButtonDetails()
    {
        nameText.text = paymentName;
        amountText.text = "£" + amount.ToString("0.00");
    }
}
