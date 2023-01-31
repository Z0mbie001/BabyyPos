using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaffAdd : MonoBehaviour
{
    [Header("UI Inputs")]
    public InputField idInput;
    public InputField firstNameInput;
    public InputField lastNameInput;
    public InputField dateOfBirthInput;
    public InputField startDateInput;
    public InputField endDateInput;
    public InputField permissionsLevelInput;

    //Confirms the inputs (validation)
    public void ConfirmInputs()
    {
        DateTime dobDT;
        DateTime startDT;
        DateTime endDT;
        if(idInput.text == "" || firstNameInput.text == "" || lastNameInput.text == "" || dateOfBirthInput.text == "" || startDateInput.text == "" || endDateInput.text == "" || permissionsLevelInput.text == "")
        {
            return;
        }else if(!DateTime.TryParse(dateOfBirthInput.text, out dobDT) || !DateTime.TryParse(startDateInput.text, out startDT) || !DateTime.TryParse(endDateInput.text, out endDT))
        {
            return;
        }
        string q_toSend = "&STAFFWR|" + idInput.text + "|" + firstNameInput.text + "|" + lastNameInput.text + "|" + dobDT.ToString("s") + "|" + startDT.ToString("s") + "|" + endDT.ToString("s") + "|" + permissionsLevelInput.text;
        FindObjectOfType<Client>().instance.toSend.AddLast(q_toSend);
        CloseWindow();
    }

    //Closes the panel
    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}
