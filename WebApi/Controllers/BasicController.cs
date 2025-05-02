﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    public class BasicController<T> : ControllerBase
    {
        private ISender _sender;

        public ISender MediatorSender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
    }
}
