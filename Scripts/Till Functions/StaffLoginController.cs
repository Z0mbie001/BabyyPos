using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffLoginController : MonoBehaviour
{
    public StaffLoginController instance;

    [Header("Input Fields")]
    public InputField staffIDInput;

    [Header("Staff members")]
    public List<StaffMember> returnedStaffMembers = new List<StaffMember>();
    public bool allStaffMembersReturned;

    [Header("GameObjects")]
    public GameObject staffLoginPanel;

    [Header("References")]
    private Client client;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        client = FindObjectOfType<Client>();  
    }

    //Called when the button is pressed
    public void OnPress()
    {
        if(staffIDInput.text == "")
        {
            Debug.Log("No ID Input detected");
            return;
        }
        else
        {
            StartCoroutine(StaffLogin());
        }
    }

    //Processes the staff login, sending the request and recieving the returned data
    IEnumerator StaffLogin()
    {
        string q_toSend = "&STAFFLOGINDBR|" + staffIDInput.text;
        staffIDInput.text = "";
        allStaffMembersReturned = false;
        returnedStaffMembers.Clear();
        client.instance.toSend.AddLast(q_toSend);
        yield return new WaitUntil(() => allStaffMembersReturned);
        if(returnedStaffMembers.Count > 1)
        {
            Debug.Log("More than 1 staff members returned");
            yield return null;
        }
        else if(returnedStaffMembers.Count <= 0)
        {
            Debug.Log("No staff members returned");
            yield return null;
        }
        else
        {
            FindObjectOfType<ClientController>().instance.activeStaffMember = returnedStaffMembers[0];
            staffLoginPanel.SetActive(false);
        }
    }    
    
    //If the button is pressed on the admin page
    public void OnPressAdmin()
    {
        if(staffIDInput.text == "")
        {
            Debug.Log("No ID Input detected");
            return;
        }
        else
        {
            StartCoroutine(StaffLoginAdmin());
        }
    }

    //Processes a staff login request for the admin page
    IEnumerator StaffLoginAdmin()
    {
        string q_toSend = "&STAFFLOGINDBR|" + staffIDInput.text;
        staffIDInput.text = "";
        allStaffMembersReturned = false;
        returnedStaffMembers.Clear();
        client.instance.toSend.AddLast(q_toSend);
        yield return new WaitUntil(() => allStaffMembersReturned);
        if(returnedStaffMembers.Count > 1)
        {
            Debug.Log("More than 1 staff members returned");
            yield return null;
        }
        else if(returnedStaffMembers.Count <= 0)
        {
            Debug.Log("No staff members returned");
            yield return null;
        }else if (returnedStaffMembers[0].permissionLevel < 4 && returnedStaffMembers[0].permissionLevel != -1)
        {
            Debug.Log("Insufficent Permissions");
            yield return null;
        }
        else
        {
            staffLoginPanel.SetActive(false);
        }
    }
}
