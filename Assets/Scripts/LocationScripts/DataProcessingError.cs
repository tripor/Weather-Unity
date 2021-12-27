using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataProcessingError : Exception
{
    private string errorMessage;

    public DataProcessingError(string message)
    {
        errorMessage = message;
    }

    public string GetErrorMessage()
    {
        return errorMessage;
    }

    public override string ToString()
    {
        return errorMessage;
    }
}
