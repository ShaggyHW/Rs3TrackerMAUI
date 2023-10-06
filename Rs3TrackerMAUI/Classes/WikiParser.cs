using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rs3TrackerMAUI.Classes {
    public class WikiParser {
#if WINDOWS
        string mainDir = Microsoft.Maui.Storage.FileSystem.CacheDirectory;
#endif
#if MACCATALYST
        string mainDir = Microsoft.Maui.Storage.FileSystem.CacheDirectory;
#endif
        public string getHTMLCode(string endpoint) {
            string url = "https://runescape.wiki/w/";
            string pageHTML = "";
            using (WebClient web = new WebClient()) {
                web.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
                try {
                    pageHTML = web.DownloadString(url + endpoint);
                } catch (Exception ex) { }
            }
            return pageHTML;
        }

        public string SaveImageFROMURL(string name, string endpoint) {
            string finalName = name.Replace(" ", "_");
            if (name.Contains("Destroy")) {
                finalName = name.Replace(" ", "_") + "_(ability)";
            }
            if (File.Exists(Path.Combine(mainDir, "Images", name.Replace(" ", "_") + ".png"))) {
                return name.Replace(" ", "_");
            }
            string url = "https://runescape.wiki" + endpoint;
            using (WebClient client = new WebClient()) {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.Headers.Add("user-agent", "PostmanRuntime/7.26.1");
                try {
                    client.Headers.Add("user-agent", "PostmanRuntime/7.26.1");
                    string fileResult = Path.Combine(mainDir, "Images", name.Replace(" ", "_") + ".png");
                    client.DownloadFile(new Uri(url), fileResult);
                } catch (Exception ex) {
                    try {
                        finalName = name.Replace(" ", "_") + "_(Ability)";
                        url = "https://runescape.wiki/images/" + finalName + ".png";
                        client.Headers.Add("user-agent", "PostmanRuntime/7.26.1");
                        string fileResult = Path.Combine(mainDir, "Images", name.Replace(" ", "_") + ".png");
                        client.DownloadFile(new Uri(url), fileResult);
                    } catch (Exception ex2) {
                        try {

                            finalName = name.Replace(" ", "_") + "_(ability)";
                            url = "https://runescape.wiki/images/" + finalName + ".png";
                            client.Headers.Add("user-agent", "PostmanRuntime/7.26.1");
                            string fileResult = Path.Combine(mainDir, "Images", name.Replace(" ", "_") + ".png");
                            client.DownloadFile(new Uri(url), fileResult);
                        } catch (Exception ex3) {
                       
                         //DisplayAlert("Couldn't Download Image", "ERROR LOADING IMAGE:" + endpoint + "\r\nONCE IT FINISHES CLICK IMPORT AGAIN UNTIL YOU DONT GET ERRORS", "OK");
                           
                        }
                    }
                }

            }
            return name.Replace(" ", "_");
        }
    }
}
