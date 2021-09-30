using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Extentions;
using NSE.WebApp.MVC.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Service
{
    public class AutenticacaoService : Service, IAutenticacaoService
    {
        //HttpClient: Biblioteca usada para se comunicar com serviços externos como APIs por meio de requisições HTTP.
        private readonly HttpClient _httpClient;

        public AutenticacaoService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
           httpClient.BaseAddress = new Uri(settings.Value.AutenticacaoUrl); //Define o endereço base do nosso HttpClient como sendo o da url de AutenticacaoUrl presente no arquivo appsettings.

            _httpClient = httpClient;
        }

        public async Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            var loginContent = ObterConteudo(usuarioLogin);

            var response = await _httpClient.PostAsync("/api/identidade/autenticar", loginContent);


            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }

        public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistro usuarioRegistro)
        {
            var registerContent = ObterConteudo(usuarioRegistro);

            var response = await _httpClient.PostAsync("/api/identidade/nova-conta", registerContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }

        // Não se consome serviços externos de APIs de forma sincrona portanto utilizaremos métodos com chamadas assincronas a API.
    }
}
