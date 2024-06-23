using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Unity.VisualScripting;
using UnityEditor;
using Firebase.Extensions;
using System;

public class test_authentication : MonoBehaviour
{
    public InputField email;
    public InputField password;
    public Button signin;
    public Button signup;
    
    void Start(){
        StartCoroutine(flow());
    }

    IEnumerator flow(){
        var auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += (object sender, System.EventArgs eventArgs) => {
            // if (auth.CurrentUser != user) {
            //     bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
            //         && auth.CurrentUser.IsValid();
            //     if (!signedIn && user != null)
            //     {
            //         DebugLog("Signed out " + user.UserId);
            //     }
            //     user = auth.CurrentUser;
            //     if (signedIn)
            //     {
            //         DebugLog("Signed in " + user.UserId);
            //         displayName = user.DisplayName ?? "";
            //         emailAddress = user.Email ?? "";
            //         photoUrl = user.PhotoUrl ?? "";
            //     }
            // }

            if(auth.CurrentUser != null){
                Debug.Log("email verified " + auth.CurrentUser.IsEmailVerified);
            }
        };
        
        var selection = new selection(signin, signup);
        yield return selection;
        if(selection.selected == signin){
            var task = auth.SignInWithEmailAndPasswordAsync(email.text, password.text);
            yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled);
            Debug.Log(1);
            if(task.IsCompleted){
                if(task.Exception != null){
                    Debug.Log(task.Exception);
                }
                else{
                    Debug.Log(task.Result.User.IsEmailVerified);
                }

            }

            
            // auth.SignInWithCredentialAsync(Firebase.Auth.EmailAuthProvider.GetCredential(email.text, password.text)).ContinueWithOnMainThread(task_signin => {
            
            auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task_signin => {
                Debug.Log("return");

                Debug.Log("why");

                try{
                    Debug.LogError(task_signin.Exception);
                    Debug.Log(task_signin.IsCompleted);
                    Debug.Log(task_signin.IsFaulted);
                    Debug.Log(task_signin.IsCanceled);
                    if(task_signin.IsCompleted){
                        Debug.Log("completed");
                        Debug.Log(task_signin.Result);
                        if(!task_signin.Result.User.IsEmailVerified){
                            Debug.Log("email not verified");
                            // var task_verify = task_signin.Result.User.SendEmailVerificationAsync();
                            // yield return new WaitUntil(() => task_verify.IsCompleted || task_verify.IsFaulted);
                            // Debug.Log("email sent");
                        }
                        else{
                            Debug.Log("email verified");
                        }
                    }
                    else if(task_signin.IsFaulted){
                        Debug.Log("why");
                        Debug.LogError(task_signin.Exception);
                        // Debug.Log("email not verified");
                    }
                    else if(task_signin.IsCanceled){
                        Debug.Log("why");
                        Debug.LogError("canceled");
                        // Debug.Log("email not verified");
                    }
                    Debug.Log("end of try");
                }
                catch(Exception e){
                    Debug.LogError(e);

                }
                

                Debug.Log("why");

                Debug.LogError(task_signin.Exception);

            });
            // yield return new WaitUntil(() => task_signin.IsCompleted || task_signin.IsFaulted || task_signin.IsCanceled);
            // Debug.Log(JsonUtility.ToJson(task_signin.Result.User));
            
        }
        else if(selection.selected == signup){

        }

        // auth.CreateUserWithEmailAndPasswordAsync("wonhotoss@gmail.com", "FirebasePW0").ContinueWith(task =>
        // {
        //     if (task.IsCanceled)
        //     {
        //         Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        //         return;
        //     }
        //     if (task.IsFaulted)
        //     {
        //         Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
        //         return;
        //     }

        //     // Firebase user has been created.
        //     var result = task.Result;
        //     Debug.LogFormat("Firebase user created successfully: {0} ({1})",
        //         result.User.DisplayName, result.User.UserId);
                

        //         result.User.IsEmailVerified
        //         result.User.SendEmailVerificationAsync
        // });

        

        // auth.SignOut

        

        // auth.CurrentUser

        // auth.StateChanged

        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
