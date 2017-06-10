using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, string fioy, string emaily, string phony, string dayreg, string commentito, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    var text = String.Format("ФИО: {0}\nПОЧТА: {1}\nТЕЛЕФОН: {2}\nДень Недели: {3}\nКомментарий: {4}", fioy, emaily, phony, dayreg, commentito);
    log.Info(text);

    var results = await SendTelegramMessage(text);
    log.Info(String.Format("{0}", results));

    return req.CreateResponse(HttpStatusCode.OK);
}

public static async Task<string> SendTelegramMessage(string text)
{
    using (var client = new HttpClient())
    {

        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("chat_id", "-210784585");
        dictionary.Add("text", text);

        string json = JsonConvert.SerializeObject(dictionary);
        var requestData = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(String.Format("https://api.telegram.org/bot{0}/sendMessage", Environment.GetEnvironmentVariable("tgKey")), requestData);
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }
}