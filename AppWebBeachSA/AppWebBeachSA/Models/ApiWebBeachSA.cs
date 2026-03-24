namespace AppWebBeachSA.Models
{
    public class ApiWebBeachSA
    {
        public HttpClient IniciarAPI()
        {
            
            HttpClient client = new HttpClient();

            
            client.BaseAddress = new Uri("https://localhost:7074/");

            

 
            return client;
        }
    }
}
