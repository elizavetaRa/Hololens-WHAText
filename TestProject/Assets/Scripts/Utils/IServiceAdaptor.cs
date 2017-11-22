using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (!UNITY_EDITOR)
using Windows.Globalization;
#endif

public interface IServiceAdaptor
{
    string ApiKey { get; }
    string Uri { get; }
    OcrResult OcrResult { get; }

#if (!UNITY_EDITOR)
    Language PreferredLang { get; }
#endif

    void HttpPostImage(string url = null, byte[] jsonBytes = null);
    void ParseResponseData(object response);
}
