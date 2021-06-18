using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Deer.WebSockets;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebsocketServerDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;
        private IDeerWebSocketConnectionManager<TestDeerWebsocket> _connectionManager;
        public TestController(ILogger<TestController> logger, IDeerWebSocketConnectionManager<TestDeerWebsocket> connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }

        [HttpGet]
        public async Task<string> Get()
        {

            var connections = await _connectionManager.GetAllAsync();


            return $"当前连接数：{connections.Count()}";
        }
    }
}