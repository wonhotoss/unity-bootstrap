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
            if(auth.CurrentUser != null){
                Debug.Log("email verified " + auth.CurrentUser.IsEmailVerified);
            }
        };   

        var logged_in = false;

        while(!logged_in){
            var selection = new selection(signin, signup);
            yield return selection;
            if(selection.selected == signin){
                var signing_in = auth.SignInWithEmailAndPasswordAsync(email.text, password.text);
                yield return new WaitUntil(() => signing_in.IsCompleted || signing_in.IsCanceled);
                if(signing_in.IsCompleted){
                    if(signing_in.Exception != null){
                        Debug.LogError(signing_in.Exception);
                    }
                    else{
                        var user = signing_in.Result.User;
                        if(user.IsEmailVerified){
                            logged_in = true;
                        }
                        else{
                            var sending_email = user.SendEmailVerificationAsync();
                            Debug.Log("SEND VERIFICATION MAIL");
                            yield return new WaitUntil(() => sending_email.IsCompleted || sending_email.IsCanceled);
                            while(!user.IsEmailVerified){
                                yield return new WaitForSeconds(2.0f);
                                var reloading = user.ReloadAsync();
                                Debug.Log("RELOADING");
                                yield return new WaitUntil(() => reloading.IsCompleted || reloading.IsCanceled);
                            }
                            logged_in = true;
                        }
                    }
                }
            }
        }

        Debug.Log("LOGGED IN");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
