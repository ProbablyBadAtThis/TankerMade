using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using TankerMade.Contracts.DTOs.Auth;

namespace TankerMade.Client.Services;

public class AuthSession
{
    private const string StorageKey = "tankermade.auth";
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public AuthSession(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public event Action? Changed;

    public AuthState State { get; private set; } = AuthState.SignedOut;

    public bool IsSignedIn => !string.IsNullOrWhiteSpace(State.Token) && State.ExpiresAt > DateTime.UtcNow;

    public async Task InitializeAsync()
    {
        var saved = await _localStorage.GetItemAsync<AuthState>(StorageKey);
        if (saved == null || saved.ExpiresAt <= DateTime.UtcNow)
        {
            await SignOutAsync();
            return;
        }

        State = saved;
        ApplyBearerToken();
        Changed?.Invoke();
    }

    public async Task<ApiResult> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", request);
        return await CompleteAuthRequestAsync(response);
    }

    public async Task<ApiResult> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/register", request);
        return await CompleteAuthRequestAsync(response);
    }

    public async Task SignOutAsync()
    {
        State = AuthState.SignedOut;
        _http.DefaultRequestHeaders.Authorization = null;
        await _localStorage.RemoveItemAsync(StorageKey);
        Changed?.Invoke();
    }

    private async Task<ApiResult> CompleteAuthRequestAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return ApiResult.Failed(await ReadErrorAsync(response));
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (auth == null || string.IsNullOrWhiteSpace(auth.Token))
        {
            return ApiResult.Failed("The server returned an empty sign-in response.");
        }

        State = new AuthState(auth.Token, auth.Username, auth.Email, auth.Role, auth.ExpiresAt);
        await _localStorage.SetItemAsync(StorageKey, State);
        ApplyBearerToken();
        Changed?.Invoke();

        return ApiResult.Success();
    }

    private void ApplyBearerToken()
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", State.Token);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(body))
        {
            return body;
        }

        return $"Request failed with status {(int)response.StatusCode}.";
    }
}

public sealed record AuthState(
    string Token,
    string Username,
    string Email,
    string Role,
    DateTime ExpiresAt)
{
    public static AuthState SignedOut { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue);
}

public sealed record ApiResult(bool Succeeded, string Error)
{
    public static ApiResult Success() => new(true, string.Empty);
    public static ApiResult Failed(string error) => new(false, error);
}
