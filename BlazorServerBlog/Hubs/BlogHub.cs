using Microsoft.AspNetCore.SignalR;

namespace BlazorServerBlog.Hubs
{
    public class BlogHub : Hub
    {
        public async Task SendMessage(bool message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
