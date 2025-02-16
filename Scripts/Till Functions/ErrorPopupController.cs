using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPopupController : MonoBehaviour
{
    public Text descText;

    public void SetData(string message) { descText.text = message; } //Sets the message for the popup

    public void DestroyObject() { Destroy(gameObject); } //Destroys the popup
}
