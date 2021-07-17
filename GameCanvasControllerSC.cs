using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasControllerSC : MonoBehaviour
{
    // ~~ Username ~~
    public Text usernameText;

    public Button saveButton;

    // ~~ Food UI ~~
    public Slider foodSlider;

    [SerializeField] Button foodButton;

    [SerializeField] Text foodTXT;

    public float foodValue = 90;

    // ~~ Water UI ~~
    public Slider waterSlider;

    [SerializeField] Button waterButton;

    [SerializeField] Text waterTXT;

    public float waterValue = 90;

    // ~~ Money UI ~~
    public Slider moneySlider;

    [SerializeField] Button moneyButton;

    [SerializeField] Text moneyTXT;

    public float moneyValue = 90;

    // ~~ Managers ~~
    private AuthManager authManagerSC;

    public RealtimeDatabse realtimeDB;

    // Start is called before the first frame update
    void Start()
    {
        authManagerSC = GameObject.FindGameObjectWithTag("Authentication").GetComponent<AuthManager>();

        usernameText.text = authManagerSC.usernameReference;

        realtimeDB.ReadData();

        StartCoroutine(StartUpdateScore());

    }

    private IEnumerator StartUpdateScore()
    {
        Debug.Log("Waiting...");

        yield return new WaitForSeconds(1f);

        if (foodValue <= 0 || waterValue <= 0 || moneyValue <= 0)
        {
            foodValue = 90;

            waterValue = 90;

            moneyValue = 90;

        }

        foodSlider.value = foodValue;

        waterSlider.value = waterValue;

        moneySlider.value = moneyValue;

        Debug.Log("Updated Sliders.");

        UpdateText();

        StartCoroutine(StartRemovingScore());

    }

    private IEnumerator StartRemovingScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(6f);

            foodSlider.value -= 10;

            waterSlider.value -= 10;

            moneySlider.value -= 10;

            UpdateText();

        }

    }

    public void UpdateText()
    {
        Debug.Log("Entered Text Update.");

        foodTXT.text = "" + foodSlider.value;

        waterTXT.text = "" + waterSlider.value;

        moneyTXT.text = "" + moneySlider.value;

        Debug.Log("Ended Text Update.");

    }

    public void OnClickFoodButton()
    {
        foodSlider.value += 1;

        UpdateText();

    }

    public void OnClickWaterButton()
    {
        waterSlider.value += 1;

        UpdateText();

    }

    public void OnClickMoneyButton()
    {
        moneySlider.value += 1;

        UpdateText();

    }

}
