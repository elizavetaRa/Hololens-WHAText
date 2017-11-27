using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (!UNITY_EDITOR)
using System.Threading.Tasks;
using Windows.Globalization;
#endif

public interface IServiceAdaptor
{
    string ApiKey { get; }
    string Uri { get; }
    OcrResult OcrResult { get; }

#if (!UNITY_EDITOR)
    Language PreferredLang { get; }

    Task<OcrResult> HttpPostImage(string url = null, byte[] jsonBytes = null);
#endif

    void ParseResponseData(object response);
}
