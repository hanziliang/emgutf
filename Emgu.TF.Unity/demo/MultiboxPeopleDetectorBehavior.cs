//----------------------------------------------------------------------------
//  Copyright (C) 2004-2017 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------


using UnityEngine;
using System;

using System.Collections;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Emgu.TF;
using Emgu.TF.Models;

public class MultiboxPeopleDetectorBehavior : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private Texture2D resultTexture;
    private Color32[] data;
    private byte[] bytes;
    private WebCamDevice[] devices;
    public int cameraCount = 0;
    private bool _textureResized = false;
    private Quaternion baseRotation;
    bool _liveCameraView = false;

    // Use this for initialization
    void Start()
    {
        //Warning: The following code is used to get around a https certification issue for downloading tesseract language files from Github
        //Do not use this code in a production environment. Please make sure you understand the security implication from the following code before using it
        ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            HttpWebRequest webRequest = sender as HttpWebRequest;
            if (webRequest != null)
            {
                String requestStr = webRequest.Address.AbsoluteUri;
                if (requestStr.StartsWith(@"https://github.com/") || requestStr.StartsWith(@"https://raw.githubusercontent.com/"))
                    return true;
            }
            return false;
        };

        WebCamDevice[] devices = WebCamTexture.devices;
        int cameraCount = devices.Length;

        if (cameraCount == 0)
        {
            _liveCameraView = false;
            Texture2D texture = Resources.Load<Texture2D>("surfers");
            Tensor imageTensor = ImageIO.ReadTensorFromTexture2D(texture, 224, 224, 128.0f, 1.0f / 128.0f, true);

            byte[] raw = ImageIO.EncodeJpeg(imageTensor, 128.0f, 128.0f);
            System.IO.File.WriteAllBytes("surfers_out.jpg", raw);

            MultiboxGraph multiboxGraph = new MultiboxGraph();
            MultiboxGraph.Result results = multiboxGraph.Detect(imageTensor);
            
            Texture2D drawableTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            drawableTexture.SetPixels(texture.GetPixels());
            MultiboxGraph.DrawResults(drawableTexture, results, 0.02f);
            
            this.GetComponent<GUITexture>().texture = drawableTexture;
            this.GetComponent<GUITexture>().pixelInset = new Rect(-texture.width/2, -texture.height/2, texture.width, texture.height);
        }
        else
        {
            _liveCameraView = true;
            webcamTexture = new WebCamTexture(devices[0].name);

            baseRotation = transform.rotation;
            webcamTexture.Play();
            //data = new Color32[webcamTexture.width * webcamTexture.height];
            TfInvoke.CheckLibraryLoaded();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (_liveCameraView)
        {
            if (webcamTexture != null && webcamTexture.didUpdateThisFrame)
            {
                #region convert the webcam texture to RGBA bytes

                if (data == null || (data.Length != webcamTexture.width * webcamTexture.height))
                {
                    data = new Color32[webcamTexture.width * webcamTexture.height];
                }
                webcamTexture.GetPixels32(data);

                if (bytes == null || bytes.Length != data.Length * 4)
                {
                    bytes = new byte[data.Length * 4];
                }
                GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                Marshal.Copy(handle.AddrOfPinnedObject(), bytes, 0, bytes.Length);
                handle.Free();

                #endregion

                #region convert the RGBA bytes to texture2D

                if (resultTexture == null || resultTexture.width != webcamTexture.width ||
                    resultTexture.height != webcamTexture.height)
                {
                    resultTexture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32,
                        false);
                }

                resultTexture.LoadRawTextureData(bytes);
                resultTexture.Apply();

                #endregion

                if (!_textureResized)
                {
                    this.GetComponent<GUITexture>().pixelInset = new Rect(-webcamTexture.width / 2,
                        -webcamTexture.height / 2, webcamTexture.width, webcamTexture.height);
                    _textureResized = true;
                }

                transform.rotation = baseRotation * Quaternion.AngleAxis(webcamTexture.videoRotationAngle, Vector3.up);

                this.GetComponent<GUITexture>().texture = resultTexture;
                //count++;

            }
        }
    }
}
