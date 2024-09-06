using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HoverText : MonoBehaviour
{
    private GameObject hoverTextPrefab;
    public string title = "";
    public string text = "";
    public string condition = "";
    UIControl uiControl;

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && Controller.Condition(condition))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject == gameObject)
            {
                if (hoverTextPrefab != null)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    hoverTextPrefab.transform.position = new Vector3(mousePos.x + 1.25f, mousePos.y - 1, -2.0f);
                }
                else Open(title, text);
            }
            else if (hoverTextPrefab != null)
                Close();
        }
        else if (hoverTextPrefab != null)
            Close();
    }
    public void Open(string title, string text)
    {
        uiControl = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIControl>();
        hoverTextPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/HoverText"), Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity, GameObject.FindGameObjectWithTag("Canvas").transform);
        hoverTextPrefab.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        hoverTextPrefab.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
    }

    public void Close()
    {
        Destroy(hoverTextPrefab);
    }
}
