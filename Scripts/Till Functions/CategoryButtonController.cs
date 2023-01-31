using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CategoryButtonController : MonoBehaviour
{
    [Header("Catagories")]
    public Category buttonCategory;

    [Header("References")]
    private ClientController clientController;

    // Start is called before the first frame update
    void Start()
    {
        clientController = FindObjectOfType<ClientController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Updates the detials on the button object
    public void UpdateButton()
    {
        gameObject.GetComponentInChildren<Text>().text = buttonCategory.categoryName;
        gameObject.GetComponent<Image>().color = buttonCategory.categoryColour;
    }

    //Called when the button is pressed
    public void OnPress()
    {
        //Changes the active category and updates the item buttons
        clientController.instance.activeCategory = buttonCategory;
        clientController.instance.CreateCategoryItemButtons();
    }
}
