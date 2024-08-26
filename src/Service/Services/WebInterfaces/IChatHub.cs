using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services.WebInterfaces
{
    public interface IChatHub
    {
        Task ForceLogout(string message);
        Task BroadcastMessage(string name, string message);

    }

    [Authorize]
    public class ChatHub : Hub<IChatHub>,IChatHub
    {
        public Task BroadcastMessage(string name, string message)
        {
            throw new NotImplementedException();
        }

        public Task ForceLogout(string message)
        {
            throw new NotImplementedException();
        }

        public void Send(string name, string message)
        {
            Clients.All.BroadcastMessage(name, message);
        }
    }
}
