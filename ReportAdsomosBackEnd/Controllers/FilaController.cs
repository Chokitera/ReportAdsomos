using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ReportAdsomosBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilaController : ControllerBase
    {
        private readonly string urlCentral;
        private readonly string urlRelatorio;
        private readonly string urlGeral;
        private string token;
        private string fila;
        /* 
          REQUISITO: COLETAR OS DADOS DA CENTRAL E TRANSFORMAR EM JSON, SOMENTE AS INFORMAÇÕES ABAIXO:

        - AGENTE
        - FILA
        - STATUS
        - TEMPO EM LIGAÇÃO
     
      DEPOIS COM ESSAS INFORMAÇÕES MONTAR UMA ROTA QUE DEVOLVE ESSES DADOS PARA O FRONTEND.
      NECESSÁRIO CRIAR TAMBÉM UMA OUTRA ROTA PARA RETORNAR OS VALORES EM FORMA DE RELATÓRIO, INICIALMENTE SEM PAGINAÇÃO.
      SALVAR OS DADOS DO RELATÓRIO NO BANCO, NO CASO O RELATÓRIO VAI SE BASEAR NOS DADOS DO BANCO.
     
     */

        public FilaController()
        {
            urlGeral = "/public/";
            urlCentral = "/public/index";
            urlRelatorio = "/library/fila/realtime_ajax.php";
            token = "";
        }

        [HttpGet("fila")]
        public string GetFila()
        {
            try
            {
                //Inicialização
                var client = this.ServiceHTTPClient;

                //Gerar o Token
                var consult2 = client.GetAsync("http://192.168.0.6/integrador/public/index").Result;

                //Coletando o Token dos Cookies recebidos
                if (consult2.Headers.TryGetValues("Set-Cookie", out var cookieValues))
                {
                    foreach (var cookieValue in cookieValues)
                    {
                        client.DefaultRequestHeaders.Add("Cookie", cookieValue);
                        token = cookieValue.ToString().Replace("PHPSESSID=", "").Replace(";", "").Replace("path=/", "").Replace(" ", "");
                    }
                }

                //Define o cookie
                var cookie = new Cookie("PHPSESSID", token);

                //Novo HttpClient para Cookie (Handler)
                var handler = new HttpClientHandler();
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(client.BaseAddress, cookie);

                //Cria um novo HttpClient com o HttpClientHandler personalizado (Cookie)
                var httpClientWithCookies = new HttpClient(handler);
                httpClientWithCookies.BaseAddress = new Uri("http://192.168.0.6/integrador/public/index");

                //Definição dos dados para Login
                using MultipartFormDataContent content = new()
                {
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "login" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "senha" }
                };

                //Login
                var consult = httpClientWithCookies.PostAsync("", content).Result;

                //Definição dos filtros para o relatório
                using MultipartFormDataContent filtroRelatorio = new()
                {
                    { new StringContent("13/03/2024", Encoding.UTF8, MediaTypeNames.Text.Plain), "data_ini" },
                    { new StringContent("00:00", Encoding.UTF8, MediaTypeNames.Text.Plain), "hora_inicio" },
                    { new StringContent("13/03/2024", Encoding.UTF8, MediaTypeNames.Text.Plain), "data_final" },
                    { new StringContent("23:59", Encoding.UTF8, MediaTypeNames.Text.Plain), "hora_fim" },
                    { new StringContent("'atitude'", Encoding.UTF8, MediaTypeNames.Text.Plain), "List_Queue[]" },
                    { new StringContent("'Agent/9234'", Encoding.UTF8, MediaTypeNames.Text.Plain), "List_Agent[]" },
                    { new StringContent("124", Encoding.UTF8, MediaTypeNames.Text.Plain), "totagentes" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "start" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "end" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "secondsstart" },
                    { new StringContent("tv", Encoding.UTF8, MediaTypeNames.Text.Plain), "secondsend" }
                };

                //Filtrar relatório
                var relatorio = httpClientWithCookies.PostAsync("http://192.168.0.6/integrador/library/fila/answered.php", filtroRelatorio).Result;

                //Fila com os agentes
                var consult1 = httpClientWithCookies.GetAsync("http://192.168.0.6/integrador/library/fila/realtime_ajax.php").Result;

                //Coleta o retorno e define a fila
                fila = consult1.Content.ReadAsStringAsync().Result;
                Console.WriteLine(fila); //Debug

                //Retorna o Token + a Fila (Utilizado como teste atualmente)
                if (token == string.Empty)
                    token = "Token não gerado!";
                return "Token: " + token + "\nFila: " + fila;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public HttpClient ServiceHTTPClient
        {
            get
            {
                //HttpClientHandler com cCookies
                //var handler = new HttpClientHandler();
                //handler.CookieContainer = new CookieContainer();
                //handler.UseCookies = true;

                //HttpClient
                var SerHTTPClient = new HttpClient();
                SerHTTPClient.Timeout = new TimeSpan(0, 5, 0);
                //SerHTTPClient.BaseAddress = new Uri("http://192.168.0.6/integrador");
                SerHTTPClient.BaseAddress = new Uri("http://192.168.0.6/integrador");

                return SerHTTPClient;
            }

        }
    }
}