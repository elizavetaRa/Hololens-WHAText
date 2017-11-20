using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (PLATFORM_HOLOLENS)
using Windows.Globalization;
#endif

public interface IServiceAdaptor
{
    string ApiKey { get; set; }
    string Uri { get; set; }

#if (PLATFORM_HOLOLENS)
    Language preferredLang { get; set; }
#endif

    void HttpPostImage(string url = null, byte[] jsonBytes = null);
    void ParseResponseData();
}
