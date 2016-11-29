using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;

namespace OrderSDK.HttpUtility
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestUtility
    {
        /// <summary>
        /// ʹ��Get������ȡ�ַ��������û�м���Cookie��
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        public static string HttpGet(string url, Encoding encoding = null)
        {
            //WebClient wc = new WebClient();
            //wc.Encoding = encoding ?? Encoding.UTF8;
            //if (encoding != null)
            //{
            //    wc.Encoding = encoding;
            //}
            //return wc.DownloadString(url);
            return HttpGet(url, null, encoding ?? Encoding.UTF8);
        }

        /// <summary>
        /// ʹ��Get������ȡ�ַ������������Cookie��
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpGet(string url, CookieContainer cookieContainer = null, Encoding encoding = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = Config.TimeOut;
            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }

            using (Stream responseStream = response.GetResponseStream())
            {
                using (var myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.GetEncoding("utf-8")))
                {
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }

        /// <summary>
        /// ʹ��Post������ȡ�ַ�������������ύ
        /// </summary>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            MemoryStream ms = new MemoryStream();
            ms.Write(formDataBytes, 0, formDataBytes.Length);
            ms.Seek(0, SeekOrigin.Begin);//����ָ���ȡλ��
            //var response = new HttpHelper().GetHtml(new HttpItem()
            //{
            //    URL = "http://japi.zto.cn/zto/api_utf8/comInterface",
            //    Encoding = encoding,
            //    Method = "POST",
            //    PostDataType = PostDataType.Byte,
            //    PostdataByte = formDataBytes
            //});
            //return response.Html;
            return HttpPost(url, cookieContainer, ms, null, null, encoding);
        }

        /// <summary>
        /// ʹ��Post������ȡ�ַ������
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="postStream"></param>
        /// <param name="fileDictionary">��Ҫ�ϴ����ļ���Key����ӦҪ�ϴ���Name��Value�������ļ���</param>
        /// <param name="refererUrl"> </param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, Stream postStream = null, Dictionary<string, string> fileDictionary = null, string refererUrl = null, Encoding encoding = null)
        {
            //HttpClient client= new HttpClient();
            //client.PostAsync(url, new StreamContent(postStream), new FormUrlEncodedMediaTypeFormatter());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version10;
            // �������������ȥ�� http://www.baiwanzhan.com/site/t121032/
            request.Timeout = Config.TimeOut;
            request.ReadWriteTimeout = Config.TimeOut;
            // http://blog.csdn.net/zhuyu19911016520/article/details/47948101
            ServicePointManager.DefaultConnectionLimit = 50;

            #region ����Form���ļ��ϴ�
            var formUploadFile = fileDictionary != null && fileDictionary.Count > 0;//�Ƿ���Form�ϴ��ļ�
            if (formUploadFile)
            {
                //ͨ�����ϴ��ļ�
                postStream = new MemoryStream();

                string boundary = "----" + DateTime.Now.Ticks.ToString("x");
                //byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";

                //foreach (var file in fileDictionary)
                //{
                //    try
                //    {
                //        var fileName = file.Value;
                //        //׼���ļ���
                //        using (var fileStream = FileHelper.GetFileStream(fileName))
                //        {
                //            var formdata = string.Format(formdataTemplate, file.Key, fileName /*Path.GetFileName(fileName)*/);
                //            var formdataBytes = Encoding.ASCII.GetBytes(postStream.Length == 0 ? formdata.Substring(2, formdata.Length - 2) : formdata);//��һ�в���Ҫ����
                //            postStream.Write(formdataBytes, 0, formdataBytes.Length);

                //            //д���ļ�
                //            byte[] buffer = new byte[1024];
                //            int bytesRead = 0;
                //            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                //            {
                //                postStream.Write(buffer, 0, bytesRead);
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //}
                //��β
                var footer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                postStream.Write(footer, 0, footer.Length);

                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            #endregion

            request.ContentLength = postStream != null ? postStream.Length : 0;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = false;
            if (!string.IsNullOrEmpty(refererUrl))
            {
                request.Referer = refererUrl;
            }
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";

            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }

            #region �����������
            if (postStream != null)
            {
                postStream.Position = 0;
                //ֱ��д����
                Stream requestStream = request.GetRequestStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                postStream.Close();//�ر��ļ�����
            }
            #endregion

            // Responseһ��Ҫ�ͷ�
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (cookieContainer != null)
                {
                    response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
                }
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
        }

        /// <summary>
        /// ��װQueryString�ķ���
        /// ����֮����&���ӣ���λû�з��ţ��磺a=1&b=2&c=3
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static string GetQueryString(this Dictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

    }
}
