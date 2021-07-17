using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class RealtimeDatabse : MonoBehaviour
{
    DatabaseReference dbReference;

    public GameCanvasControllerSC canvasControlGO;

    public AuthManager authManager;

    public Button saveButtonGame;

    void Awake()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        authManager = GameObject.FindGameObjectWithTag("Authentication").GetComponent<AuthManager>();

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SaveData()
    {
        User user = new User();

        user.UserName = canvasControlGO.usernameText.text;

        user.foodScore = canvasControlGO.foodSlider.value.ToString();

        user.waterScore = canvasControlGO.waterSlider.value.ToString();

        user.moneyScore = canvasControlGO.moneySlider.value.ToString();

        string json = JsonUtility.ToJson(user);

        dbReference.Child("User").Child(user.UserName).SetRawJsonValueAsync(json).ContinueWith(task =>
        {

            if (task.IsCompleted)
            {
                Debug.Log("Success adding data to database!");

            }
            else
            {
                Debug.Log("Unsuccessful saving data to database.");
            }

        });

    }

    public void QuitOnClick()
    {
        Application.Quit();

    }

    public void ReadData()
    {
        dbReference.Child("User").Child(authManager.usernameReference).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                Debug.Log("username: " + snapshot.Child("UserName").Value.ToString());

                int _foodData = int.Parse(snapshot.Child("foodScore").Value.ToString());

                Debug.Log("food: " + snapshot.Child("foodScore").Value.ToString());

                int _waterData = int.Parse(snapshot.Child("waterScore").Value.ToString());

                Debug.Log("water: " + snapshot.Child("waterScore").Value.ToString());

                int _moneyData = int.Parse(snapshot.Child("moneyScore").Value.ToString());

                Debug.Log("money: " + snapshot.Child("moneyScore").Value.ToString());

                Debug.Log("Success reading the data from databse!");

                UpdateSliders((float)_foodData, (float)_waterData, (float)_moneyData);

            }
            else
            {
                Debug.Log("Unsuccessful reading the data from databse.");

            }

        });

    }



    public void UpdateSliders(float food, float water, float money)
    {
        Debug.Log("Entered Update Values Function.");

        canvasControlGO.foodValue = food;

        Debug.Log("Food Value: " + canvasControlGO.foodValue);

        canvasControlGO.waterValue = water;

        Debug.Log("Water Value: " + canvasControlGO.waterValue);

        canvasControlGO.moneyValue = money;

        Debug.Log("Money Value: " + canvasControlGO.moneyValue);

        Debug.Log("Ended Update Values Function.");

    }

    // Update is called once per frame
    void Update()
    {

    }
}
