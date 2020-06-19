using SeasideResearch.LibCurlNet;
using System;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace SVK.BGPB
{
    // https://csharp.hotexamples.com/ru/examples/SeasideResearch.LibCurlNet/Easy/SetOpt/php-easy-setopt-method-examples.html
    public class RegisterDo 
    {
        StringBuilder connectLog = null;
        StringBuilder headerLog = null;
        StringBuilder answer = null;

        string connectLogText = null;
        string headerLogText = null;
        string answerText = null;

        public string ConnectLog { get { return connectLogText; } }
        public string HeaderLog { get { return headerLogText; } }
        public string Answer { get { return answerText; } }

        // https://csharp.hotexamples.com/ru/examples/SeasideResearch.LibCurlNet/Easy/SetOpt/php-easy-setopt-method-examples.html

        public RegisterDo(string orderNumber, string description, float amount, string userName, string password, string returnUrl, string failUrl)
        {
            if (amount == 0 || String.IsNullOrEmpty(orderNumber) || String.IsNullOrEmpty(returnUrl) || String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password)) { }
            else
            {
                connectLog = new StringBuilder();
                headerLog = new StringBuilder();
                answer = new StringBuilder();

                string URL = String.Format("https://mpi-test.bgpb.by:9443/payment/rest/register.do?amount={0}&currency=933&language=en&orderNumber={1}&returnUrl={2}&userName={3}&password={4}&failUrl={5}&description={6}", amount.ToString("N2").Replace(",", ""), orderNumber, returnUrl, userName, password, failUrl, description);

                Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

                using (Easy easy = new Easy())
                {
                    easy.SetOpt(CURLoption.CURLOPT_URL, URL);
                    easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, 0);
                    easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYHOST, 1);
                    easy.SetOpt(CURLoption.CURLOPT_SSLCERT, HostingEnvironment.ApplicationPhysicalPath + "\\mpi-test.bgpb.by.crt");
                    easy.SetOpt(CURLoption.CURLOPT_SSLKEY, HostingEnvironment.ApplicationPhysicalPath + "\\mpi.test.key");
                    easy.SetOpt(CURLoption.CURLOPT_SSLKEYPASSWD, "Bgpb2019");
                    easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, 1);
                    easy.SetOpt(CURLoption.CURLOPT_TIMEOUT, "60");

                    Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
                    easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);
                    easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);

                    easy.SetOpt(CURLoption.CURLOPT_USERAGENT, "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko");
                    easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, 1);
                    easy.SetOpt(CURLoption.CURLOPT_POST, true);
                    easy.Perform();
                }

                connectLogText = connectLog.ToString(); connectLog = null;
                headerLogText = headerLog.ToString(); headerLog = null;
                answerText = answer.ToString(); answer = null;
                Curl.GlobalCleanup();
            }
        }

        public void OnDebug(CURLINFOTYPE infoType, String msg, Object extraData)
        {
            switch (infoType)
            {
                case CURLINFOTYPE.CURLINFO_HEADER_IN: if (msg.StartsWith("HTTP/1.1 ")) headerLog.Append("\r\n");  headerLog.Append(msg);  break;
                case CURLINFOTYPE.CURLINFO_HEADER_OUT: if (msg.EndsWith("ontinue\r\n\r\n")) headerLog.Append(msg.Substring(0, msg.Length - 2)); else headerLog.Append(msg); break;
                case CURLINFOTYPE.CURLINFO_TEXT: connectLog.Append(msg); break;
                case CURLINFOTYPE.CURLINFO_DATA_IN: answer.Append(msg); break;
            }
        }
    }
}
