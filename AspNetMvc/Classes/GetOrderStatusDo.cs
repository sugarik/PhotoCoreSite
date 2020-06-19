using SeasideResearch.LibCurlNet;
using System;
using System.Text;

namespace SVK.BGPB
{
    // https://csharp.hotexamples.com/ru/examples/SeasideResearch.LibCurlNet/Easy/SetOpt/php-easy-setopt-method-examples.html
    public class GetOrderStatusDo
    {
        StringBuilder connectLog = null;
        StringBuilder headerLog = null;
        StringBuilder answer = null;

        string connectLogText = null;
        string headerLogText = null;
        string answerText = null;

        public string ConnectLog { get { return connectLogText; } }
        public string HeaderLog { get { return headerLogText; } }
        //public string Answer { get { return answerText; } }
        public string Answer { get { return GetAnswer(answerText); } }

        // https://csharp.hotexamples.com/ru/examples/SeasideResearch.LibCurlNet/Easy/SetOpt/php-easy-setopt-method-examples.html


        string GetAnswer(string answerText)
        {
            string result = answerText;
            string[] parts = answerText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for(int i=0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("{") && parts[i].EndsWith("}"))
                {
                    result = parts[i];
                    break;
                }
            }
            return result;
        }

        public GetOrderStatusDo(string userName, string password, string mdOrder)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(mdOrder)) { }
            else
            {
                connectLog = new StringBuilder();
                headerLog = new StringBuilder();
                answer = new StringBuilder();

                //string URL = String.Format("https://mpi-test.bgpb.by:9443/payment/rest/getOrderStatus.do?userName={0}&password={1}&mdOrder={2}", userName, password, mdOrder);
                string URL = String.Format("https://mpi-test.bgpb.by:9443/payment/rest/getOrderStatus.do?userName={0}&password={1}&orderId={2}&language=en", userName, password, mdOrder);

                Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL);

                using (Easy easy = new Easy())
                {
                    easy.SetOpt(CURLoption.CURLOPT_URL, URL);
                    easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, 0);
                    easy.SetOpt(CURLoption.CURLOPT_SSL_VERIFYHOST, 1);
                    easy.SetOpt(CURLoption.CURLOPT_SSLCERT, "mpi-test.bgpb.by.crt");
                    easy.SetOpt(CURLoption.CURLOPT_SSLKEY, "mpi.test.key");
                    easy.SetOpt(CURLoption.CURLOPT_SSLKEYPASSWD, "Bgpb2019");
                    easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, 1);
                    easy.SetOpt(CURLoption.CURLOPT_TIMEOUT, "60");

                    Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
                    easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);
                    easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);

                    easy.SetOpt(CURLoption.CURLOPT_USERAGENT, "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko");
                    easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, 1);
                    easy.SetOpt(CURLoption.CURLOPT_HTTPGET, true);
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
                case CURLINFOTYPE.CURLINFO_HEADER_IN: if (msg.StartsWith("HTTP/1.1 ")) headerLog.Append("\r\n"); headerLog.Append(msg); break;
                case CURLINFOTYPE.CURLINFO_HEADER_OUT: if (msg.EndsWith("ontinue\r\n\r\n")) headerLog.Append(msg.Substring(0, msg.Length - 2)); else headerLog.Append(msg); break;
                case CURLINFOTYPE.CURLINFO_TEXT: connectLog.Append(msg); break;
                case CURLINFOTYPE.CURLINFO_DATA_IN:
                    {
                        answer.Append(GetAnswer(msg));
                    }

                    break;
            }
        }
    }
}
