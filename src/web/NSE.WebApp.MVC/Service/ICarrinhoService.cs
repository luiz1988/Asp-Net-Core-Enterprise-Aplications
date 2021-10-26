using NSE.WebApp.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Service
{
    public interface ICarrinhoService
    {
        Task<CarrinhoViewModel> ObterCarrinho();
        Task<ResponseResult> AdicionarItemCarrinho(ItemProdutoViewModel produto);
        Task<ResponseResult> AtualizarItemCarrinho(Guid produtoId, ItemProdutoViewModel produto);
        Task<ResponseResult> RemoverItemCarrinho(Guid produtoId);
    }
}
