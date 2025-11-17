using System;
using UnityEngine;

public class InputValidators : MonoBehaviour
{
    public static bool IsValidPort(string portString)
    {
        if (string.IsNullOrEmpty(portString))
        {
            return false;
        }

        if (portString.StartsWith("0") && portString.Length > 1)
        {
            return false;
        }

        if (!ushort.TryParse(portString, out ushort portNumber))
        {
            return false;
        }

        if (portNumber < 1024)
        {
            return false;
        }
        return true;
    }

    public static bool IsValidIP(string ipString)
    {
        if (string.IsNullOrEmpty(ipString))
        {
            return false;
        }

        string[] octets = ipString.Split('.');

        if (octets.Length != 4)
        {
            return false;
        }

        foreach (string octet in octets)
        {
            if (octet.StartsWith("0") && octet.Length > 1)
            {
                return false;
            }

            if (!byte.TryParse(octet, out byte _))
            {
                return false;
            }
        }

        return true;
    }
}
