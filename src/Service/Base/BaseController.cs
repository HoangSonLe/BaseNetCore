﻿using Application.Services.WebInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Base
{
    public class BaseController<T> : ControllerBase
    {
        private readonly ILogger<T> _logger;
        private readonly IUserService _userService;


        public string _currentUserId => HttpContext.User.FindFirst("UserID").Value;

        public BaseController(ILogger<T> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        public IUserService UserService => _userService;
        public ILogger<T> Logger => _logger;
    }
}