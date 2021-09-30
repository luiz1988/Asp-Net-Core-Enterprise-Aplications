using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Extentions;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class IdentidadeController : MainController
    {
        private readonly IAutenticacaoService _authService;
        public IdentidadeController(IAutenticacaoService authService)
        {
            _authService = authService;
        }

        [HttpGet("nova-conta")]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost("nova-conta")]
        public async Task<IActionResult> Registro(UsuarioRegistro usuarioRegistro)
        {
            //var teste = new HttpResponseMessage();
            //throw new CustomHttpRequestException(teste.StatusCode);


            //return new StatusCodeResult(401);
            if (!ModelState.IsValid)
                return View(usuarioRegistro);

            //Se comunicar com a API - Registro
            var resposta = await _authService.Registro(usuarioRegistro);

            if (ResponsePossuiErros(resposta.ResponseResult))
                return View(usuarioRegistro);

            //// Realizar login na API
            //return RedirectToAction("Index", "Catalogo");

            //if (false) return View(usuarioRegistro);

            //return RedirectToAction("Index", "Catalogo

            await RealizarLogin(resposta);
            return RedirectToAction("Index", "Catalogo");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if(returnUrl == null)
                ModelState.Remove("returnUrl");

            if (!ModelState.IsValid)
                return View(usuarioLogin);

            //TODO: Se comunicar com a API - Login
            var resposta = await _authService.Login(usuarioLogin);

            if (ResponsePossuiErros(resposta.ResponseResult))
                return View(usuarioLogin);

            // Realizar login na API
            await RealizarLogin(resposta);

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");

            return LocalRedirect(returnUrl);
        }

        [HttpGet("sair")]
        public async Task<IActionResult> Logout()
        {
            // TODO: Programar limpa do cookie da sessão para derrubar usuário.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task RealizarLogin(UsuarioRespostaLogin resposta)
        {
            var token = ObterTokenFormatado(resposta.AccessToken);

            var claims = new List<Claim>();

            claims.Add(new Claim("JWT", resposta.AccessToken));
            claims.AddRange(token.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true
            };

            ///<summary>
            ///Configura que o HttpContext vai trabalhar no esquema de autenticação de usuários utilizando Cookies.
            ///</summary>
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        private static JwtSecurityToken ObterTokenFormatado(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
        }
    }
}
