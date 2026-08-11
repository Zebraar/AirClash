using UnityEngine;
using Mirror;
using System;

public struct OpponentLeftMessage : NetworkMessage
{
    
}

public class MyNetworkManager : NetworkManager
{
    [Header("Dependencies")]
    public RoomManager roomManager;

    private string currentRoomCode = string.Empty;

    public static event Action OnOpponentDisconnected;
    public static event Action OnLocalClientDisconnected;

    public void SetCurrentRoomCode(string code)
    {
        currentRoomCode = code;
    }

    #region Client Callbacks

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<OpponentLeftMessage>(OnOpponentLeftMessageReceived);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        NetworkClient.UnregisterHandler<OpponentLeftMessage>();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnLocalClientDisconnected?.Invoke();
    }

    private void OnOpponentLeftMessageReceived(OpponentLeftMessage msg)
    {
        Debug.Log("[NetworkManager] Получено сообщение: оппонент вышел.");
        OnOpponentDisconnected?.Invoke();
    }

    #endregion

    #region Server Callbacks

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        foreach(var readyConn in NetworkServer.connections.Values)
        {
            if(readyConn != null && readyConn != conn)
            {
                readyConn.Send(new OpponentLeftMessage());
            }
        }

        base.OnServerDisconnect(conn);

        if(NetworkServer.connections.Count <= 1)
        {
            DeleteRoomFromBackend();
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        DeleteRoomFromBackend();
    }

    private void DeleteRoomFromBackend()
    {
        if(!string.IsNullOrEmpty(currentRoomCode) && roomManager != null)
        {
            string codeToDelete = currentRoomCode;

            roomManager.DeleteRoom(codeToDelete,
                () => 
                {
                    Debug.Log($"[NetworkManager] Комната {codeToDelete} удалена.");
                    currentRoomCode = string.Empty;
                },
                (err) => 
                {
                    Debug.LogError($"[NetworkManager] Ошибка удаления {codeToDelete}: {err}");
                    currentRoomCode = string.Empty;
                }
            );
        }
    }

    #endregion
}