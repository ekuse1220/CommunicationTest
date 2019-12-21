//socket通信版
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using UnityEngine.XR.WSA.WebCam;
using System.Threading;
#if !UNITY_EDITOR
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public class CameraProcessing2 : MonoBehaviour
{

    public VideoCapture mVideoCapture = null;
    public Texture2D texture2D;
    public WebCamTexture webCamTexture;
    public GameObject plane;
    public GameObject text;
    public RawImage rawImage;
    public Thread thread;

    static string address = "163.221.174.236"; //接続先のIPv4 address
    static int port = 50011; //接続先のポート番号
    Socket socket;
    Color[] colors;
    // Use this for initialization
    void Start()
    {
        //初期化
        //StartVideoCaptureTest();



        //カメラデバイスを取得
        WebCamDevice[] devices = WebCamTexture.devices;
        //カメラデバイスが存在するとき
        if (devices.Length > 0)
        {
            //第一引数：1個目に確認されたカメラデバイスを使用，第二三引数：解像度指定
            webCamTexture = new WebCamTexture(devices[0].name, 480, 640);
            Debug.Log(webCamTexture.width + ":" + webCamTexture.height);
            //string str = webCamTexture.width.ToString() + ":" + webCamTexture.height.ToString();
            //text.GetComponent<TextMesh>().text = str;
            rawImage.texture = webCamTexture;
            webCamTexture.Play();
            //スレッド起動a
            StartCoroutine(Upload());
        }

    }
    IEnumerator Upload()
    {


        yield return new WaitForSeconds(1);
        while (true)
        {
            byte[] bytes = new byte[921600];
            int count = 0;
            foreach (Color i in colors)
            {
                bytes[count] = (byte)(i.b * 255);
                bytes[count + 1] = (byte)(i.g * 255);
                bytes[count + 2] = (byte)(i.r * 255);
                count += 3;
            }
            Debug.Log(bytes.Length);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            WWWForm form = new WWWForm();
            form.AddBinaryData("img", bytes);
            string text = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(text.Length);
            //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
            //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

            UnityWebRequest www = UnityWebRequest.Post("http://163.221.174.236:50020/post", form);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }


    void Update()
    {

        colors = webCamTexture.GetPixels();
    }


    //マルチスレッド処理
    // 初期化処理
    // スレッドを生成し、スタートさせておく
    //private void Initialize()
    //{
    //    _thread = new Thread(ThreadRun);
    //    _thread.IsBackground = true;
    //    _thread.Start();
    //}



    //void StartVideoCaptureTest()
    //{
    //    //OrderByDescendingはVideoCapture.SupportedResolutionsリストを降順にならべる
    //    Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
    //    Debug.Log(cameraResolution);

    //    float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
    //    Debug.Log(cameraFramerate);

    //    VideoCapture.CreateAsync(false, delegate (VideoCapture videoCapture)
    //    {
    //        if (videoCapture != null)
    //        {
    //            mVideoCapture = videoCapture;
    //            Debug.Log("Created VideoCapture Instance!");

    //            CameraParameters cameraParameters = new CameraParameters();
    //            cameraParameters.hologramOpacity = 0.0f;
    //            cameraParameters.frameRate = cameraFramerate;
    //            cameraParameters.cameraResolutionWidth = cameraResolution.width;
    //            cameraParameters.cameraResolutionHeight = cameraResolution.height;
    //            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

    //            mVideoCapture.StartVideoModeAsync(cameraParameters,
    //                                               VideoCapture.AudioState.ApplicationAndMicAudio,
    //                                               OnStartedVideoCaptureMode);
    //        }
    //        else
    //        {
    //            Debug.LogError("Failed to create VideoCapture Instance!");
    //        }
    //    });
    //}

    //void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    //{
    //    Debug.Log("Started Video Capture Mode!");
    //    string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
    //    string filename = string.Format("TestVideo_{0}.mp4", timeStamp);
    //    string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);
    //    filepath = filepath.Replace("/", @"\");
    //    m_VideoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
    //}

    // Update is called once per frame

}
