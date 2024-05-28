namespace OdpTracking.Client
{
    public class AccountEvent : OdpClientEvent
    {
        public override string Type { get; } = OdpClientSideEventTypes.Account.ToString();

        public static AccountEvent Login() => new() { Action = GetCurrentMethod() };
        public static AccountEvent Logout() => new() { Action = GetCurrentMethod() };
        public static AccountEvent Register() => new() { Action = GetCurrentMethod() };
        public static AccountEvent Update() => new() { Action = GetCurrentMethod() };
    }
}