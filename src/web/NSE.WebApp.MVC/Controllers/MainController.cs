using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using System.Linq;

namespace NSE.WebApp.MVC.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponsePossuiErros(ResponseResult resposta)
        {
            if (resposta != null && resposta.Errors.Mensagens.Any())
            {
                foreach (var mensagem in resposta.Errors.Mensagens) //Varendo a lista de mensagens de erro do nosso objeto resposta do tipo ResponseResult
                {
                    ModelState.AddModelError(string.Empty, mensagem); // Agora adicionaremos as mensagens uma a uma na nossa ModelState para que possamos repassar essas mensagens de erro para os nossos usuários!
                }

                return true;
            }

            return false;
        }
    }
}
