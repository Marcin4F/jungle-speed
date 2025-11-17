using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Laczenie : MonoBehaviour
{
    private string nick, code, serverIp, firstMessage;
    private int serverPort;

    private TcpClient client;
    private NetworkStream stream;
    private bool isConnected = false;

    [SerializeField] MainMenuUI mainMenuUI;

    public async void ConnectToServer(char type)
    {
        if (isConnected) return;

        if (type != 's' && type != 'j')
        {
            Debug.LogError($"[ConnectToServer] Invalid connection type: '{type}'. Must be 's' or 'j'.");
            return;
        }

        nick = mainMenuUI.nick;
        serverIp = mainMenuUI.ipAddress;
        serverPort = int.Parse(mainMenuUI.port);

        try
        {
            client = new TcpClient();
            Debug.Log($"Connecting to {serverIp}:{serverPort}...");

            await client.ConnectAsync(serverIp, serverPort);

            if (client.Connected)
            {
                stream = client.GetStream();
                isConnected = true;
                Debug.Log("Successfully connected to server!");

                mainMenuUI.inputUIPanel.SetActive(false);

                StartListening();

                if (type == 's')
                {
                    firstMessage = "START#" + nick + "%";
                }
                else if (type == 'j')
                {
                    code = mainMenuUI.code;
                    firstMessage = "JOIN#" + nick + "#" + code + "%";
                }
                SendMessageToServer(firstMessage);
            }
        }

        catch (SocketException e)
        {
            Debug.LogError($"ConnectToSerwer: SocketException: {e.Message}");
            mainMenuUI.connectingServerPanel.SetActive(false);
            mainMenuUI.connectionErrorPanel.SetActive(true);
            mainMenuUI.ipAddressInput.text = "";
            mainMenuUI.portInput.text = "";
            mainMenuUI.port = null;
            mainMenuUI.ipAddress = null;
        }
        catch (Exception e)
        {
            mainMenuUI.connectingServerPanel.SetActive(false);
            Debug.LogError($"Bl¹d ConnectToSerwer: {e.Message}");
        }
    }

    private async void StartListening()
    {
        byte[] buffer = new byte[255];

        try
        {
            while (isConnected && stream != null)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Debug.Log("Server disconnected.");
                    CloseConnection();
                    break;
                }

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log($"Server says: {receivedMessage}");
            }
        }

        catch (ObjectDisposedException)
        {
            Debug.Log("Connection closed (stream disposed).");
        }
        catch (Exception e)
        {
            if (isConnected)
            {
                Debug.LogError($"Error while listening: {e.Message}");
                CloseConnection();
            }
        }
    }

    public async void SendMessageToServer(string message)
    {
        if (!isConnected || stream == null || !stream.CanWrite)
        {
            Debug.LogWarning("Not connected to server or stream is not writable.");
            return;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            await stream.WriteAsync(data, 0, data.Length);
            Debug.Log($"Sent to server: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending message: {e.Message}");
            CloseConnection();
        }
    }

    private void CloseConnection()
    {
        if (!isConnected) return;

        isConnected = false;

        if (stream != null)
        {
            stream.Close();
            stream = null;
        }

        if (client != null)
        {
            client.Close();
            client = null;
        }

        Debug.Log("Disconnected from server.");
    }

    void OnDestroy()
    {
        CloseConnection();
    }
}
