using Chess.Dal.Interfaces;
using Chess.Domain.DTO;
using Chess.Domain.Entities;
using Chess.Domain.Helpers;
using Chess.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Chess.Models
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<GameHub> _hub;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IServiceScopeFactory scopeFactory,
                       IHubContext<GameHub> hub,
                       ILogger<GameHub> logger)
        {
            _scopeFactory = scopeFactory;
            _hub = hub;
            _logger = logger;
        }

        private long? GetUserId()
        {
            var id = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(id, out var v) ? v : null;
        }

        private string? GetToken() => Context.Items["token"] as string;

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            var token = GetToken();
            if (token != null)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var rooms = scope.ServiceProvider.GetRequiredService<IActiveRoomRepository>();

                var room = await rooms.GetByToken(token);
                if (room != null)
                {
                    bool isWhite = room.WhiteConnectionId == Context.ConnectionId;
                    bool isBlack = room.BlackConnectionId == Context.ConnectionId;

                    if (isWhite) room.WhiteConnectionId = null;
                    if (isBlack) room.BlackConnectionId = null;

                    if (room.WhiteConnectionId == null && room.BlackConnectionId == null)
                        room.LastHeartbeat = DateTime.UtcNow.AddMinutes(-5);

                    await rooms.Update(room);

                    await _hub.Clients.Group(token).SendAsync("OpponentDisconnected",
                        isWhite ? "white" : "black");
                }
            }
            await base.OnDisconnectedAsync(ex);
        }


        public async Task<object> CreateRoom(string name, string color)
        {
            var userId = GetUserId();
            if (userId == null) throw new HubException("Не авторизован");

            bool isWhite;
            if (color == "Random") isWhite = Random.Shared.Next(2) == 0;
            else isWhite = color != "Black";

            var token = Guid.NewGuid().ToString("N");
            var board = ChessHelper.CreateStartBoard();

            var room = new ActiveRoom
            {
                Name = name,
                Elo = 100,
                Token = token,
                OwnerUserId = userId,
                CreatedAt = DateTime.UtcNow,
                LastHeartbeat = DateTime.UtcNow,
                IsWhite = isWhite,
                IsWhiteTurn = true,
                IsMate = false,
                BoardState = ChessHelper.Serialize(board),
                WhiteConnectionId = isWhite ? Context.ConnectionId : null,
                BlackConnectionId = isWhite ? null : Context.ConnectionId
            };

            await using var scope = _scopeFactory.CreateAsyncScope();
            var rooms = scope.ServiceProvider.GetRequiredService<IActiveRoomRepository>();
            await rooms.Create(room);

            Context.Items["token"] = token;
            await Groups.AddToGroupAsync(Context.ConnectionId, token);

            return new
            {
                token,
                isWhite,
                boardState = room.BoardState,
                isWhiteTurn = room.IsWhiteTurn
            };
        }


        public async Task<object> JoinRoom(string token)
        {
            var userId = GetUserId();
            if (userId == null) throw new HubException("Не авторизован");

            await using var scope = _scopeFactory.CreateAsyncScope();
            var rooms = scope.ServiceProvider.GetRequiredService<IActiveRoomRepository>();
            var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var room = await rooms.GetByToken(token);
            if (room == null) throw new HubException("Комната не найдена");

            bool amOwner = room.OwnerUserId == userId;
            bool amGuest = room.GuestUserId == userId;

            if (!amOwner && !amGuest && room.GuestUserId != null)
                throw new HubException("Комната заполнена");

            bool asWhite;
            if (amOwner)
            {
                asWhite = room.IsWhite;
                room.WhiteConnectionId = asWhite ? Context.ConnectionId : room.WhiteConnectionId;
                room.BlackConnectionId = !asWhite ? Context.ConnectionId : room.BlackConnectionId;
            }
            else if (amGuest)
            {
                asWhite = !room.IsWhite;
                room.WhiteConnectionId = asWhite ? Context.ConnectionId : room.WhiteConnectionId;
                room.BlackConnectionId = !asWhite ? Context.ConnectionId : room.BlackConnectionId;
            }
            else
            {
                room.GuestUserId = userId;
                asWhite = !room.IsWhite;
                room.WhiteConnectionId = asWhite ? Context.ConnectionId : room.WhiteConnectionId;
                room.BlackConnectionId = !asWhite ? Context.ConnectionId : room.BlackConnectionId;
            }

            room.LastHeartbeat = DateTime.UtcNow;
            await rooms.Update(room);

            Context.Items["token"] = token;
            await Groups.AddToGroupAsync(Context.ConnectionId, token);

            await _hub.Clients.Group(token).SendAsync("OpponentJoined", "Соперник");

            return new
            {
                token,
                isWhite = asWhite,
                boardState = room.BoardState,
                isWhiteTurn = room.IsWhiteTurn,
                isMate = room.IsMate
            };
        }


        public async Task<object> SelectPiece(MoveRequest req)
        {
            var (room, playerIsWhite, _) = await LoadRoomForPlayer(req.Token);
            if (room == null) throw new HubException("Комната не найдена");
            if (room.IsMate) throw new HubException("Игра окончена");

            await using var scope = _scopeFactory.CreateAsyncScope();
            var games = scope.ServiceProvider.GetRequiredService<IGameService>();
            var outcome = await games.SelectPieceAsync(room, playerIsWhite, req);
            if (!outcome.Success) throw new HubException(outcome.Error ?? "Ошибка выбора");

            return new { moves = outcome.Moves };
        }


        public async Task MakeMove(MoveRequest req)
        {
            var (room, playerIsWhite, _) = await LoadRoomForPlayer(req.Token);
            if (room == null) throw new HubException("Комната не найдена");
            if (room.IsMate) throw new HubException("Игра окончена");
            if (room.IsWhiteTurn != playerIsWhite) throw new HubException("Сейчас не ваш ход!");

            await using var scope = _scopeFactory.CreateAsyncScope();
            var games = scope.ServiceProvider.GetRequiredService<IGameService>();
            var rooms = scope.ServiceProvider.GetRequiredService<IActiveRoomRepository>();

            var outcome = await games.MakeMoveAsync(room, playerIsWhite, req);
            if (!outcome.Success) throw new HubException(outcome.Error ?? "Ошибка хода");

            await rooms.Update(room);

            await _hub.Clients.Group(req.Token).SendAsync("MoveMade", new
            {
                fromX = req.FromX,
                fromY = req.FromY,
                toX = req.ToX,
                toY = req.ToY,
                boardState = room.BoardState,
                isWhiteTurn = room.IsWhiteTurn,
                isMate = room.IsMate
            });

            if (room.IsMate)
                await _hub.Clients.Group(req.Token).SendAsync("GameOver",
                    new { winner = playerIsWhite ? "white" : "black" });
        }


        public async Task Heartbeat(string token)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var rooms = scope.ServiceProvider.GetRequiredService<IActiveRoomRepository>();
            await rooms.TouchHeartbeat(token);
        }

        public async Task LeaveRoom(string token)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var rooms = scope.ServiceProvider.GetRequiredService<IActiveRoomRepository>();

            var room = await rooms.GetByToken(token);
            if (room != null)
            {
                if (room.WhiteConnectionId == Context.ConnectionId) room.WhiteConnectionId = null;
                if (room.BlackConnectionId == Context.ConnectionId) room.BlackConnectionId = null;
                await rooms.Update(room);
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, token);
            Context.Items.Remove("token");
        }

        private async Task<(ActiveRoom?, bool, long?)> LoadRoomForPlayer(string token)
        {
            var userId = GetUserId();
            if (userId == null) return (null, false, null);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var rooms = scope.ServiceProvider.GetRequiredService<IActiveRoomRepository>();
            var room = await rooms.GetByToken(token);
            if (room == null) return (null, false, userId);

            bool isWhite;
            if (room.OwnerUserId == userId) isWhite = room.IsWhite;
            else if (room.GuestUserId == userId) isWhite = !room.IsWhite;
            else return (null, false, userId);

            return (room, isWhite, userId);
        }
    }
}