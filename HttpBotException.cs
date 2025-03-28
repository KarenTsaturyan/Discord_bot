namespace DiscordBot_Ecommerce
{
    public class HttpBotException : Exception
    {
        public string Response { get; }
        public HttpBotException(HttpResponseMessage response) => Response = response.Content.ReadAsStringAsync().Result;
    }
}
