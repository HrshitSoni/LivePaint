using LivePaint.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace LivePaint.Server.Hubs
{
    public class DrawHub:Hub
    {
        public async Task SendDrawing(DrawModel draw)
        {
            await Clients.Others.SendAsync("ReceiveDrawing", draw); 
        }
    }
}
