using System;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;

public class Laczenie : MonoBehaviour
{
    private string nick, code, serverIp, firstMessage;
    private int serverPort;

    private TcpClient client;
    private NetworkStream stream;
    public bool isConnected = false;
    private bool isConnecting = false;

    public static Laczenie instance;

    [SerializeField] MainMenuUI mainMenuUI;
    [SerializeField] InGameUI inGameUI;
    [SerializeField] TextMeshProUGUI displayText;

    public event Action<string> OnMessageReceived;

    private void Awake()
    {
        instance = this;
    }

    public async void ConnectToServer(char type)    // podlaczenie sie do serwera
    {
        try
        {
            if (isConnected || isConnecting)        // sprawdzenie czy juz nie jestesmy polaczeni albo czy juz sie nie probujemy polaczyc
                return;
            isConnecting = true;

            if (type != 's' && type != 'j')         // typ polaczenia (czy tworzymy pokoj s - start, czy dolaczamy j - join)
            {
                isConnecting = false;
                Debug.LogError($"[ConnectToServer] Invalid connection type: '{type}'. Must be 's' or 'j'.");
                return;
            }

            // wartosci do wyslania do serwera:
            nick = mainMenuUI.nick;
            serverIp = mainMenuUI.ipAddress;
            serverPort = int.Parse(mainMenuUI.port);
        }
        catch
        { ErrorCatcher.instance.ErrorHandler(); return; }


        try
        {
            // uzyskanie deskryptora
            client = new TcpClient();
            Debug.Log($"Connecting to {serverIp}:{serverPort}...");

            // laczenie
            await client.ConnectAsync(serverIp, serverPort);

            if (client.Connected)
            {
                stream = client.GetStream();
                isConnected = true;
                Debug.Log("Successfully connected to server!");

                StartListening();       // asynchroniczna funkcja od sluchania

                if (type == 's')        // wyslanie odpowiedniego komunikatu
                {
                    firstMessage = "CREATE_ROOM " + nick + "%";
                }
                else if (type == 'j')
                {
                    code = mainMenuUI.code;
                    firstMessage = "JOIN_ROOM " + code + " " + nick + "%";
                }
                SendMessageToServer(firstMessage);
            }
        }

        catch (SocketException e)
        {
            Debug.LogError($"ConnectToSerwer: SocketException: {e.Message}");
            mainMenuUI.connectingServerPanel.SetActive(false);
            displayText.SetText("Couldn't connect to the server.\r\nCheck IP Address and Port number");
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
        finally
        {
            isConnecting = false;
        }
    }

    private async void StartListening()
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (isConnected && stream != null)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);       // odczytanie danych wyslanych przez serwer

                if (bytesRead == 0)     // serwer sie odlaczyl
                {
                    Debug.Log("Server disconnected.");
                    CloseConnection();
                    inGameUI.mainPanel.SetActive(false);
                    GameMeneger.instance.ResetParameters();
                    mainMenuUI.CleanFields();
                    mainMenuUI.mainPanel.SetActive(true);
                    mainMenuUI.connectionLostText.SetText("Lost connection with server");
                    mainMenuUI.connectionLostMessage.SetActive(true);
                    break;
                }

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);     // zamiana na string
                OnMessageReceived?.Invoke(receivedMessage);         // przetwazrania komunikatow
            }
        }

        catch (ObjectDisposedException)
        {
            Debug.Log("Connection closed (stream disposed).");
            inGameUI.mainPanel.SetActive(false);
            GameMeneger.instance.ResetParameters();
            mainMenuUI.CleanFields();
            mainMenuUI.mainPanel.SetActive(true);
            mainMenuUI.connectionLostText.SetText("Lost connection with server");
            mainMenuUI.connectionLostMessage.SetActive(true);
        }
        catch (Exception e)
        {
            if (isConnected)
            {
                Debug.LogError($"Error while listening: {e.Message}");
                CloseConnection();
                inGameUI.mainPanel.SetActive(false);
                GameMeneger.instance.ResetParameters();
                mainMenuUI.CleanFields();
                mainMenuUI.mainPanel.SetActive(true);
                mainMenuUI.connectionLostText.SetText("Error occured with server connection.\nLost connection with server");
                mainMenuUI.connectionLostMessage.SetActive(true);
            }
        }
    }

    public async void SendMessageToServer(string message)       // wysylanie do serwera
    {
        if (!isConnected || stream == null || !stream.CanWrite)
        {
            Debug.LogWarning("Not connected to server or stream is not writable.");
            return;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);      // zamiana na bajty

            await stream.WriteAsync(data, 0, data.Length);      // wyslanie
            Debug.Log($"Sent to server: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending message: {e.Message}");
            CloseConnection();
        }
    }

    public void CloseConnection()       // zamkniecie polaczenia
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
