using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogoController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet]
        [Route("")]
        [Route("vitrine")]
        public async Task<IActionResult> Index()
        {
            var produtos = await _catalogoService.ObterTodos();
            return View(produtos);
        }

        [HttpGet]
        [Route("produto-detalhe/{id:guid}")]
        public async Task<IActionResult> ProdutoDetalhe(Guid id)
        {
                var produto = await _catalogoService.ObterPorId(id);
                return View(produto);
        }
    }
}
