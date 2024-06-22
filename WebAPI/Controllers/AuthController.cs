﻿using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
            :base(logger)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            LogInformation("AuthController Login method called.");
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                LogError(new Exception(userToLogin.Message));
                return BadRequest(userToLogin.Message);
            }

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                LogInformation($"Login successfully");
                return Ok(result);
            }
            LogError(new Exception(result.Message));
            return BadRequest(result.Message);
        }

        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            LogInformation("AuthController Register method called.");
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                LogError(new Exception(userExists.Message));
                return BadRequest(userExists.Message);
            }

            var registerResult = _authService.Register(userForRegisterDto);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success)
            {
                LogInformation($"Register successfully");
                return Ok(result.Data);
            }
            LogError(new Exception(result.Message));
            return BadRequest(result.Message);
        }
    }
}
