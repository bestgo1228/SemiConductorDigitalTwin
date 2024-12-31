using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.IO;


public class MPSManager : MonoBehaviour
{


    private bool isCollectingData = false;
    private DatabaseReference reference;

    public FirebaseDBManager DBManager;

    // ������ ����
    private float consumption;
    private float temperature;
    private float humidity;
    public string Name = "��ũ�ؼ�1ȣ";

    // ������ ���� �� Firebase�� �߰�
    string firebaseCode = "";  // ���� �ڵ�
    string statusMessage = "";  // ���� �޽���
    string path = "";

    [SerializeField] int StartButtonState = 0;
    [SerializeField] int StopButtonState = 0;
    [SerializeField] int EStopButtonState = 0;

    [SerializeField] TMP_InputField EnergyConsumption;
    [SerializeField] TMP_InputField Temperature;
    [SerializeField] TMP_InputField Humidity;
    [SerializeField] TMP_Text UpdateLogText;

    [SerializeField] GameObject[] uiObjects;

    private int currentChipIndex = 0;  // �˻��� Ĩ�� �ε����� �����ϴ� ����
    private List<int> coloredChips = new List<int>(); // ���� ����� ��ü���� �����ϴ� ����Ʈ

    private int currentWaferIndex = 1;
    private bool isInspectionComplete = false;  // �˻簡 �Ϸ�� ���¸� �����ϴ� ����

