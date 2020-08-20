using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace News
{
    public enum OperationRequest
    {
        Post,
        Get,
        Put,
        Delete
    }

    public class ServerMeneger
    {
        public const string DATA_RECEIVED = "dataReceived";
        private readonly string _requestUrl = "http://frontappapi.dock7.66bit.ru/api/";

        private HttpRequestMessage request;
        private HttpResponseMessage response;

        public async Task<ReceivedDataEvent> RequestResponse(
            string url = null, string body = null,
            OperationRequest method = OperationRequest.Post, string Token = null)
        {
            Debug.WriteLine("url : " + _requestUrl + url);
            Debug.WriteLine("body : " + body);
            try
            {
                var statusWiFi = StatusWiFi();
                Debug.WriteLine("statusWiFi : " + statusWiFi.Status + " " + statusWiFi.Message);
                if (statusWiFi.Status)
                {
                    using (var httpClient = new HttpClient())
                    {
                        switch (method)
                        {
                            case OperationRequest.Post:
                                if (string.IsNullOrEmpty(body))
                                    response = await HttpClientOptions(httpClient, Token).PostAsync(_requestUrl + url, null);
                                else
                                {
                                    request = GetPostRequest(body, url);
                                    response = Token != null ?
                                        await HttpClientOptions(httpClient, Token).SendAsync(request) : await HttpClientOptions(httpClient).SendAsync(request);
                                }
                                break;
                            case OperationRequest.Get:
                                response = await HttpClientOptions(httpClient, Token).GetAsync(_requestUrl + url);
                                break;
                            case OperationRequest.Put:
                                request = GetPutRequest(body, url);
                                response = Token != null ?
                                    await HttpClientOptions(httpClient, Token).SendAsync(request) : await HttpClientOptions(httpClient).SendAsync(request);
                                break;
                            case OperationRequest.Delete:
                                response = await HttpClientOptions(httpClient, Token).DeleteAsync(_requestUrl + url);
                                break;
                        }

                        var content = response.Content;
                        var json = await content.ReadAsStringAsync();

                        Debug.WriteLine("response.StatusCode : " + response.StatusCode);

                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                return new ReceivedDataEvent(true, json);
                            case HttpStatusCode.InternalServerError:
                                Debug.WriteLine("InternalServerError : " + json);
                                throw new Exception("Сервер не работает");
                            case HttpStatusCode.Forbidden:
                                throw new Exception("Доступ запрещен");
                            default:
                                throw new Exception(json);
                        }
                    }
                }
                else
                {
                    throw new Exception(statusWiFi.Message);
                }
            }
            catch (Exception ex)
            {
                return new ReceivedDataEvent(false, ex.Message);
            }
        }

        /// <summary>
        /// Метод проверки состояния Wi-Fi
        /// </summary>
        /// <returns></returns>
        private ModelStatusInternet StatusWiFi()
        {
            var statusInternet = new ModelStatusInternet();

            bool keyIsStatus = false;
            string message = "";

            var current = Connectivity.NetworkAccess;

            switch (current)
            {
                case NetworkAccess.Internet:
                    var profiles = Connectivity.ConnectionProfiles;
                    if (profiles.Contains(ConnectionProfile.WiFi))
                    {
                        Debug.WriteLine("WiFi true");
                    }
                    else
                    {
                        Debug.WriteLine("WiFi false");
                    }

                    keyIsStatus = true;
                    // Connected to internet
                    break;
                case NetworkAccess.Local:
                    Debug.WriteLine("Local true");
                    keyIsStatus = true;
                    // Only local network access
                    break;
                case NetworkAccess.ConstrainedInternet:
                    message = "Ограниченный доступ к интернету";
                    // Connected, but limited internet access such as behind a network login page
                    break;
                case NetworkAccess.None:
                    // No internet available
                    message = "Нет подключение к интернету";
                    break;
                case NetworkAccess.Unknown:
                    message = "Не удается определить режим подключения";
                    // Internet access is unknown
                    break;
            }

            statusInternet.Status = keyIsStatus;
            statusInternet.Message = message;
            return statusInternet;
        }

        /// <summary>
        /// Gets the request. Структура HttpClient
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="accessToken">Access token.</param>
        private HttpClient HttpClientOptions(HttpClient client, string accessToken = null)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Debug.WriteLine("Time: " + DateTime.Now);
            Debug.WriteLine("accessToken: " + accessToken);

            //#if DEBUG
            //accessToken = "$2y$10$0.nP3gV8KBj654w2cQnMDO4YwwnUD8Q.9/meXXlSTJKdChsbaF9fu";
            //#endif

            if (accessToken != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return client;
        }

        /// <summary>
        /// Gets the post request. - Формирование сообщение запроса
        /// </summary>
        /// <returns>The post request.</returns>
        /// <param name="body">Body.</param>
        /// <param name="path">Path.</param>
        private HttpRequestMessage GetPostRequest(string body, string path) => new HttpRequestMessage
        {
            RequestUri = new Uri(_requestUrl + path),
            Method = HttpMethod.Post,

            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

        private HttpRequestMessage GetPutRequest(string body, string path) => new HttpRequestMessage
        {
            RequestUri = new Uri(_requestUrl + path),
            Method = HttpMethod.Put,
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
    }
}
