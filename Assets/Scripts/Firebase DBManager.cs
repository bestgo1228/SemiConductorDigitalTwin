using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using System.IO;
using System.Collections.Generic;




public class FirebaseDBManager : MonoBehaviour
{
    private DatabaseReference reference;
    public FirebaseDBManager DBManager;

    // Firebase���� ���� �� ����� ����
    [SerializeField] string dbURL;
    [SerializeField] List<Data> ����;
   

    public class Data
    {
        public string consumption;
        public string temperature;
        public string humidity;

    }


    void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);

    }

    // Firebase�� ������ ������ (EnergyConsumption, Temperature, Humidity)
    void SendDataToFirebase(string consumption, string temperature, string humidity)
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // ������ ����
        reference.Child("Data").Child(timeStamp).Child("EnergyConsumption").SetValueAsync(consumption);
        reference.Child("Data").Child(timeStamp).Child("Temperature").SetValueAsync(temperature);
        reference.Child("Data").Child(timeStamp).Child("Humidity").SetValueAsync(humidity);

        Debug.Log("�����Ͱ� Firebase�� ���۵Ǿ����ϴ�.");
    }



    void SendJsonFile(string filePath, string refName)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                string json = sr.ReadToEnd();

                DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                dbRef.Child(refName).Child(refName).Child(refName).SetRawJsonValueAsync(json);
            }
        }
    }

    public void SetRawJsonValueAsync(string jsonFile) //
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        dbRef.SetRawJsonValueAsync(jsonFile);
    }

    string totalInfo = "";

    
   
}
