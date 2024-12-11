using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockManagement_WinUI3;

public class UserReturn
{
    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SessionReturn
{
    public string Id { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class LoginReturn
{
    public UserReturn User { get; set; }
    public SessionReturn Session { get; set; }
}

public class LoginMethodReturn
{
    public bool success { get; set; }
    public UserReturn? user { get; set; }
}

public static class AppViewModel
{
    public static bool LoggedIn { get; private set; }
    public static string? SessionToken;
    public static string Username;
    public static event Action<UserReturn> UserLoggedIn;


    public static async Task<LoginMethodReturn> Login(string username, string password)
    {
        var httpClient = new HttpClient();
        var content = new StringContent(JsonConvert.SerializeObject(new { Username = username, Password = password }),
            Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(new Uri("http://localhost:5177/api/auth/login"), content);

        if (!response.IsSuccessStatusCode)
        {
            return new LoginMethodReturn { success = false };
        }

        var responseData = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(responseData))
        {
            throw new Exception("Response content is empty");
        }

        var data = JsonConvert.DeserializeObject<LoginReturn>(responseData);
        Debug.WriteLine(data);

        if (data == null)
        {
            return new LoginMethodReturn { success = false };
        }

        LoggedIn = true;
        SessionToken = $"{data.Session.Id}.{data.Session.Token}";
        Username = data.User.Username;

        UserLoggedIn?.Invoke(data.User);


        return new LoginMethodReturn { success = true, user = data.User };
    }

    public static async Task<bool> Logout()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", SessionToken);
        var response = await httpClient.PostAsync(new Uri("http://localhost:5177/api/products"), null);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        LoggedIn = false;
        SessionToken = null;
        return true;
    }
}