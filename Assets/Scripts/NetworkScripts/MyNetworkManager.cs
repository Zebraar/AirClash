using UnityEngine;
using Mirror;
using System;

public class MyNetworkManager : NetworkManager
{
    [Header("Dependencies")]
    public RoomManager roomManager;

    private string currentRoomCode = string.Empty;

    public static event Action OnClientDisconnected;

    public void SetCurrentRoomCode(string code)
    {
        currentRoomCode = code;
    }

    #region Client Callbacks

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientDisconnected?.Invoke();
    }

    #endregion

    #region Server Callbacks

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if(NetworkServer.connections.Count == 0)
        {
            Debug.Log("[NetworkManager] Все клиенты отключились. Удаляем комнату из базы...");
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
            currentRoomCode = string.Empty;

            roomManager.DeleteRoom(codeToDelete,
                () => Debug.Log($"[NetworkManager] Комната {codeToDelete} удалена."),
                (err) => Debug.LogError($"[NetworkManager] Ошибка удаления {codeToDelete}: {err}")
            );
        }
    }

    #endregion
}