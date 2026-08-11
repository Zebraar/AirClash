using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class CreateRoomRequest
{
    public string eos_id;
}

[Serializable]
public class CreateRoomResponse
{
    public string status;
    public string room_code;
    public string eos_id;
    public string message;
}

[Serializable]
public class GetRoomRequest
{
    public string room_code;
}

[Serializable]
public class GetRoomResponse
{
    public string status;
    public string eos_id;
    public string message;
}

[Serializable]
public class DeleteRoomRequest
{
    public string room_code;
}

[Serializable]
public class DeleteRoomResponse
{
    public string status;
    public string message;
}

public class RoomManager : MonoBehaviour
{
    private const string BASE_URL = "https://airclashserver.onrender.com";

    public void CreateRoom(string eosId, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(CreateRoomRoutine(eosId, onSuccess, onError));
    }

    public void JoinRoom(string roomCode, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(JoinRoomRoutine(roomCode, onSuccess, onError));
    }

    public void DeleteRoom(string roomCode, Action onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(DeleteRoomRoutine(roomCode, onSuccess, onError));
    }

    private IEnumerator CreateRoomRoutine(string eosId, Action<string> onSuccess, Action<string> onError)
    {
        string url = BASE_URL + "/createRoom";
        CreateRoomRequest requestData = new CreateRoomRequest { eos_id = eosId };
        string json = JsonUtility.ToJson(requestData);

        using(UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if(www.result == UnityWebRequest.Result.Success)
            {
                CreateRoomResponse res = JsonUtility.FromJson<CreateRoomResponse>(www.downloadHandler.text);
                if(res.status == "success")
                {
                    onSuccess?.Invoke(res.room_code);
                }
                else
                {
                    onError?.Invoke(res.message);
                }
            }
            else
            {
                onError?.Invoke(www.error);
            }
        }
    }

    private IEnumerator JoinRoomRoutine(string roomCode, Action<string> onSuccess, Action<string> onError)
    {
        string url = BASE_URL + "/getRoomEosId";
        GetRoomRequest requestData = new GetRoomRequest { room_code = roomCode };
        string json = JsonUtility.ToJson(requestData);

        using(UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if(www.result == UnityWebRequest.Result.Success)
            {
                GetRoomResponse res = JsonUtility.FromJson<GetRoomResponse>(www.downloadHandler.text);
                if(res.status == "success")
                {
                    onSuccess?.Invoke(res.eos_id);
                }
                else
                {
                    onError?.Invoke(res.message);
                }
            }
            else
            {
                onError?.Invoke(www.error);
            }
        }
    }

    private IEnumerator DeleteRoomRoutine(string roomCode, Action onSuccess, Action<string> onError)
    {
        string url = BASE_URL + "/deleteRoom";
        DeleteRoomRequest requestData = new DeleteRoomRequest { room_code = roomCode };
        string json = JsonUtility.ToJson(requestData);

        using(UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if(www.result == UnityWebRequest.Result.Success)
            {
                DeleteRoomResponse res = JsonUtility.FromJson<DeleteRoomResponse>(www.downloadHandler.text);
                if(res.status == "success")
                {
                    Debug.Log($"[RoomManager] Комната {roomCode} успешно удалена с сервера.");
                    onSuccess?.Invoke();
                }
                else
                {
                    onError?.Invoke(res.message);
                }
            }
            else
            {
                onError?.Invoke(www.error);
            }
        }
    }
}