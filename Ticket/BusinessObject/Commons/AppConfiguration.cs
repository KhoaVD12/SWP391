namespace BusinessObject.Commons;

public class AppConfiguration
{
    public string DatabaseConnection { get; set; }
    public JWTSection JWTSection { get; set; }
}

public class JWTSection
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}