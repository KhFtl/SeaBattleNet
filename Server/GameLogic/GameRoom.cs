using System.Text.Json;
using SeaBattle.Protocol;

namespace Server.GameLogic
{
    public class GameRoom
    {
        public string Id { get; }
        public ClientHandler Player1 { get; }
        public ClientHandler Player2 { get; }

        public PlayerBoard Board1 { get; } = new();
        public PlayerBoard Board2 { get; } = new();

        public bool IsPlayer1Ready { get; private set; } = false;
        public bool IsPlayer2Ready { get; private set; } = false;
        public bool IsGameStarted { get; private set; } = false;
        public bool IsGameOver { get; private set; } = false;

        public ClientHandler? CurrentTurnPlayer { get; private set; }

        public event Action<GameRoom>? RoomClosed;

        public GameRoom(ClientHandler p1, ClientHandler p2)
        {
            Id = Guid.NewGuid().ToString("N")[..8];
            Player1 = p1;
            Player2 = p2;

            Player1.RoomId = Id;
            Player2.RoomId = Id;

            Player1.MessageReceived += OnPlayerMessage;
            Player2.MessageReceived += OnPlayerMessage;

            Player1.Disconnected += OnPlayerDisconnected;
            Player2.Disconnected += OnPlayerDisconnected;
        }

        public async Task StartRoomAsync()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] Створено кімнату для {Player1.Name} та {Player2.Name}.");

