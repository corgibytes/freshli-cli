namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class IdentityEntity
{
    // "provider": "github",
    public string Provider { get; set; }
    // "user_id": 21340,
    public int UserId { get; set; }
    // "connection": "github",
    public string Connection { get; set; }
    // "isSocial": true
    public bool IsSocial { get; set; }
}
