namespace WeatherMVC.Services
{
  public class IdentityServerSettings
  {
    public string DiscoveryUrl { get; set; } // the url to access the server
    public string ClientName { get; set; }  // the name of the client that i need to get this information
    public string ClientPassword { get; set; }  // the password of the client 
    public bool UseHttps { get; set; }
  }
}