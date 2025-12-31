using UnityEngine;

public class InputValidators : MonoBehaviour
{
    public static bool IsValidPort(string portString)       // walidacja numeru portu
    {
        try
        {
            if (string.IsNullOrEmpty(portString))       // nie moze byc pusty
            {
                return false;
            }

            if (portString.StartsWith("0") && portString.Length > 1)        // nie moze zaczynac sie od 0
            {
                return false;
            }

            if (!ushort.TryParse(portString, out ushort portNumber))        // sprawdzenie maksymalnego rozmiaru + czy same liczby (ushort -> od 0 do 65 535)
            {
                return false;
            }

            if (portNumber < 1024)      // musi byc wiekszy od 1023
            {
                return false;
            }
            return true;
        }
        catch
        {
            ErrorCatcher.instance.ErrorHandler();
            return false;
        }
        
    }

    public static bool IsValidIP(string ipString)       // walidacja numeru ip
    {
        try
        {
            if (string.IsNullOrEmpty(ipString))     // nie moze byc pusty
            {
                return false;
            }

            string[] octets = ipString.Split('.');      // podzial na oktety wzgledem kropek

            if (octets.Length != 4)     // musza byc 4 oktety
            {
                return false;
            }

            foreach (string octet in octets)        // dla kazdego oktetu
            {
                if (octet.StartsWith("0") && octet.Length > 1)      // nie moze zaczynac sie od 0 chyba ze to samo 0
                {
                    return false;
                }

                if (!byte.TryParse(octet, out byte _))      // proba zamiany na bajt -> rozmiar od 0 do 255
                {
                    return false;
                }
            }

            return true;
        }
        catch
        {
            ErrorCatcher.instance.ErrorHandler();
            return false;
        }
        
    }
}
