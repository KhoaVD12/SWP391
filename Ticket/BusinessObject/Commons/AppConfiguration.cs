namespace BusinessObject.Commons;

public abstract class AppConfiguration
{
    public string DatabaseConnection { get; set; }
    public JWTSection JWTSection { get; set; }
}

public class JWTSection
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}