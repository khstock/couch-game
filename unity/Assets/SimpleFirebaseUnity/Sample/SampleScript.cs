// Last update: 2018-05-20  (by Dikra)

using UnityEngine;

using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

public class SampleScript : MonoBehaviour
{

    static int debug_idx = 0;

    [SerializeField]
    TextMesh textMesh;

    [SerializeField]
    public CG_Player cg_player;
    public CG_GameManager cg_manager;

    const string glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //add the characters you want

    [SerializeField]
    string RoomID = "";
        
    // Use this for initialization
    void Start()
    {
        textMesh.text = "";
        generateRoomId();
        StartCoroutine(Tests());
    }

    void generateRoomId()
    {
        for (int i = 0; i < 4; i++)
        {
            RoomID += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
        }
    }

    void GetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Get from key: <" + sender.FullKey + ">");
        DebugLog("[OK] Raw Json: " + snapshot.RawJson);


        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;

        if (sender.FullKey == "/Players/player1")
        {
            JsonUtility.FromJsonOverwrite(snapshot.RawJson, cg_player); ;
            cg_player.refresh();
            DebugLog("[OK] JSON SAVED IN CG_PLAYER");
        }
        DebugLog("[OK] NACH DEM JSON IF");

        if (keys != null)
            foreach (string key in keys)
            {
                DebugLog(key + " = " + dict[key].ToString());
            }
    }

    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void SetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Set from key: <" + sender.FullKey + ">");
    }

    void SetFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Set from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void UpdateOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Update from key: <" + sender.FullKey + ">");
    }

    void UpdateFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Update from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void DelOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Del from key: <" + sender.FullKey + ">");
    }

    void DelFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Del from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void PushOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] Push from key: <" + sender.FullKey + ">");
    }

    void PushFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] Push from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void GetRulesOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DebugLog("[OK] GetRules");
        DebugLog("[OK] Raw Json: " + snapshot.RawJson);
    }

    void GetRulesFailHandler(Firebase sender, FirebaseError err)
    {
        DebugError("[ERR] GetRules,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void GetTimeStamp(Firebase sender, DataSnapshot snapshot)
    {
        long timeStamp = snapshot.Value<long>();
        DateTime dateTime = Firebase.TimeStampToDateTime(timeStamp);

        DebugLog("[OK] Get on timestamp key: <" + sender.FullKey + ">");
        DebugLog("Date: " + timeStamp + " --> " + dateTime.ToString());
    }

    void DebugLog(string str)
    {
        Debug.Log(str);
        if (textMesh != null)
        {
            textMesh.text += (++debug_idx + ". " + str) + "\n";
        }
    }

    void DebugWarning(string str)
    {
        Debug.LogWarning(str);
        if (textMesh != null)
        {
            textMesh.text += (++debug_idx + ". " + str) + "\n";
        }
    }

    void DebugError(string str)
    {
        Debug.LogError(str);
        if (textMesh != null)
        {
            textMesh.text += (++debug_idx + ". " + str) + "\n";
        }
    }

    IEnumerator Tests()
    {
        // README
        DebugLog("This plugin simply wraps Firebase's RealTime Database REST API.\nPlease read here for better understanding of the API: https://firebase.google.com/docs/reference/rest/database/\n");
              
        // Inits Firebase using Firebase Secret Key as Auth
        // The current provided implementation not yet including Auth Token Generation
        // If you're using this sample Firebase End, 
        // there's a possibility that your request conflicts with other simple-firebase-c# user's request
        Firebase firebase = Firebase.CreateNew("https://arpege-3b760.firebaseio.com", "QQEXzUTjKfWtLs4WWeDxrL3KbHbn1CFfINY6YrlV");

        // Init callbacks
        firebase.OnGetSuccess += GetOKHandler;
        firebase.OnGetFailed += GetFailHandler;
        firebase.OnSetSuccess += SetOKHandler;
        firebase.OnSetFailed += SetFailHandler;
        firebase.OnUpdateSuccess += UpdateOKHandler;
        firebase.OnUpdateFailed += UpdateFailHandler;
        firebase.OnPushSuccess += PushOKHandler;
        firebase.OnPushFailed += PushFailHandler;
        firebase.OnDeleteSuccess += DelOKHandler;
        firebase.OnDeleteFailed += DelFailHandler;

        // Get child node from firebase, if false then all the callbacks are not inherited.
        Firebase temporary = firebase.Child("temporary", true);
        Firebase lastUpdate = firebase.Child("lastUpdate");
        Firebase name = firebase.Child("testName");

        lastUpdate.OnGetSuccess += GetTimeStamp;

        //tmp.text = name.GetValue<String>();

        // Make observer on "last update" time stamp
        FirebaseObserver observer = new FirebaseObserver(lastUpdate, 1f);
        observer.OnChange += (Firebase sender, DataSnapshot snapshot) =>
        {
            DebugLog("[OBSERVER] Last updated changed to: " + snapshot.Value<long>());
        };
        observer.Start();
        DebugLog("[OBSERVER] FirebaseObserver on " + lastUpdate.FullKey + " started!");

        // Print details
        DebugLog("Firebase endpoint: " + firebase.Endpoint);
        DebugLog("Firebase key: " + firebase.Key);
        DebugLog("Firebase fullKey: " + firebase.FullKey);
        DebugLog("Firebase child key: " + temporary.Key);
        DebugLog("Firebase child fullKey: " + temporary.FullKey);

        // Unnecessarily skips a frame, really, unnecessary.
        yield return null;

        // Create a FirebaseQueue
        FirebaseQueue firebaseQueue = new FirebaseQueue(true, 3, 1f); // if _skipOnRequestError is set to false, queue will stuck on request Get.LimitToLast(-1).
                                                                    

        // Test #1: Test all firebase commands, using FirebaseQueueManager
        // The requests should be running in order 
        firebaseQueue.AddQueueSet(firebase, GetSamplePlayers(), FirebaseParam.Empty.PrintSilent());
        firebaseQueue.AddQueuePush(firebase.Child("broadcasts", true), "{ \"name\": \"simple-firebase-csharp\", \"message\": \"awesome!\"}", true);
        firebaseQueue.AddQueueSetTimeStamp(firebase, "lastUpdate");
        //firebaseQueue.AddQueueGet(firebase, "print=pretty");
        firebaseQueue.AddQueueUpdate(firebase.Child("layout", true), "{\"x\": 5.8, \"y\":-94}");
        //firebaseQueue.AddQueueGet(firebase.Child("layout", true));
        //firebaseQueue.AddQueueGet(lastUpdate);

        //Deliberately make an error for an example
        //DebugWarning("[WARNING] There is one invalid request below (Get with invalid OrderBy) which will gives error, only for the sake of example on error handling.");
        //firebaseQueue.AddQueueGet(firebase, FirebaseParam.Empty.LimitToLast(-1));

        // (~~ -.-)~~
        //DebugLog("==== Wait for seconds 15f ======");
        //yield return new WaitForSeconds(15f);
        //DebugLog("==== Wait over... ====");


        // Test #2: Calls without using FirebaseQueueManager
        // The requests could overtake each other (ran asynchronously)
        //firebase.Child("broadcasts", true).Push("{ \"name\": \"dikra\", \"message\": \"hope it runs well...\"}", false);
        //firebase.GetValue(FirebaseParam.Empty.OrderByKey().LimitToFirst(2));
        //DebugLog("Hier wurden Limiti First 2");
        //temporary.GetValue();
        //firebase.GetValue(FirebaseParam.Empty.OrderByKey().LimitToLast(2));
        //temporary.GetValue();

        // Please note that orderBy "rating" is possible because I already defined the index on the Rule.
        // If you use your own endpoint, you might get an error if you haven't set it on your Rule.
        //firebase.Child("scores", true).GetValue(FirebaseParam.Empty.OrderByChild("rating").LimitToFirst(2));
        //firebase.GetRules(GetRulesOKHandler, GetRulesFailHandler);

        // ~~(-.- ~~)
        //yield return null;
        //DebugLog("==== Wait for seconds 15f ======");
        //yield return new WaitForSeconds(15f);
        //DebugLog("==== Wait over... ====");

        // We need to clear the queue as the queue is left with one error command (the one we deliberately inserted last time).
        // When the queue stuck with an error command at its head, the next (or the newly added command) will never be processed.
        //firebaseQueue.ForceClearQueue();
        //yield return null;      

        // Test #3: Delete the frb_child and broadcasts
        firebaseQueue.AddQueueGet(firebase.Child(RoomID, true));
        //firebaseQueue.AddQueueGet(firebase.Child("Players", true).Child("player1", true));

        

        //firebaseQueue.AddQueueDelete(temporary);

        //// Please notice that the OnSuccess/OnFailed handler is not inherited since Child second parameter not set to true.
        //DebugLog("'broadcasts' node is deleted silently.");
        //firebaseQueue.AddQueueDelete(firebase.Child("broadcasts"));
        //firebaseQueue.AddQueueGet(firebase);

        // ~~(-.-)~~
        yield return null;
        DebugLog("==== Wait for seconds 15f ======");
        yield return new WaitForSeconds(15f);
        DebugLog("==== Wait over... ====");
        observer.Stop();
        DebugLog("[OBSERVER] FirebaseObserver on " + lastUpdate.FullKey + " stopped!");
    }

    Dictionary<string, object> GetSamplePlayers()
    {
        Dictionary<string, object> PlayerBoard = new Dictionary<string, object>();
        Dictionary<string, object> RoomCode = new Dictionary<string, object>();
        Dictionary<string, object> players = new Dictionary<string, object>();
        Dictionary<string, object> p1 = new Dictionary<string, object>();
        Dictionary<string, object> p2 = new Dictionary<string, object>();
        Dictionary<string, object> p3 = new Dictionary<string, object>();
        Dictionary<string, object> p4 = new Dictionary<string, object>();

        p1.Add("name", "scripto");
        p1.Add("score", 80);
        p1.Add("host", true);

        p2.Add("name", "nudella");
        p2.Add("score", 100);
        p2.Add("host", false);

        p3.Add("name", "angemeldet");
        p3.Add("score", 60);
        p3.Add("host", false);

        p4.Add("name", "h4");
        p4.Add("score", 70);
        p4.Add("host", false);

        players.Add("player1", p1);
        players.Add("player2", p2);
        players.Add("player3", p3);
        players.Add("player4", p4);

        RoomCode.Add("Players", players);
        RoomCode.Add("gameMode", 0);
        RoomCode.Add("gameState", "lobby");

        PlayerBoard.Add(RoomID, RoomCode);
 


        return PlayerBoard;
    }
}