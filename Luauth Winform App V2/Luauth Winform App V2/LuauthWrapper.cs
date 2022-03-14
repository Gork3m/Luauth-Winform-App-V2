using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.IO;
using System.Windows.Forms;

namespace Luauth_Winform_App_V2 {

    public class LuauthWrapper {
        string apikey { get; set; }
        public HttpTypes.APIKeyDetails RawData { get; set; }

        public bool Initialized { get; set; }
        public LuauthWrapper(string api_key) {
            string statusData = Http.GetAPIStatus();
            HttpTypes.APIStatus t = JsonConvert.DeserializeObject<HttpTypes.APIStatus>(statusData);
            if (t.active == false) {
                MessageBox.Show(t.message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            apikey = api_key;
            Initialized = false;
        }
        public void Initialize() {
            string respData = Http.GetAPIKeyDetails(apikey);
            HttpTypes.APIResponse t = JsonConvert.DeserializeObject<HttpTypes.APIResponse>(respData);
            if (t.success == false) {
                MessageBox.Show(t.message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            } else {
                HttpTypes.APIKeyDetails keyDetails = JsonConvert.DeserializeObject<HttpTypes.APIKeyDetails>(respData);
                RawData = keyDetails;
                Initialized = true;
            }
        }
    }

    public class Endpoints {
        const string APIVersion = "v2";
        const string BaseURL = "https://api.luauth.xyz";
        public static string GetAPIKeyDetails(string key) => $"{BaseURL}/{APIVersion}/keys/{key}/details";
        public static string GetAPIStatus() => $"{BaseURL}/status";

        public static string WhitelistIdentifier(string script_id) => $"{BaseURL}/{APIVersion}/whitelist/{script_id}";
        public static string UnwhitelistIdentifier(string script_id, string identifier) => $"{BaseURL}/{APIVersion}/whitelist/{script_id}/{identifier}";
        public static string GetIdentifierDetails(string script_id, string identifier) => $"{BaseURL}/{APIVersion}/whitelist/{script_id}/{identifier}";
        public static string GetAllIdentifiers(string script_id) => $"{BaseURL}/{APIVersion}/whitelist/{script_id}";
        public static string UnbanIdentifier(string script_id, string unban_token) => $"{BaseURL}/{APIVersion}/unban/{script_id}?token={unban_token}";

        public static string NewScript() => $"{BaseURL}/{APIVersion}/scripts";
        public static string UpdateScript(string script_id) => $"{BaseURL}/{APIVersion}/scripts/{script_id}";
        public static string DeleteScript(string script_id) => $"{BaseURL}/{APIVersion}/scripts/{script_id}";

        public static string GetLoader(string script_id) => $"{BaseURL}/files/{APIVersion}/loaders/{script_id}.lua";
        public static string GetAPILogs(string api_key) => $"{BaseURL}/{APIVersion}/logs/{api_key}";
        public static string GetScriptLogs(string api_key, string script_id) => $"{BaseURL}/{APIVersion}/logs/{api_key}/{script_id}";
    }
    class Objects {
        public class LuauthClient {
            public HttpTypes.IdentifierDetails RawDetails { get; set; }
        }

        public class LuauthScript {
            public HttpTypes.ScriptDetails RawDetails { get; set; }
            public HttpTypes.APIResponse WhitelistIdentifier() { return null; }


        }
    }
    public class HttpTypes {
        public class APIResponse {
            public bool success { get; set; }
            public string message { get; set; }
        }
        public class APIStatus {
            public string message { get; set; }
            public string gateway_version { get; set; }
            public bool active { get; set; }
        }
        public class APIKeyDetails {
            public string owner { get; set; }
            public int expires_at { get; set; }
            public string key { get; set; }
            public int max_scripts { get; set; }
            public ScriptDetails[] scripts { get; set; }
        }
        public class ScriptDetails {
            public string script_name { get; set; }
            public string platform { get; set; }
            public int total_executions { get; set; }
            public string script_id { get; set; }
            public string script_version { get; set; }
            public bool disabled { get; set; }
            public bool ffa { get; set; }
        }
        public class AddIdentifier {
            public string identifier { get; set; }
            public Int32? auth_expire { get; set; }
            public string note { get; set; }
        }
        public class IdentifierDetails {
            public string identifier { get; set; }
            public string identifier_type { get; set; }
            public bool whitelisted { get; set; }
            public bool banned { get; set; }
            public string ban_reason { get; set; }
            public int total_executions { get; set; }
            public Int32 auth_expire { get; set; }
            public string unban_token { get; set; }
            public string note { get; set; }
        }
        public class AllIdentifiers {
            public string script_name { get; set; }
            public string script_id { get; set; }
            public IdentifierDetails[] whitelisted_users { get; set; }
        }
        public class CreateScript {
            public string script_name { get; set; }
            public string platform { get; set; }
            public string script { get; set; }
            public string logs_webhook { get; set; }
            public string alerts_webhook { get; set; }
            public bool? ffa { get; set; }
        }
        public class ScriptCreated {
            public string script_id { get; set; }
            public string script_name { get; set; }
            public string loader_script { get; set; }
            public bool ffa { get; set; }
        }
        public class UpdateScript {
            public string script { get; set; }
            public string logs_webhook { get; set; }
            public string alerts_webhook { get; set; }
            public bool? ffa { get; set; }
        }
        public class ScriptUpdated {
            public string version { get; set; }
        }

    }

    public class Http {

        public static string GetAPIKeyDetails(string api_key) {
            return RawGet(Endpoints.GetAPIKeyDetails(api_key));
        }

        public static string GetAPIStatus() {
            return RawGet(Endpoints.GetAPIStatus());
        }

        public static string RawPost(string url, string method, string body, string auth) {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;
            httpWebRequest.ContentLength = body.Length;
            if (auth != "") {
                httpWebRequest.Headers.Add("Authorization", auth);
            }
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
                streamWriter.Write(body);
            }
            try {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    var result = streamReader.ReadToEnd();
                    return result.ToString();
                }
            } catch (WebException ex) {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    string err = reader.ReadToEnd();
                    //MessageBox.Show(err);
                    return err;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }

        public static string RawGet(string url, string auth = "") {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            if (auth != "") {
                httpWebRequest.Headers.Add("Authorization", auth);
            }

            try {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    var result = streamReader.ReadToEnd();
                    return result.ToString();
                }
            } catch (WebException ex) {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    string err = reader.ReadToEnd();
                    //MessageBox.Show(err);
                    return err;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }
    }
}
