using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Net.Http;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using System;
using System.Net;


namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        #endregion


        #region Private Fields


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
      /*  [SerializeField]
        private GameObject _inputField;
        [SerializeField]
        private GameObject _inputField2;*/
        string email, password;
        //private static readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;
        //JsonObject playerInfo;
        //string info;
        #endregion
        #region public Fields
        public InputField inputField1, inputField2;
        public static string data="";
        #endregion

        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            //launcher = new Launcher();
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            
        }

        /* IEnumerator request(WWW www)
         {

           yield return www;
         }*/
        IEnumerator Upload(string email, string password)
        {
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);

           /* form.AddField("email", "alumn@poker.com");
            form.AddField("password", "12345678");*/

            UnityWebRequest www = UnityWebRequest.Post("https://pokernetwork.herokuapp.com/users/login", form);
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
                Debug.Log("Login complete!");
            }
        }
        IEnumerator Post(string email, string password)
        {
            string bodyJsonString="{\"email\" : \""+email+"\", \"password\" : \""+password+"\"}";
            Debug.Log(bodyJsonString);
            var request = new UnityWebRequest("https://pokernetwork.herokuapp.com/users/login", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);
            data= request.downloadHandler.text;
            //Debug.Log(all);
            // playerInfo = request;
            /*string encodedString = "{\"field1\": 0.5,\"field2\": \"sampletext\",\"field3\": [1,2,3]}";
            JSONObject j = new JSONObject(encodedString);*/


        }
        IEnumerator GetText()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://pokernetwork.herokuapp.com/");
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }
        }

        #endregion


        #region Public Methods


        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
           // StartCoroutine(Post("alumn@poker.com", "12345678"));
            StartCoroutine(Post(inputField1.text.ToString(), inputField2.text.ToString()));
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            
            //Debug.Log(playerInfo.downloadHandler.text);
            /* WWWForm formDate = new WWWForm();
             formDate.AddField("email", "alumn@poker.com");
             formDate.AddField("password", "12345678");
             WWW www = new WWW("https://pokernetwork.herokuapp.com/users/login",formDate);
             StartCoroutine(request(www));
             Debug.Log(info);*/
            /* var values = new Dictionary<string, string>
 {
     { "email", "alumn@poker.com" },
     { "password", "12345678" }
 };

             var content = new FormUrlEncodedContent(values);

             var response = await client.PostAsync("https://pokernetwork.herokuapp.com/users/login", content);

             var responseString = await response.Content.ReadAsStringAsync();*/

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                // #Critical, we must first and foremost connect to Photon Online Server.
                //PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
            
        }
        
        #endregion
        #region MonoBehaviourPunCallbacks Callbacks


        public override void OnConnectedToMaster()
        {
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            //PhotonNetwork.JoinRandomRoom();
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");


                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Room for 1");
            }
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }

        #endregion

    }
}