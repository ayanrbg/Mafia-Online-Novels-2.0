    using System.Collections;
    using System.Collections.Generic;
    using Firebase.Extensions;
    using Google;
    using System.Threading.Tasks;
    using UnityEngine;
    using TMPro;
    using Firebase.Auth;
    using UnityEngine.UI;
    using UnityEngine.Networking;

    public class LoginWithGoogle : MonoBehaviour
    {
        [Header("Google API")]
        private string GoogleAPI = "850279515963-ctle51nm1jqjlnuedq2nrl8siru68ptd.apps.googleusercontent.com"; // Replace with your actual WebClientID
        private GoogleSignInConfiguration configuration;

        [Header("Firebase Auth")]
        private FirebaseAuth auth;
        private FirebaseUser user;

        [Header("UI References")]
        public TextMeshProUGUI Username, UserEmail, debugText;
        public GameObject LoginPanel, UserPanel;
        public Image UserProfilePic;

        private string imageUrl;
        private bool isGoogleSignInInitialized = false;

        private void Start()
        {
            InitFirebase();
            InitGoogle();
        }
        void InitGoogle()
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };
        }
        void InitFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
        }

        public void Login()
        {
            Debug.Log("Login button pressed");
            
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                Debug.Log("Google SignIn success");

                if (task.IsCanceled)
                {
                    Debug.LogWarning("Google sign-in was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Google sign-in encountered an error: " + task.Exception);
                    return;
                }

                GoogleSignInUser googleUser = task.Result;

                Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
                Debug.Log("Signing into Firebase...");

                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(async authTask =>
                {
                    if (authTask.IsFaulted || authTask.IsCanceled)
                    {
                        Debug.LogError("Firebase auth failed: " + authTask.Exception);
                        return;
                    }

                    var firebaseUser = authTask.Result;

                    if (firebaseUser == null)
                    {
                        Debug.LogError("Firebase user is null");
                        return;
                    }

                    try
                    {
                        string firebaseIdToken = await firebaseUser.TokenAsync(true);

                        Debug.Log("TOKEN RECEIVED. Length: " + firebaseIdToken.Length);

                        WebSocketManager.Instance.SendFirebaseId(firebaseIdToken);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Token error: " + e);
                    }
                });


            });
        }
        IEnumerator GetTokenAndSend()
        {
            var tokenTask = user.TokenAsync(true);

            yield return new WaitUntil(() => tokenTask.IsCompleted);

            if (tokenTask.IsFaulted)
            {
                Debug.LogError("Token error: " + tokenTask.Exception);
                yield break;
            }

            if (tokenTask.IsCanceled)
            {
                Debug.LogError("Token request canceled");
                yield break;
            }

            string firebaseIdToken = tokenTask.Result;

            Debug.Log("Firebase ID token length: " + firebaseIdToken.Length);

            if (WebSocketManager.Instance == null)
            {
                Debug.LogError("WebSocketManager is NULL!");
                yield break;
            }

            WebSocketManager.Instance.SendFirebaseId(firebaseIdToken);
        }

        private string CheckImageUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return imageUrl;
        }

        IEnumerator LoadImage(string imageUri)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                Debug.Log("Image loaded successfully.");
            }
            else
            {
                Debug.LogError("Error loading profile image: " + www.error);
            }
        }
        // User SignOut From Firebase First Then again Sign IN With Google
        public async void SignOut()
        {
            Debug.Log("GOOGLE SIGN OUT START");

            // 1️⃣ Google
            GoogleSignIn.DefaultInstance.SignOut();

            // 2️⃣ Firebase
            if (auth != null)
            {
                auth.SignOut();
            }

            // 3️⃣ WebSocket logout (если есть)
            if (WebSocketManager.Instance != null)
            {
                WebSocketManager.Instance.Logout();
            }

            Debug.Log("GOOGLE SIGN OUT COMPLETE");
        }

    }
