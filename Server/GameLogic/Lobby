using SeaBattle.Protocol;

namespace Server.GameLogic
{
    public class Lobby
    {
        private readonly List<ClientHandler> _waitingPlayers = new();
        private readonly List<GameRoom> _activeRooms = new();
        private readonly object _lock = new();

        public void AddPlayer(ClientHandler player)
        {
            lock (_lock)
            {
                if (!_waitingPlayers.Contains(player))
                {
                    player.IsInLobby = true;
                    _waitingPlayers.Add(player);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOBBY] Гравець {player.Name} увійшов у лоббі. В черзі: {_waitingPlayers.Count}");

                    player.MessageReceived += OnPlayerMessage;
                    player.Disconnected += OnPlayerDisconnected;

                    BroadcastLobbyStatusUnsafe();
                    CheckAndCreateMatchUnsafe();
                }
            }
        }

        private void RemovePlayerUnsafe(ClientHandler player)
        {
            player.IsInLobby = false;
            player.MessageReceived -= OnPlayerMessage;
            player.Disconnected -= OnPlayerDisconnected;
            _waitingPlayers.Remove(player);
            BroadcastLobbyStatusUnsafe();
        }

        private void OnPlayerMessage(ClientHandler sender, NetworkMessage message)
        {
            if (message.Type == MessageTypes.LeaveGame)
            {
                lock (_lock)
                {
                    RemovePlayerUnsafe(sender);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOBBY] Гравець {sender.Name} залишив чергу.");
                }
            }
        }

        private void OnPlayerDisconnected(ClientHandler sender)
        {
            lock (_lock)
            {
                RemovePlayerUnsafe(sender);
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOBBY] Гравець {sender.Name} відключився з лоббі.");
            }
        }

        private void BroadcastLobbyStatusUnsafe()
        {
            int count = _waitingPlayers.Count;
            foreach (var p in _waitingPlayers)
            {
                _ = p.SendAsync(MessageTypes.LobbyStatus, new LobbyStatusDto
                {
                    PlayersInQueue = count,
                    Message = count % 2 == 1 
                        ? $"Очікування суперника... (Гравців у черзі: {count})" 
                        : $"Формування гри... (Гравців у черзі: {count})"
                });
            }
        }

        private void CheckAndCreateMatchUnsafe()
        {
            // Коли набирається парна кількість гравців (>= 2), сервер випадковим чином (Random) об'єднує їх у пари та створює для них окрему Кімнату
            while (_waitingPlayers.Count >= 2)
            {
                // Обираємо двох випадкових гравців
                int idx1 = Random.Shared.Next(_waitingPlayers.Count);
                var p1 = _waitingPlayers[idx1];
                _waitingPlayers.RemoveAt(idx1);

                int idx2 = Random.Shared.Next(_waitingPlayers.Count);
                var p2 = _waitingPlayers[idx2];
                _waitingPlayers.RemoveAt(idx2);

                p1.IsInLobby = false;
                p2.IsInLobby = false;
                p1.MessageReceived -= OnPlayerMessage;
                p1.Disconnected -= OnPlayerDisconnected;
                p2.MessageReceived -= OnPlayerMessage;
                p2.Disconnected -= OnPlayerDisconnected;

                var room = new GameRoom(p1, p2);
                _activeRooms.Add(room);

                room.RoomClosed += OnRoomClosed;
                _ = room.StartRoomAsync();
            }

            BroadcastLobbyStatusUnsafe();
        }

        private void OnRoomClosed(GameRoom room)
        {
            lock (_lock)
            {
                _activeRooms.Remove(room);
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOBBY] Кімнату {room.Id} закрито. Активних кімнат: {_activeRooms.Count}");
            }
        }
    }
}