    void Start()
    {
       

        InvokeRepeating("UpdateData", 2, 1); //2���� ����, ���� 1�� �������� ȣ��

        GenerateRandomData();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
        });

    }

    private float lastUpdateTime = 0f;
    public float updateInterval = 10f;

    void Update()
    {

        // �����͸� �����ϴ� ���̸� �������� ������ ����
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            if (isCollectingData)
            {
                lastUpdateTime = Time.time; //������ ������Ʈ �ð��� ���� �ð����� ����
                GenerateRandomData();
                UpdateUI();
            }
        }
   
    }


    // Start ��ư Ŭ���� ������ ���� ����
    // Start ��ư Ŭ�� �� ù ��° ������ �˻� ���� �� Ĩ ���� ����
    public void OnStartButtonClicked()
    {
        // ������ ���� ���� ����
        isCollectingData = true;
        StartButtonState = 1;
        StopButtonState = 0;

        print("1ȣ �˻� ����");

        // ù ��° Ĩ�� ������ ���� (�˻� ����)
        ChangeChipUIColorBasedOnTemperature();

        // ù ��° ������ �˻� ���� �� 2�� �ں��� 10�ʸ��� ����ؼ� ���� ���� �۾��� ����
        InvokeRepeating("UpdateChipInspection", 2f, 1f); // 2�� �� �����ϰ� 5�ʸ��� ��� ȣ��
    }
    // Stop ��ư Ŭ���� 3�� �Ŀ� ������ ���� ����
    public void OnStopButtonClicked()
    {

        StartButtonState = 1;
        StopButtonState = 0;

        if (isCollectingData)
        { 
        StartCoroutine(StopDataCollection());
        print("������ ���� ����");
        }
    }

    // 3�� �Ŀ� ������ ������ ���ߴ� �ڷ�ƾ
    
   IEnumerator StopDataCollection()
    {
        print("������ ���� ������ 3�� �� �����մϴ�");  // "������ ���� ������ �����մϴ�" �޽��� ǥ��
        isCollectingData = false;

        yield return new WaitForSeconds(1f);
        print("3");  // 3�� �� ǥ��

        yield return new WaitForSeconds(1f);
        print("2");  // 2�� �� ǥ��

        yield return new WaitForSeconds(1f);
        print("1");  // 1�� �� ǥ��

        yield return new WaitForSeconds(1f);
        print("�����Ϸ�");  // "�����Ϸ�" �޽��� ǥ��

        // ���� ������ �˻� �Ϸ� �� ��ȯ
        if (currentWaferIndex >= 1 && currentWaferIndex < 10)
        {
            isInspectionComplete = true;  // ���� ������ �˻� �Ϸ�
            print($"������ {currentWaferIndex}ȣ �˻� �Ϸ�");

            // ���� �����۷� ��ȯ (1 ~ 9ȣ���� ���� ��ȣ�� �Ѿ)
            currentWaferIndex++;
            isInspectionComplete = false;  // ���ο� ������ ���� ���� �� �˻� �Ϸ� ���� ����
            print($"������ {currentWaferIndex}ȣ ���� ����");
        }
        else if (currentWaferIndex == 10)
        {
            isInspectionComplete = true;  // ������ 10ȣ �˻� �Ϸ�
            print("������ 10ȣ �˻� �Ϸ�");

            // ������ 1ȣ�� ���ư��� �ٽ� ���� ����
            currentWaferIndex = 1;
            isInspectionComplete = false;  // �˻� �Ϸ� ���� ����
            print("������ 1ȣ�� ���ư��� ���� ����");
        }
    }

    // E-stop ��ư Ŭ���� ��� ������ ���� ����
    public void OnEStopButtonClicked()
    {
        EStopButtonState = (EStopButtonState == 1) ? 0 : 1;
        StartButtonState = 0;
        StopButtonState = 0;

        isCollectingData = false;
        print("��� ����");
    }

    // Ĩ �˻� �� ���� ������ ������Ʈ�ϴ� �Լ�
    private void UpdateChipInspection()
    {
        // �����͸� ���� ���̶�� ����ؼ� �˻� ����
        if (isCollectingData)
        {
            // Ĩ �˻� �Լ� ȣ��
            ChangeChipUIColorBasedOnTemperature();
        }
    }

    // ���� ������ ���� �Լ�
    public void GenerateRandomData()
    {
        consumption = UnityEngine.Random.Range(0f, 1000f); // ��: 0 ~ 1000
        temperature = UnityEngine.Random.Range(-20f, 50f);  // ��: -20 ~ 50��C
        humidity = UnityEngine.Random.Range(30f, 90f);      // ��: 30 ~ 90%
    }
    public void SetRawJsonValueAsync(string path, string json)
    {
        // Firebase�� �����͸� �����ϴ� �ڵ�
        FirebaseDatabase.DefaultInstance
            .RootReference
            .Child(path)
            .SetRawJsonValueAsync(json);
    }
    public void AddDataToFirebase(string json, int chipIndex)
    {
        // �µ��� ���� �ҷ� ���� (���, ����)
        string folderPath = "";

        if (temperature > 45f)
        {
            // ��� �ҷ� �߻�
            folderPath = $"Real-time Information/������ 1/�ҷ� �߻�/(���)Code 001/Chip {chipIndex + 1}";
        }
        else if (temperature < -15f)
        {
            // ���� �ҷ� �߻�
            folderPath = $"Real-time Information/������ 1/�ҷ� �߻�/(����)Code 002/Chip {chipIndex + 1}";
        }
        else
        {
            // ��ǰ ����
            folderPath = $"Real-time Information/������ 1/��ǰ����/Chip {chipIndex + 1}";
        }

        // Firebase�� �ش� ��η� ������ ����
        reference.Child(folderPath).SetRawJsonValueAsync(json);
    }

    public void UpdateUI()
    {
        // TMP_InputField�� �����͸� ǥ��
        EnergyConsumption.text = " " + consumption.ToString("F2") + "kWh";
        Temperature.text = " " + temperature.ToString("F2") + "��C";
        Humidity.text = " " + humidity.ToString("F2") + "%";

        // �гο� ���� ������Ʈ ǥ��
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // ���� �ð��� "yyyy-MM-dd HH:mm:ss" �������� ������

        string newLog = "���� ������Ʈ �Ϸ�\n" +
                             "EnergyConsumption : " + consumption.ToString("F2") + "kWh\n" +
                             "Temperature : " + temperature.ToString("F2") + "��C\n" +
                             "Humidity : " + humidity.ToString("F2") + "%";

        // ������ �α׿� ���ο� ������ �߰�
        UpdateLogText.text += newLog;  // ���� ���� �ڿ� ���ο� �α׸� �߰�

        // ���� �ð� ǥ��
        UpdateLogText.text = "CurrentTime\n" + currentTime;  // �ؽ�Ʈ UI�� ���� �ð� ǥ��

        string json = $@"{{
                ""Data"":[
                ],
                ""consumption"":""{consumption.ToString("F2") + "kWh"}"",
                ""Temperature"":""{temperature.ToString("F2") + "��C"}"",
                ""Humidity"":""{humidity.ToString("F2") + "%"}"",
                ""�˻���"":""{Name}""
            }}";


        AddDataToFirebase(json, currentChipIndex); //firebase�� ������ �߰�
        // DBManager.SetRawJsonValueAsync(json);

        // �µ��� 30���� �ʰ��ϸ� �������� ��ü ���� ����
        //ChangeRandomUIColorBasedOnTemperature();


    }
    // Ĩ UI ���� ���� �Լ�
    private void ChangeChipUIColorBasedOnTemperature()
    {
        // �˻��� Ĩ�� ���� ������ ���������� �˻�
        if (currentChipIndex < 37)
        {
            // ���� �˻��� Ĩ
            int chipIndex = currentChipIndex;

            // ���õ� UI ��ü���� RawImage ������Ʈ�� ã�Ƽ� ������ ����
            if (uiObjects[chipIndex] != null)
            {
                RawImage rawImage = uiObjects[chipIndex].GetComponent<RawImage>();
                if (rawImage != null)
                {
                    // �µ��� ���� ���� ����
                    if (temperature > 45f)
                    {
                        rawImage.color = Color.red; // �µ��� 45���� �ʰ��ϸ� ������
                        print($"�ҷ� �߻� : Code:001 (Chip {chipIndex + 1})");
                    }
                    else if (temperature < -15f)
                    {
                        rawImage.color = Color.blue; // �µ��� -15�� �̸��̸� �Ķ���
                        print($"�ҷ� �߻� : Code:002 (Chip {chipIndex + 1})");
                    }
                    else
                    {
                        rawImage.color = Color.green; // �µ��� -15~45�� ���̸� �ʷϻ�
                        print($"��ǰ ���� : Code:000 (Chip {chipIndex + 1})");
                    }

                    // ������ ����� ��ü�� coloredChips�� �߰�
                    coloredChips.Add(chipIndex); // ���� ����� chip�� �ε����� �߰��Ͽ� ���� ������� �ʵ��� ��
                }
            }

            // �˻��� Ĩ �ε��� ����
            currentChipIndex++;

            // ��� Ĩ�� �˻� �Ϸ�Ǹ� "�˻� �Ϸ�" �޽��� ���
            if (currentChipIndex >= uiObjects.Length)
            {
                print("��� Ĩ �˻� �Ϸ�");
                isInspectionComplete = true;
            }
        }
    }


    public void UpdateData()
        {
            if (isCollectingData)
            {
                GenerateRandomData();
                UpdateUI();

                print("���� ������Ʈ �Ϸ�");
            }

        }
}
