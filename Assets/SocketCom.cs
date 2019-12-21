using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine.UI;


public class SocketCom : MonoBehaviour {

    //private IPAddress add = 163.221.174.236;

    static string add = "163.221.174.236"; //接続先のIPv4 address
    IPAddress iPAddress = IPAddress.Parse(add);
    static int port = 50011; //接続先のポート番号
    Socket socket;
    IPEndPoint ipe;
    [SerializeField] Texture2D texture2d; //送信画像
    string path = "Assets/rgb_img2.bmp";
    [SerializeField] Image image;
    // Use this for initialization a er
    void Start () {
        //エンドポイントの登録
        IPEndPoint ipe = new IPEndPoint(iPAddress, port);
        socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ipe);
  
        byte[] texture_byte = readPngFile(path);
        Texture tex = readByBinary(texture_byte);
        image.material.mainTexture = tex;

        Color[] colors = texture2d.GetPixels();

        byte[] bytes = new byte[921600];
        int count = 0;
        Debug.Log("color : " + (colors.Length*3).ToString());
        foreach (Color i in colors)
        {
            bytes[count] = (byte)(i.b * 255);
            bytes[count + 1] = (byte)(i.g * 255);
            bytes[count + 2] = (byte)(i.r * 255);
            count += 3;
            //if (count < 100)
            //{
            //    Debug.Log(i * 255);
            //}
        }

        Debug.Log(texture2d.height.ToString() + ":" + texture2d.width.ToString());

        if (socket.Connected)
        {
            Debug.Log("communication connected");
            // byte[] texture_byte = readPngFile(path);
            Debug.Log(bytes.Length);
            socket.Send(bytes);
            
        }
        else
        {
            Debug.Log("connection failed");
        }
        socket.Close();

    }

    //pngファイルをバイナリとして読み込む
    byte[] readPngFile(string path){
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }   

    //読み込んだバイナリをtextureにする
    public Texture readByBinary(byte[] bytes)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        return texture;
    }

    // Update is called once per frame
    //void Update() {

    //}
}
