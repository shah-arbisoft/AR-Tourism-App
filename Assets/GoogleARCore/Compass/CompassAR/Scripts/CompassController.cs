
namespace GoogleARCore.Compass.HelloAR
{
    
    using GoogleARCore;
    using UnityEngine;

    /*#if UNITY_EDITOR
        // Set up touch input propagation while using Instant Preview in the editor.
        using Input = InstantPreviewInput;
    #endif*/

    /// <summary>
    /// Controls the Compass.
    /// </summary>
    public class CompassController : MonoBehaviour
    {
        string msg = "initializing...";
        void OnGUI()
        {
            GUI.skin.label.fontSize = 100;
            GUI.Label(new Rect(100, 200, 1000, 1000), msg);
            GUI.backgroundColor=Color.black;
           
        }
       
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

       
        /// <summary>
        /// Gameobject refering to compass_prefab .
        /// </summary>
        public GameObject GameObjectPointPrefab;

       
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




        //=============================================
        GameObject prefab;
        GameObject gameObject;

        bool check = false;
        private void Start()
        {
            Input.location.Start(2, 0);
            Input.compass.enabled = true;
            prefab = GameObjectPointPrefab;
            gameObject = Instantiate(prefab);
            gameObject.SetActive(false);
        }


        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        
        public void Update()
        {
            msg = "        " + (int)Input.compass.trueHeading;
            gameObject.SetActive(true);

      
            int trueNorth = (int)Input.compass.trueHeading;
            

            if (Input.deviceOrientation == DeviceOrientation.FaceUp && check == false)
            {


                gameObject.transform.position = FirstPersonCamera.transform.position;
                gameObject.transform.eulerAngles = new Vector3(0.0f,FirstPersonCamera.transform.eulerAngles.y,0.0f);
                


                check = true;


                //gameObject.transform.rotation = Quaternion.Euler(0.0f, -(float)trueNorth, 0.0f);
                gameObject.transform.Rotate(0.0f, -(float)trueNorth, 0.0f, Space.Self);


            }
         



            if (Input.deviceOrientation == DeviceOrientation.Portrait)
            {
               
                    
                check = false;
            }




            
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
