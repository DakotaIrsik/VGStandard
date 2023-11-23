using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;

namespace VGStandard.Core.Utility;

public static class UtilityMethods
{
    public static bool IsValidJson(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput)) { return false; }
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
            (strInput.StartsWith("\"{") && strInput.EndsWith("}\"")) || //For object with quotes
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
        {
            var obj = JToken.Parse(strInput);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static IDictionary<string, string> ToKeyValue(this object metaToken)
    {
        if (metaToken == null)
        {
            return null;
        }

        // Added by me: avoid cyclic references
        var serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        var token = metaToken as JToken;
        if (token == null)
        {
            // Modified by me: use serializer defined above
            return ToKeyValue(JObject.FromObject(metaToken, serializer));
        }

        if (token.HasValues)
        {
            var contentData = new Dictionary<string, string>();
            foreach (var child in token.Children().ToList())
            {
                var childContent = child.ToKeyValue();
                if (childContent != null)
                {
                    contentData = contentData.Concat(childContent)
                                             .ToDictionary(k => k.Key, v => v.Value);
                }
            }

            return contentData;
        }

        var jValue = token as JValue;
        if (jValue?.Value == null)
        {
            return null;
        }

        var value = jValue?.Type == JTokenType.Date ?
                        jValue?.ToString("o", CultureInfo.InvariantCulture) :
                        jValue?.ToString(CultureInfo.InvariantCulture);

        return new Dictionary<string, string> { { token.Path, value } };
    }

    public static FormUrlEncodedContent ToFormData(this object obj)
    {
        var formData = obj.ToKeyValue();

        return new FormUrlEncodedContent(formData);
    }
    public static string GetMachineIPAddress()
    {
        IPHostEntry host;
        string localIP = "?";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily.ToString() == "InterNetwork")
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
    }

    public static string GetClassAndMethodName()
    {
        string fullName;
        Type declaringType;
        int skipFrames = 1;
        //do
        //{
        MethodBase method = new StackFrame(skipFrames, false).GetMethod();
        declaringType = method.DeclaringType;
        if (declaringType == null)
        {
            return method.Name;
        }
        //skipFrames++;
        fullName = declaringType.FullName;
        //}
        //while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        return fullName;
    }
}