            await Player1.SendAsync(MessageTypes.MatchFound, new MatchFoundDto { RoomId = Id, OpponentName = Player2.Name });
            await Player2.SendAsync(MessageTypes.MatchFound, new MatchFoundDto { RoomId = Id, OpponentName = Player1.Name });
        }

        private async void OnPlayerMessage(ClientHandler sender, NetworkMessage message)
        {
            try
            {
                if (IsGameOver) return;

                switch (message.Type)
                {
                    case MessageTypes.SetReady:
                        await HandleSetReadyAsync(sender, message.Payload);
                        break;

                    case MessageTypes.Shoot:
                        await HandleShootAsync(sender, message.Payload);
                        break;

                    case MessageTypes.LeaveGame:
                        await HandleLeaveGameAsync(sender);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] [ROOM {Id}] Помилка обробки повідомлення: {ex.Message}");
            }
        }

        private async Task HandleSetReadyAsync(ClientHandler sender, string payload)
        {
            if (IsGameStarted) return;

            List<ShipPlacementDto>? placements = null;
            try
            {
                placements = JsonSerializer.Deserialize<List<ShipPlacementDto>>(payload);
            }
            catch
            {
                await sender.SendAsync(MessageTypes.Error, "Некоректний формат даних розстановки кораблів.");
                return;
            }

            var (isValid, errorMsg, ships) = PlayerBoard.ValidateAndCreate(placements);
            if (!isValid || ships == null)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] Невалідна розстановка від {sender.Name}: {errorMsg}");
                await sender.SendAsync(MessageTypes.Error, $"Помилка валідації розстановки: {errorMsg}");
                return;
            }

            if (sender == Player1)
            {
                Board1.Initialize(ships);
                IsPlayer1Ready = true;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] {Player1.Name} готовий до бою.");
            }
            else if (sender == Player2)
            {
                Board2.Initialize(ships);
                IsPlayer2Ready = true;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] {Player2.Name} готовий до бою.");
            }

            if (IsPlayer1Ready && IsPlayer2Ready)
            {
                await StartBattleAsync();
            }
        }

        private async Task StartBattleAsync()
        {
            IsGameStarted = true;
            // Випадково обираємо хто ходить першим
            bool p1First = Random.Shared.Next(2) == 0;
            CurrentTurnPlayer = p1First ? Player1 : Player2;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] Бій розпочато! Перший хід робить: {CurrentTurnPlayer.Name}");

            await Player1.SendAsync(MessageTypes.GameStart, new GameStartDto
            {
                IsMyTurn = p1First,
                OpponentName = Player2.Name
            });

            await Player2.SendAsync(MessageTypes.GameStart, new GameStartDto
            {
                IsMyTurn = !p1First,
                OpponentName = Player1.Name
            });
        }

        private async Task HandleShootAsync(ClientHandler sender, string payload)
        {
            if (!IsGameStarted || IsGameOver) return;

            if (sender != CurrentTurnPlayer)
            {
                await sender.SendAsync(MessageTypes.Error, "Зараз не ваш хід!");
                return;
            }

            ShootRequestDto? shotReq = null;
            try
            {
                shotReq = JsonSerializer.Deserialize<ShootRequestDto>(payload);
            }
            catch
            {
                return;
            }

            if (shotReq == null || shotReq.X < 0 || shotReq.X >= 10 || shotReq.Y < 0 || shotReq.Y >= 10)
            {
                await sender.SendAsync(MessageTypes.Error, "Некоректні координати пострілу.");
                return;
            }

            bool isP1Shooting = (sender == Player1);
            PlayerBoard targetBoard = isP1Shooting ? Board2 : Board1;
            ClientHandler opponent = isP1Shooting ? Player2 : Player1;

            char colChar = (char)('А' + shotReq.X);
            string coordText = $"{colChar}{shotReq.Y + 1}";

            var (outcome, hitShip, surroundingMisses) = targetBoard.ProcessShot(shotReq.X, shotReq.Y);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] Постріл {sender.Name} по ({coordText}): {outcome}");

            List<CellCoordDto>? killedCellsDto = null;
            if (outcome == ShotOutcome.Kill && hitShip != null)
            {
                killedCellsDto = hitShip.Cells.Select(c => new CellCoordDto { X = c.X, Y = c.Y }).ToList();
            }

            var surroundingMissCellsDto = surroundingMisses.Select(c => new CellCoordDto { X = c.X, Y = c.Y }).ToList();

            // Повідомлення для того, хто стріляв (Attacker)
            await sender.SendAsync(MessageTypes.ShotResult, new ShotResultDto
            {
                X = shotReq.X,
                Y = shotReq.Y,
                IsAttacker = true,
                Outcome = outcome,
                KilledShipCells = killedCellsDto,
                SurroundingMissCells = surroundingMissCellsDto
            });

            // Повідомлення для того, по кому стріляли (Defender)
            await opponent.SendAsync(MessageTypes.ShotResult, new ShotResultDto
            {
                X = shotReq.X,
                Y = shotReq.Y,
                IsAttacker = false,
                Outcome = outcome,
                KilledShipCells = killedCellsDto,
                SurroundingMissCells = surroundingMissCellsDto
            });

            // Перевірка завершення гри
            if (targetBoard.AreAllShipsSunk)
            {
                IsGameOver = true;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] Всі кораблі {opponent.Name} знищено! Переможець: {sender.Name}");

                await sender.SendAsync(MessageTypes.GameOver, new GameOverDto
                {
                    IsWinner = true,
                    Reason = "Ви знищили всі кораблі суперника! Перемога!"
                });

                await opponent.SendAsync(MessageTypes.GameOver, new GameOverDto
                {
                    IsWinner = false,
                    Reason = "Всі ваші кораблі знищено. Поразка."
                });

                CloseRoom();
                return;
            }

            // Логіка черговості ходу: при влучанні (Hit або Kill) гравець ходить знову, при Miss - хід переходить супернику
            if (outcome == ShotOutcome.Miss)
            {
                CurrentTurnPlayer = opponent;
                await sender.SendAsync(MessageTypes.TurnChanged, new TurnChangedDto { IsMyTurn = false });
                await opponent.SendAsync(MessageTypes.TurnChanged, new TurnChangedDto { IsMyTurn = true });
            }
            else
            {
                // Додатковий хід тому ж гравцю
                await sender.SendAsync(MessageTypes.TurnChanged, new TurnChangedDto { IsMyTurn = true });
                await opponent.SendAsync(MessageTypes.TurnChanged, new TurnChangedDto { IsMyTurn = false });
            }
        }

        private async Task HandleLeaveGameAsync(ClientHandler sender)
        {
            if (IsGameOver) return;
            IsGameOver = true;

            var opponent = (sender == Player1) ? Player2 : Player1;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] {sender.Name} здався або залишив гру.");

            await opponent.SendAsync(MessageTypes.GameOver, new GameOverDto
            {
                IsWinner = true,
                Reason = "Суперник покинув гру. Вам зараховано технічну перемогу!"
            });

            await sender.SendAsync(MessageTypes.GameOver, new GameOverDto
            {
                IsWinner = false,
                Reason = "Ви покинули гру. Зараховано технічну поразку."
            });

            CloseRoom();
        }

        private async void OnPlayerDisconnected(ClientHandler disconnectedPlayer)
        {
            if (IsGameOver) return;
            IsGameOver = true;

            var remainingPlayer = (disconnectedPlayer == Player1) ? Player2 : Player1;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ROOM {Id}] {disconnectedPlayer.Name} відключився. Технічна перемога {remainingPlayer.Name}.");

            await remainingPlayer.SendAsync(MessageTypes.GameOver, new GameOverDto
            {
                IsWinner = true,
                Reason = "Суперник втратив зв'язок із сервером. Вам зараховано технічну перемогу!"
            });

            CloseRoom();
        }

        private void CloseRoom()
        {
            Player1.MessageReceived -= OnPlayerMessage;
            Player2.MessageReceived -= OnPlayerMessage;
            Player1.Disconnected -= OnPlayerDisconnected;
            Player2.Disconnected -= OnPlayerDisconnected;

            Player1.RoomId = null;
            Player2.RoomId = null;

            RoomClosed?.Invoke(this);
        }
    }
}
