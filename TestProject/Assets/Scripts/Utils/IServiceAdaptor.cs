﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (!UNITY_EDITOR)
using Windows.Globalization;
#endif

public interface IServiceAdaptor
{
    string ApiKey { get; set; }
    string Uri { get; set; }

#if (!UNITY_EDITOR)
    Language PreferredLang { get; set; }
#endif

    void HttpPostImage(string url = null, byte[] jsonBytes = null);
    void ParseResponseData();
}