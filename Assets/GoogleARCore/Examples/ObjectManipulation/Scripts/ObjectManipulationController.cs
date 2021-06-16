//-----------------------------------------------------------------------
// <copyright file="ObjectManipulationController.cs" company="Google LLC">
//
// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.ObjectManipulation
{
   
        using System.Collections.Generic;
        using System.Collections;
        using GoogleARCore;
        using GoogleARCore.Examples.Common;
        using UnityEngine;
        using UnityEngine.EventSystems;



    /*#if UNITY_EDITOR
        // Set up touch input propagation while using Instant Preview in the editor.
        using Input = InstantPreviewInput;
    #endif*/

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class ObjectManipulationController : MonoBehaviour
    {
        string msg = "initializing...";
        public float thisLat;
        public float thisLong;
        public float startingLat = 0.0f;
        public float startingLong = 0.0f;
        float kblt = 21.422487f;
        float kblg = 39.826206f;

        
        public GameObject gameObject;
        GameObject gameobj;

        //====================
        public static float Bearing(float pt1lt, float pt1lg, float pt2lt, float pt2lg)
        {
            float x = (float)Mathf.Cos(DegreesToRadians(pt1lt)) *
            (float)Mathf.Sin(DegreesToRadians(pt2lt))
            - (float)Mathf.Sin(DegreesToRadians(pt1lt)) * (float)Mathf.Cos(DegreesToRadians(pt2lt)) * (float)Mathf.Cos(DegreesToRadians(pt2lg - pt1lg));
            float y = (float)Mathf.Sin(DegreesToRadians(pt2lg - pt1lg)) * (float)Mathf.Cos(DegreesToRadians(pt2lt));

            // Math.Atan2 can return negative value, 0 <= output value < 2*PI expected 
            return ((float)Mathf.Atan2(y, x) + (float)Mathf.PI * 2.0f) % ((float)Mathf.PI * 2.0f) * 180.0f / Mathf.PI;
        }

        public void spawnObjOnButton()
        {
            if (startingLat == 0.0f && thisLat != 0.0f)
            {
                startingLat = thisLat;
                startingLong = thisLong;
                return;
            }

            float rotated_from_origin = FirstPersonCamera.transform.rotation.eulerAngles.y;
            float compass_heading = Input.compass.trueHeading;
            //gameobj.transform.position = FirstPersonCamera.transform.position + new Vector3(0,0,4.0f);
            float angle_to_origin = Bearing(thisLat, thisLong, startingLat, startingLong);
            float z_cord;
            float x_cord;

            float d = get_distance();



            //gameobj.transform.eulerAngles = new Vector3(0.0f, FirstPersonCamera.transform.eulerAngles.y, 0.0f); // rotate with respect to camera like default            
            


            while (d >= 0)
            {
                gameobj = Instantiate(GameObjectVerticalPlanePrefab);
                //gameobj.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                z_cord = d * Mathf.Cos(((angle_to_origin + rotated_from_origin - compass_heading) % 360) * Mathf.PI / 180.0f);
                x_cord = d * Mathf.Sin(((angle_to_origin + rotated_from_origin - compass_heading) % 360) * Mathf.PI / 180.0f);
                gameobj.transform.position = FirstPersonCamera.transform.position + new Vector3(x_cord, 0, z_cord);
                gameobj.transform.Rotate((int)angle_to_origin,0.0f, 0.0f, Space.Self);
                gameobj.transform.eulerAngles = new Vector3(0.0f, FirstPersonCamera.transform.eulerAngles.y, 0.0f); // rotate with respect to camera like default            

                d -= 3.0f;


            }


        }

        public void Update_location()
        {

            thisLat = Input.location.lastData.latitude;
            thisLong = Input.location.lastData.longitude;



        }

        public float get_distance()
        {
            if (startingLat == 0 && startingLong == 0)
            {
                return 0.0f;
            }
            return Haversine(thisLat, thisLong, startingLat, startingLong);
        }

        public void update_compass()
        {

            int trueNorth = (int)Input.compass.trueHeading;
            if (FirstPersonCamera.transform.forward.y <= -0.95)
            {


                gameObject.transform.position = FirstPersonCamera.transform.position;
                gameObject.transform.eulerAngles = new Vector3(0.0f, FirstPersonCamera.transform.eulerAngles.y, 0.0f); // rotate with respect to camera like default
                gameObject.SetActive(true);

                //gameObject.transform.rotation = Quaternion.Euler(0.0f, -(float)trueNorth, 0.0f);
                gameObject.transform.Rotate(0.0f, -trueNorth, 0.0f, Space.Self);


            }



        }


        public static float DegreesToRadians(float angle)
        {
            return angle * Mathf.PI / 180.0f;
        }

        //====================
        /* public static double FindAngle(double x1, double y1, double x2, double y2)
         {
             double dx = x2 - x1;
             double dy = y2 - y1;
             return (double)Mathf.Atan2((float)dy, (float)dx) * (180 / Mathf.PI);
         }
 */
        //====================
        void OnGUI()
        {
            GUI.skin.label.fontSize = 70;
            GUI.Label(new Rect(100, 200, 1000, 1000), msg);
            GUI.backgroundColor = Color.black;

        }
        /// <summary>
        /// The Depth Setting Menu.
        /// </summary>
        // public DepthMenu DepthMenu;

        /// <summary>
        /// The Instant Placement Setting Menu.
        /// </summary>
        //public InstantPlacementMenu InstantPlacementMenu;

        /// <summary>
        /// A prefab to place when an instant placement raycast from a user touch hits an instant
        /// placement point.
        /// </summary>
        //public GameObject InstantPlacementPrefab;

        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;
        public GameObject GameObjectVerticalPlanePrefab;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a vertical plane.
        /// </summary>
        //public GameObject GameObjectVerticalPlanePrefab;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a horizontal plane.
        /// </summary>
        // public GameObject GameObjectHorizontalPlanePrefab;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject GameObjectPointPrefab;

        /// <summary>
        /// The rotation in degrees need to apply to prefab when it is placed.
        /// </summary>
        //private const float _prefabRotation = 180.0f;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error,
        /// otherwise false.
        /// </summary>
        private bool _isQuitting = false;

        /// <summary>
        /// The Unity Awake() method.
        /// </summary>
        public void Awake()
        {
            // Enable ARCore to target 60fps camera capture frame rate on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;
        }




        float Haversine(float lat1, float long1, float lat2, float long2)
        {
            float earthRad = 6371000;
            float lRad1 = lat1 * Mathf.Deg2Rad;
            float lRad2 = lat2 * Mathf.Deg2Rad;
            float dLat = (lat2 - lat1) * Mathf.Deg2Rad;
            float dLong = (long2 - long1) * Mathf.Deg2Rad;
            float a = Mathf.Sin(dLat / 2.0f) * Mathf.Sin(dLat / 2.0f)
                + Mathf.Cos(lRad1) * Mathf.Cos(lRad2) *
                 Mathf.Sin(dLong / 2.0f) * Mathf.Sin(dLong / 2.0f);
            float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
            return earthRad * c;
        }



        //=============================================





        private void Start()
        {
            if (!Input.location.isEnabledByUser)
            {
                msg = "Location service not enabled";
            }
            Input.location.Start(2, 0);
            Input.compass.enabled = true;
            
            gameObject = Instantiate(GameObjectPointPrefab);


        }

        public void Update()
        {

            float cp_head = Input.compass.trueHeading;
            float tmp = (254.4f + FirstPersonCamera.transform.rotation.eulerAngles.y - cp_head) % 360;
            msg = "        " + (int)cp_head;
            msg += "\n check :  " + tmp;

            msg += "\n longitude : " + thisLong;
            msg += "\n latitude : " + thisLat;
            msg += "\n Distance : " + get_distance();
            msg += "\n kaaba distance  : " + Haversine(thisLat, thisLong, kblt, kblg);

            msg += "\n kaaba : " + Bearing(thisLat, thisLong, kblt, kblg);
            msg += "\n forward : " + FirstPersonCamera.transform.forward;
            msg += "\n rotation : " + FirstPersonCamera.transform.rotation.eulerAngles;
            msg += "\n postion: " + FirstPersonCamera.transform.position;

            update_compass();
            Update_location();
            msg += "\ny : " + Mathf.Round(FirstPersonCamera.transform.forward.y * 100f) / 100f;





            /* if (Input.deviceOrientation == DeviceOrientation.Portrait)
             {


                 check = false;
             }
 */











            // To use Recording API:
            // 1. Create an instance of ARCoreRecordingConfig. The Mp4DatasetFilepath needs to
            // be accessible by the app, e.g. Application.persistentDataPath, or you can request
            // the permission of external storage.
            // 2. Call Session.StartRecording(ARCoreRecordingConfig) when a valid ARCore session
            // is available.
            // 3. Call Session.StopRecording() to end the recording. When
            // ARCoreRecordingConfig.AutoStopOnPause is true, it can also stop recording when
            // the ARCoreSession component is disabled.
            // To use Playback API:
            // 1. Pause the session by disabling ARCoreSession component.
            // 2. In the next frame or later, call Session.SetPlaybackDataset(datasetFilepath)
            // where the datasetFilepath is the same one used by Recording API.
            // 3. In the next frame or later, resume the session by enabling ARCoreSession component
            // and the app will play the recorded camera stream install of using the real time
            // camera stream.
            UpdateApplicationLifecycle();


        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (_isQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to
            // appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                ShowAndroidToastMessage("Camera permission is needed to run this application.");
                _isQuitting = true;
                Invoke("DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                _isQuitting = true;
                Invoke("DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject =
                        toastClass.CallStatic<AndroidJavaObject>(
                            "makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
    }
        
  