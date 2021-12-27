using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkError : Exception
{
    private string errorMessage;

    public NetworkError(string message)
    {
        errorMessage = message;
    }
    public NetworkError(UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.ConnectionError)
        {
            errorMessage = "Connection Error";
        }
        else if (result == UnityWebRequest.Result.DataProcessingError)
        {
            errorMessage = "Data Processing Error";
        }
        else if (result == UnityWebRequest.Result.ProtocolError)
        {
            errorMessage = "Protocol Error";
        }
    }

    public override string ToString()
    {
        return errorMessage;
    }

}
