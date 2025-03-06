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
        private int listenPort = 12345; // 수신 포트

        [SerializeField]
        private Camera captureCamera;   // 화면 캡처를 위한 카메라 (null일 경우 기본 카메라 사용)

        private Texture2D screenTexture;
        private int captureWidth;   // 캡처할 화면의 너비
        private int captureHeight;  // 캡처할 화면의 높이

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

            // 화면 캡처를 위한 Texture2D 객체 생성 (해상도 낮춤)
            screenTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

            // UDP 서버 초기화
            udpServer = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(listenIp), listenPort);

            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);

            // 10 FPS로 화면 캡처 및 전송 (30 FPS -> 10 FPS로 낮춤)
            //InvokeRepeating("CaptureAndSendScreen", 0, 1f / 10f);
            StartCoroutine(CaptureAndSendScreen());
        }

        void OnApplicationQuit()
        {
            byte[] temp = { (byte)(127 + id) };
            udpServer.Send(temp, temp.Length, endPoint);
            // 애플리케이션 종료 시 소켓 닫기
            udpServer?.Close();
        }

        IEnumerator CaptureAndSendScreen()
        {
            while (true)
            {
                // 화면 캡처
                CaptureScreenUsingRenderTexture();

                // 캡처한 이미지를 JPG로 인코딩하여 전송
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


                // UDP로 전송
                //udpServer.Send(combinedBytes, combinedBytes.Length, endPoint);

                //소캣 오류시 재할당 하여 전송
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

                    // 소켓을 다시 생성하여 복구
                    udpServer?.Close();
                    udpServer = new UdpClient();
                    endPoint = new IPEndPoint(IPAddress.Parse(listenIp), listenPort);
                }

                yield return new WaitForSecondsRealtime(0.2f);
            }
        }

        void CaptureScreenUsingRenderTexture()
        {
            // 카메라의 출력 텍스처를 변경
            captureCamera.targetTexture = renderTexture;

            // 카메라로 렌더링 수행
            captureCamera.Render();

            // 텍스처를 Texture2D로 복사
            RenderTexture.active = renderTexture;
            screenTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            screenTexture.Apply();

            // RenderTexture 비활성화
            RenderTexture.active = null;
            captureCamera.targetTexture = null;  // 카메라의 출력 텍스처 초기화
        }
    }

}