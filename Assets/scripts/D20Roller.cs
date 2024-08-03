using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class D20Roller : MonoBehaviour
{
    private int d20Roll;
    private Controller controller;
    private GameObject quote;
    private GameObject rollButton;
    private GameObject number;
    private GameObject result;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        quote = transform.Find("Quote").gameObject;
        rollButton = transform.Find("Roll").gameObject;
        number = transform.Find("Number").gameObject;
        result = transform.Find("Result").gameObject;
    }
    public void Roll()
    {
        string conversion = "";
        d20Roll = Random.Range(1, 21);
        if (d20Roll == 20) { result.GetComponent<TextMeshProUGUI>().text = "Knook"; conversion = "knook"; }
        else if (d20Roll > 12) { result.GetComponent<TextMeshProUGUI>().text = "Rook"; conversion = "rook"; }
        else { result.GetComponent<TextMeshProUGUI>().text = "Knight"; conversion = "knight"; }
        number.GetComponent<TextMeshProUGUI>().text = d20Roll.ToString();
        rollButton.SetActive(false);
        quote.SetActive(true);
        result.SetActive(true);
        number.SetActive(true);
        controller.ConvertPawn(conversion);
    }
}
