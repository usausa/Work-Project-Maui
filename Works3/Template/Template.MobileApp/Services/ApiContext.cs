namespace Template.MobileApp.Services;

using Template.MobileApp.Helpers;

// UIスレッドからの書き込みを通信スレッド(ApiDelegatingHandler)が読むため、参照の可視性をvolatileで保証する
// 401 の再ログインは NetworkOperator が LoginId で行う (サーバーは Id のみで JWT を発行する契約)
public sealed class ApiContext
{
    private volatile Uri? baseAddress;

    private volatile string token = string.Empty;

    private volatile string loginId = string.Empty;

    public Uri? BaseAddress
    {
        get => baseAddress;
        set => baseAddress = value;
    }

    public string Token => token;

    // 最後にログインした Id (空なら未ログイン)
    public string LoginId
    {
        get => loginId;
        set => loginId = value;
    }

    // トークンの有効期限 (JWT の exp。ローカル時刻)
    public DateTime? TokenExpires { get; private set; }

    public bool IsAuthenticated => token.Length > 0;

    public void SetToken(string value)
    {
        TokenExpires = JwtHelper.GetExpiration(value);
        token = value;
    }

    public void ClearToken()
    {
        token = string.Empty;
        TokenExpires = null;
        loginId = string.Empty;
    }
}
