using System.Net.Sockets;
using System;
using UnityEngine;
using System.Text;
using TMPro;
using System.Collections;

public class TCPCilent : MonoBehaviour
{
    [SerializeField] TMP_InputField dataInput;
    public bool isConnected;
    public float interval;
    
    TcpClient client;
    NetworkStream stream;
    string msg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {

        //1.������ ����
        client = new TcpClient("127.0.0.1", 12345);
        print("������ �����");

        //2.��Ʈ��ũ ��Ʈ�� ���
        stream = client.GetStream();
        }
        catch(Exception ex)
        {
            print(ex);
            print("������ ���� �۵����� �ּ���.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnectRtnClkEvent()
    {
        msg = Request("Connect"); //connect �� ��� ok
        
        if( msg == "CONNECTED")
        {
            isConnected = true;

            StartCoroutine(CoRequest());
        }

    }

    public void OnDisconnectBtnClkEvent()
    {
        msg = Request("DisConnect"); // disconnected �� ��� ok

        if ( msg == "connected")
        {
            isConnected = false;
        }
    }

  

    public void Connect()
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataInput.text);
        stream.Write(dataBytes, 0, dataBytes.Length);

        //�����κ��� ������ �б�
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("����: " + response);
    }

    IEnumerator CoRequest() //�ڷ�ƾ- �ݺ�
    {
        while (isConnected)
        {
            //1. MPS�� X ����̽� ������ ���������� �����Ѵ�.

            //2. PLC�� Y ����̽� ������ 2���� ���·� �޴´�. 


            string data = Request("Temp");

            yield return new WaitForSeconds(interval);
        }
    }
    public void Request()//(������ ��ǲ �ؽ�Ʈ.)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataInput.text);
        stream.Write(dataBytes, 0, dataBytes.Length);

        //�����κ��� ������ �б�
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("����: " + response);
    }
    

    public string Request(string message)//(���ڿ��� �����ڴ�.)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(message);
        stream.Write(dataBytes, 0, dataBytes.Length);

        //�����κ��� ������ �б�
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("����: " + response);

        return response;
    }
    private void OnDestroy()
    {
        Request("Disconnect&Quit");

        if (isConnected)
        { 
            isConnected = false;
        }
    }
}