using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FaceDetection;
using FaceDetection.FaceMaskDetector;
using FaceDetectionWebAPI.models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FaceDetectionWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<FaceDetector>();
            services.AddSingleton<FaceMaskDetector>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FaceDetector faceDetector, FaceMaskDetector faceMaskDetector)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket, faceDetector, faceMaskDetector);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });
        }

        #region Echo
        private async Task Echo(HttpContext context, WebSocket webSocket, FaceDetector faceDetector, FaceMaskDetector faceMaskDetector)
        {
            var buffer = new byte[1024 * 1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                byte[] imageData = buffer[..result.Count];
                IEnumerable<FaceRectangle> faces = (await faceDetector.DetectAsync(imageData))
                    .Select(face => new FaceRectangle
                    {
                        Height = face["height"],
                        Left = face["left"],
                        Top = face["top"],
                        Width = face["width"],
                        X1 = face["x1"],
                        X2 = face["x2"],
                        Y1 = face["y1"],
                        Y2 = face["y2"]
                    });

                List<FaceModel> faceModels = new List<FaceModel>();
                Parallel.ForEach(faces, (face) => {
                    faceModels.Add(Utils.FaceAnalysis(faceMaskDetector, imageData, face, true));
                });

                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(faceModels))), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        #endregion
    }
}
