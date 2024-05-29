using LivePaint.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace LivePaint.Server.Hubs
{
    public class DrawHub:Hub
    {
        public async Task SendDrawing(DrawModel drawModel)
        {
            await Clients.Others.SendAsync("ReceiveDrawing", drawModel); 
        }
    }
}
