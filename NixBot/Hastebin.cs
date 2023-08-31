using Flurl;
using Flurl.Http;

namespace NixBot;

public class Paste
{
    public string Key = "";
    public string URL = "";
}

public class Hastebin
{
    private string Domain;

    public Hastebin(string domain)
    {
        Domain = domain;
    }

    public async Task<Paste> CreatePaste(string contents)
    {
        var paste = new Paste();
        var req = await Domain.AppendPathSegment("documents")
            .SendStringAsync(HttpMethod.Post, contents);
        var json = await req.GetJsonAsync<Dictionary<string, string>>();
        paste.Key = json["key"];
        paste.URL = Domain.AppendPathSegment(json["key"]);
        return paste;
    }
}