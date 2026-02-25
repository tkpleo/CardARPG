using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
public class AddMenuToList : MonoBehaviour
{
    [SerializeField] public List<GameObject> menuList;

    [SerializeField] private GameObject firstMenu;

    private void Start()
    {
        if (firstMenu != null)
        {
            AddMenu(firstMenu);
        }
    }

    public void AddMenu(GameObject menu)
    {
        if (!menuList.Contains(menu))
        {
            menuList.Add(menu);
            menu.SetActive(true);
            menu.transform.SetAsLastSibling(); // Ensure the menu appears on top of other UI elements
        }
    }

    public void RemoveMenu(GameObject menu)
    {
        if (menuList.Contains(menu))
        {
            menuList.Remove(menu);
            menu.SetActive(false);
        }
    }
}
