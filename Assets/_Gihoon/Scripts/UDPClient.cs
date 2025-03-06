using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

namespace GH
{
    public class UDPClient : MonoBehaviour
    {
        private UdpClient udpServer;
        private IPEndPoint endPoint;

        [SerializeField]
        private string listenIp = "127.0.0.1";

        [SerializeField]
        private int listenPort = 12345; // ���� ��Ʈ

        [SerializeField]
        private Camera captureCamera;   // ȭ�� ĸó�� ���� ī�޶� (null�� ��� �⺻ ī�޶� ���)

        private Texture2D screenTexture;
        private int captureWidth;   // ĸó�� ȭ���� �ʺ�
        private int captureHeight;  // ĸó�� ȭ���� ����

        [SerializeField]
        byte id = 1;

        byte[] combinedBytes;

        RenderTexture renderTexture;

        private void Awake()
        {
            //listenIp =
#if UNITY_EDITOR

#else
    listenIp = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "TargetIP.txt")).Trim();
#endif
        }

        void Start()
        {
            /*
            //Screen.SetResolution(1920, 1080, true);
            var temp = GetComponent<IUIManager>().GetScreenPixel();
            captureWidth = (int)(temp.x / 4);
            captureHeight = (int)(temp.y / 4);
            */

            Screen.SetResolution(1080, 1920, true);

            var temp = new Vector2(Screen.width, Screen.height);

            captureWidth = (int)(temp.x / 4);   // captureWidth = 364;
            captureHeight = (int)(temp.y / 4);  // captureHeight = 270;

            // ȭ�� ĸó�� ���� Texture2D ��ü ���� (�ػ� ����)
            screenTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

            // UDP ���� �ʱ�ȭ
            udpServer = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(listenIp), listenPort);

            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);

            // 10 FPS�� ȭ�� ĸó �� ���� (30 FPS -> 10 FPS�� ����)
            //InvokeRepeating("CaptureAndSendScreen", 0, 1f / 10f);
            StartCoroutine(CaptureAndSendScreen());
        }

        void OnApplicationQuit()
        {
            byte[] temp = { (byte)(127 + id) };
            udpServer.Send(temp, temp.Length, endPoint);
            // ���ø����̼� ���� �� ���� �ݱ�
            udpServer?.Close();
        }

        IEnumerator CaptureAndSendScreen()
        {
            while (true)
            {
                // ȭ�� ĸó
                CaptureScreenUsingRenderTexture();

                // ĸó�� �̹����� JPG�� ���ڵ��Ͽ� ����
                byte[] imageBytes = screenTexture.EncodeToJPG(50);
                //Debug.Log(imageBytes.Length);

                //combinedBytes = new byte[imageBytes.Length + 1];
                if (combinedBytes == null || combinedBytes.Length != imageBytes.Length + 1)
                {
                    combinedBytes = new byte[imageBytes.Length + 1];
                }
                else
                {
                    Array.Clear(combinedBytes, 0, combinedBytes.Length);
                }

                combinedBytes[0] = id;

                Array.Copy(imageBytes, 0, combinedBytes, 1, imageBytes.Length);


                // UDP�� ����
                //udpServer.Send(combinedBytes, combinedBytes.Length, endPoint);

                //��Ĺ ������ ���Ҵ� �Ͽ� ����
                try
                {
                    if (udpServer != null)
                    {
                        udpServer.Send(combinedBytes, combinedBytes.Length, endPoint);
                    }
                }
                catch (SocketException ex)
                {
                    Debug.LogError("UDP Send Failed: " + ex.Message);

                    // ������ �ٽ� �����Ͽ� ����
                    udpServer?.Close();
                    udpServer = new UdpClient();
                    endPoint = new IPEndPoint(IPAddress.Parse(listenIp), listenPort);
                }

                yield return new WaitForSecondsRealtime(0.2f);
            }
        }

        void CaptureScreenUsingRenderTexture()
        {
            // ī�޶��� ��� �ؽ�ó�� ����
            captureCamera.targetTexture = renderTexture;

            // ī�޶�� ������ ����
            captureCamera.Render();

            // �ؽ�ó�� Texture2D�� ����
            RenderTexture.active = renderTexture;
            screenTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            screenTexture.Apply();

            // RenderTexture ��Ȱ��ȭ
            RenderTexture.active = null;
            captureCamera.targetTexture = null;  // ī�޶��� ��� �ؽ�ó �ʱ�ȭ
        }
    }

}