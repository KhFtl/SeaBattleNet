using System.Text.Json;
using SeaBattle.Protocol;

namespace SeaBattleNet
{
    public partial class MainForm : Form
    {
        private readonly NetworkClient _client = new();
        private List<ShipPlacementDto> _myFleet = new();
        private bool _isMyTurn = false;
        private bool _isGameRunning = false;
        private bool _isReadySent = false;

        // Ручна розстановка кораблів
        private int _selectedShipSize = 4;
        private bool _isHorizontal = true;
        private readonly Dictionary<int, int> _maxShipCounts = new()
        {
            { 4, 1 },
            { 3, 2 },
            { 2, 3 },
            { 1, 4 }
        };

        public MainForm()
        {
            InitializeComponent();

            _client.Connected += OnConnected;
            _client.Disconnected += OnDisconnected;
            _client.MessageReceived += OnMessageReceived;
            _client.ErrorOccurred += OnError;

            // Підписка на події дощок
            opponentBoardControl.CellClicked += OnOpponentCellClicked;
            myBoardControl.BoardClicked += OnMyBoardClicked;
            myBoardControl.HoverCellChanged += OnMyBoardHoverChanged;

            // Початковий стан розміщення
            SetupPlacementMode(true);
            GenerateAndDisplayFleet();
        }

        private void SetupPlacementMode(bool enabled)
        {
            myBoardControl.IsInteractive = enabled;
            myBoardControl.IsPlacementMode = enabled;
            myBoardControl.PlacementShipSize = _selectedShipSize;
            myBoardControl.PlacementIsHorizontal = _isHorizontal;

            btnShip4.Visible = enabled;
            btnShip3.Visible = enabled;
            btnShip2.Visible = enabled;
            btnShip1.Visible = enabled;
            btnRotate.Visible = enabled;
            btnClearBoard.Visible = enabled;
            btnAutoPlace.Visible = enabled;
            lblPlacementTip.Visible = enabled;

            if (enabled)
            {
                UpdateShipPlacementUI();
            }
        }

        private void UpdateShipPlacementUI()
        {
            var currentCounts = GetCurrentShipCounts();

            int left4 = _maxShipCounts[4] - currentCounts[4];
            int left3 = _maxShipCounts[3] - currentCounts[3];
            int left2 = _maxShipCounts[2] - currentCounts[2];
            int left1 = _maxShipCounts[1] - currentCounts[1];

            btnShip4.Text = $"4-пал ({left4})";
            btnShip3.Text = $"3-пал ({left3})";
            btnShip2.Text = $"2-пал ({left2})";
            btnShip1.Text = $"1-пал ({left1})";

            HighlightShipButton(btnShip4, _selectedShipSize == 4, left4 > 0);
            HighlightShipButton(btnShip3, _selectedShipSize == 3, left3 > 0);
            HighlightShipButton(btnShip2, _selectedShipSize == 2, left2 > 0);
            HighlightShipButton(btnShip1, _selectedShipSize == 1, left1 > 0);

            btnRotate.Text = _isHorizontal ? "🔄 Орієнтація: Горизонт." : "🔄 Орієнтація: Вертикальн.";

            myBoardControl.PlacementShipSize = _selectedShipSize;
            myBoardControl.PlacementIsHorizontal = _isHorizontal;

            bool isFullFleet = _myFleet.Count == 10 &&
                               left4 == 0 && left3 == 0 && left2 == 0 && left1 == 0;

            btnReady.Enabled = isFullFleet && !_isReadySent;
            btnReady.BackColor = isFullFleet ? Color.FromArgb(46, 125, 50) : Color.FromArgb(80, 80, 80);

            if (!_isGameRunning && !_isReadySent)
            {
                if (isFullFleet)
                {
                    lblGameStatus.Text = "Флот укомплектовано! Натисніть «ГОТОВИЙ ДО БОЮ».";
                    lblGameStatus.ForeColor = Color.FromArgb(100, 255, 150);
                }
                else
                {
                    int totalLeft = left4 + left3 + left2 + left1;
                    lblGameStatus.Text = $"Розставте кораблі (залишилось {totalLeft}) або скористайтесь авторозстановкою.";
                    lblGameStatus.ForeColor = Color.FromArgb(120, 210, 255);
                }
            }
        }

        private void HighlightShipButton(Button btn, bool isSelected, bool hasAvailable)
        {
            if (isSelected)
            {
                btn.BackColor = Color.FromArgb(0, 150, 136);
                btn.FlatAppearance.BorderColor = Color.Cyan;
                btn.FlatAppearance.BorderSize = 2;
            }
            else if (hasAvailable)
            {
                btn.BackColor = Color.FromArgb(40, 70, 110);
                btn.FlatAppearance.BorderColor = Color.FromArgb(70, 110, 160);
                btn.FlatAppearance.BorderSize = 1;
            }
            else
            {
                btn.BackColor = Color.FromArgb(35, 45, 60);
                btn.FlatAppearance.BorderColor = Color.FromArgb(50, 60, 75);
                btn.FlatAppearance.BorderSize = 1;
            }
        }

        private Dictionary<int, int> GetCurrentShipCounts()
        {
            var counts = new Dictionary<int, int> { { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 } };
            foreach (var ship in _myFleet)
            {
                if (counts.ContainsKey(ship.Size))
                {
                    counts[ship.Size]++;
                }
            }
            return counts;
        }

        private void BtnShip_Click(object? sender, EventArgs e)
        {
            if (sender == btnShip4) _selectedShipSize = 4;
            else if (sender == btnShip3) _selectedShipSize = 3;
            else if (sender == btnShip2) _selectedShipSize = 2;
            else if (sender == btnShip1) _selectedShipSize = 1;

            UpdateShipPlacementUI();
            myBoardControl.Invalidate();
        }

        private void BtnRotate_Click(object? sender, EventArgs e)
        {
            _isHorizontal = !_isHorizontal;
            UpdateShipPlacementUI();
            myBoardControl.Invalidate();
        }

        private void BtnClearBoard_Click(object? sender, EventArgs e)
        {
            if (_isReadySent) return;
            _myFleet.Clear();
            RedrawFleetOnBoard();
            SelectFirstAvailableShipSize();
            UpdateShipPlacementUI();
            Log("Поле очищено. Розставте кораблі вручну або оберіть Авто.", Color.LightBlue);
        }

        private void SelectFirstAvailableShipSize()
        {
            var counts = GetCurrentShipCounts();
            foreach (int size in new[] { 4, 3, 2, 1 })
            {
                if (counts[size] < _maxShipCounts[size])
                {
                    _selectedShipSize = size;
                    break;
                }
            }
        }

        private void RedrawFleetOnBoard()
        {
            myBoardControl.Clear();
            foreach (var ship in _myFleet)
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    int cx = ship.IsHorizontal ? ship.X + i : ship.X;
                    int cy = ship.IsHorizontal ? ship.Y : ship.Y + i;
                    myBoardControl.SetShip(cx, cy);
                }
            }
        }

        private bool IsPlacementValid(int x, int y, int size, bool isHoriz, ShipPlacementDto? ignoreShip = null)
        {
            int endX = isHoriz ? x + size - 1 : x;
            int endY = isHoriz ? y : y + size - 1;

            if (x < 0 || y < 0 || endX >= 10 || endY >= 10)
            {
                return false;
            }

            var newShipCells = new List<(int X, int Y)>();
            for (int i = 0; i < size; i++)
            {
                newShipCells.Add((isHoriz ? x + i : x, isHoriz ? y : y + i));
            }

            foreach (var ship in _myFleet)
            {
                if (ship == ignoreShip) continue;

                var existingCells = new List<(int X, int Y)>();
                for (int i = 0; i < ship.Size; i++)
                {
                    existingCells.Add((ship.IsHorizontal ? ship.X + i : ship.X, ship.IsHorizontal ? ship.Y : ship.Y + i));
                }

                foreach (var ex in existingCells)
                {
                    foreach (var nw in newShipCells)
                    {
                        if (Math.Abs(ex.X - nw.X) <= 1 && Math.Abs(ex.Y - nw.Y) <= 1)
                        {
                            return false; // Порушено дистанцію (мінімум 1 клітинка разом із діагоналями)
                        }
                    }
                }
            }

            return true;
        }

        private ShipPlacementDto? FindShipAtCell(int x, int y)
        {
            foreach (var ship in _myFleet)
            {
                for (int i = 0; i < ship.Size; i++)
                {
                    int cx = ship.IsHorizontal ? ship.X + i : ship.X;
                    int cy = ship.IsHorizontal ? ship.Y : ship.Y + i;
                    if (cx == x && cy == y)
                    {
                        return ship;
                    }
                }
            }
            return null;
        }

        private void OnMyBoardHoverChanged(int x, int y)
        {
            if (!_isGameRunning && !_isReadySent && myBoardControl.IsPlacementMode)
            {
                if (x >= 0 && y >= 0)
                {
                    var counts = GetCurrentShipCounts();
                    if (counts[_selectedShipSize] < _maxShipCounts[_selectedShipSize])
                    {
                        myBoardControl.PlacementIsValid = IsPlacementValid(x, y, _selectedShipSize, _isHorizontal);
                    }
                    else
                    {
                        myBoardControl.PlacementIsValid = false;
                    }
                }
            }
        }

        private void OnMyBoardClicked(int x, int y, MouseButtons button)
        {
            if (_isGameRunning || _isReadySent || !myBoardControl.IsPlacementMode) return;

            if (button == MouseButtons.Right)
            {
                // Поворот корабля
                _isHorizontal = !_isHorizontal;
                UpdateShipPlacementUI();
                OnMyBoardHoverChanged(x, y);
                myBoardControl.Invalidate();
                return;
            }

            if (button == MouseButtons.Left)
            {
                // Перевіряємо, чи клікнули по вже встановленому кораблю для його видалення
                var existingShip = FindShipAtCell(x, y);
                if (existingShip != null)
                {
                    _myFleet.Remove(existingShip);
                    _selectedShipSize = existingShip.Size;
                    RedrawFleetOnBoard();
                    UpdateShipPlacementUI();
                    OnMyBoardHoverChanged(x, y);
                    myBoardControl.Invalidate();
                    Log($"Знято {existingShip.Size}-палубний корабель.", Color.FromArgb(200, 200, 150));
                    return;
                }

                // Спроба розмістити корабель
                var counts = GetCurrentShipCounts();
                if (counts[_selectedShipSize] >= _maxShipCounts[_selectedShipSize])
                {
                    Log($"Усі {_selectedShipSize}-палубні кораблі вже розставлені! Оберіть інший розмір.", Color.Orange);
                    return;
                }

                if (IsPlacementValid(x, y, _selectedShipSize, _isHorizontal))
                {
                    var newShip = new ShipPlacementDto
                    {
                        X = x,
                        Y = y,
                        Size = _selectedShipSize,
                        IsHorizontal = _isHorizontal
                    };

                    _myFleet.Add(newShip);
                    RedrawFleetOnBoard();
                    SelectFirstAvailableShipSize();
                    UpdateShipPlacementUI();
                    OnMyBoardHoverChanged(x, y);
                    myBoardControl.Invalidate();
                    Log($"Встановлено {newShip.Size}-палубний корабель на ({((char)('А' + x))}{y + 1}).", Color.LightGreen);
                }
                else
                {
                    Log("Неможливо встановити корабель тут (виходить за межі поля або порушує правило дистанції в 1 клітинку)!", Color.IndianRed);
                }
            }
        }

        private void GenerateAndDisplayFleet()
        {
            _myFleet = FleetGenerator.GenerateRandomFleet();
            RedrawFleetOnBoard();
            UpdateShipPlacementUI();
        }

        private void Log(string message, Color? color = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(message, color)));
                return;
            }

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = color ?? Color.FromArgb(200, 220, 240);
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLog.ScrollToCaret();
        }

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            string ip = txtIp.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Некоректний порт!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string playerName = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Гравець";
            }

            btnConnect.Enabled = false;
            lblLobbyStatus.Text = "Підключення до сервера...";
            lblLobbyStatus.Visible = true;
            progressBarLobby.Visible = true;
            btnCancelSearch.Visible = true;

            bool success = await _client.ConnectAsync(ip, port);
            if (success)
            {
                await _client.SendAsync(MessageTypes.JoinLobby, playerName);
            }
            else
            {
                btnConnect.Enabled = true;
                lblLobbyStatus.Visible = false;
                progressBarLobby.Visible = false;
                btnCancelSearch.Visible = false;
            }
        }

        private async void BtnCancelSearch_Click(object? sender, EventArgs e)
        {
            await _client.SendAsync(MessageTypes.LeaveGame);
            _client.Disconnect();

            btnConnect.Enabled = true;
            lblLobbyStatus.Visible = false;
            progressBarLobby.Visible = false;
            btnCancelSearch.Visible = false;
        }

        private void OnConnected()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnConnected));
                return;
            }

            lblLobbyStatus.Text = "Успішно підключено! Пошук суперника...";
        }

        private void OnDisconnected()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnDisconnected));
                return;
            }

            if (_isGameRunning)
            {
                Log("Зв'язок із сервером втрачено.", Color.Red);
                lblGameStatus.Text = "Відключено від сервера.";
                lblTurnIndicator.Text = "";
                opponentBoardControl.IsInteractive = false;
            }

            _isGameRunning = false;
            _isReadySent = false;
            btnConnect.Enabled = true;
            lblLobbyStatus.Visible = false;
            progressBarLobby.Visible = false;
            btnCancelSearch.Visible = false;
        }

        private void OnError(string error)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnError(error)));
                return;
            }

            Log($"[Помилка] {error}", Color.Red);
        }

        private void OnMessageReceived(NetworkMessage message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnMessageReceived(message)));
                return;
            }

            switch (message.Type)
            {
                case MessageTypes.LobbyStatus:
                    var lobbyDto = JsonSerializer.Deserialize<LobbyStatusDto>(message.Payload);
                    if (lobbyDto != null)
                    {
                        lblLobbyStatus.Text = lobbyDto.Message;
                    }
                    break;

                case MessageTypes.MatchFound:
                    var matchDto = JsonSerializer.Deserialize<MatchFoundDto>(message.Payload);
                    SwitchToGameScreen(matchDto?.OpponentName ?? "Суперник");
                    break;

                case MessageTypes.GameStart:
                    var startDto = JsonSerializer.Deserialize<GameStartDto>(message.Payload);
                    if (startDto != null)
                    {
                        HandleGameStart(startDto);
                    }
                    break;

                case MessageTypes.TurnChanged:
                    var turnDto = JsonSerializer.Deserialize<TurnChangedDto>(message.Payload);
                    if (turnDto != null)
                    {
                        SetTurn(turnDto.IsMyTurn);
                    }
                    break;

                case MessageTypes.ShotResult:
                    var shotDto = JsonSerializer.Deserialize<ShotResultDto>(message.Payload);
                    if (shotDto != null)
                    {
                        HandleShotResult(shotDto);
                    }
                    break;

                case MessageTypes.GameOver:
                    var overDto = JsonSerializer.Deserialize<GameOverDto>(message.Payload);
                    if (overDto != null)
                    {
                        HandleGameOver(overDto);
                    }
                    break;

                case MessageTypes.Error:
                    Log($"[Сервер]: {message.Payload}", Color.OrangeRed);
                    MessageBox.Show(message.Payload, "Повідомлення від сервера", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void SwitchToGameScreen(string opponentName)
        {
            pnlLobby.Visible = false;
            pnlGame.Visible = true;

            opponentBoardControl.Clear();
            opponentBoardControl.Title = $"ПОЛЕ СУПЕРНИКА ({opponentName})";
            opponentBoardControl.IsInteractive = false;

            _isReadySent = false;
            _isGameRunning = false;

            SetupPlacementMode(true);
            lblTurnIndicator.Text = "Очікування готовності...";
            lblTurnIndicator.ForeColor = Color.FromArgb(255, 215, 0);

            txtLog.Clear();
            Log($"Знайдено суперника: {opponentName}!", Color.FromArgb(100, 255, 100));
        }

        private void BtnAutoPlace_Click(object? sender, EventArgs e)
        {
            if (_isReadySent) return;
            GenerateAndDisplayFleet();
            Log("Випадкова розстановка кораблів сгенерована.", Color.LightBlue);
        }

        private async void BtnReady_Click(object? sender, EventArgs e)
        {
            if (_isReadySent) return;

            if (_myFleet.Count != 10)
            {
                MessageBox.Show("Необхідно розставити всі 10 кораблів!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isReadySent = true;
            SetupPlacementMode(false);
            btnReady.Enabled = false;
            lblGameStatus.Text = "Ви готові! Очікування готовності суперника...";
            lblGameStatus.ForeColor = Color.FromArgb(255, 200, 100);

            await _client.SendAsync(MessageTypes.SetReady, _myFleet);
        }

        private void HandleGameStart(GameStartDto dto)
        {
            _isGameRunning = true;
            SetupPlacementMode(false);
            Log("Всі гравці готові! Бій розпочався!", Color.Gold);
            SetTurn(dto.IsMyTurn);
        }

        private void SetTurn(bool isMyTurn)
        {
            _isMyTurn = isMyTurn;
            opponentBoardControl.IsInteractive = isMyTurn;

            if (isMyTurn)
            {
                lblTurnIndicator.Text = "⚔️ ВАШ ХІД! Стріляйте";
                lblTurnIndicator.ForeColor = Color.FromArgb(100, 255, 100);
                lblGameStatus.Text = "Ваш хід: оберіть клітинку на полі суперника для пострілу.";
                lblGameStatus.ForeColor = Color.FromArgb(120, 255, 120);
            }
            else
            {
                lblTurnIndicator.Text = "⏳ Хід суперника...";
                lblTurnIndicator.ForeColor = Color.FromArgb(255, 180, 80);
                lblGameStatus.Text = "Суперник здійснює постріл. Зачекайте...";
                lblGameStatus.ForeColor = Color.FromArgb(255, 200, 120);
            }
        }

        private async void OnOpponentCellClicked(int x, int y)
        {
            if (!_isGameRunning || !_isMyTurn) return;

            var curState = opponentBoardControl.Cells[x, y];
            if (curState == ClientCellState.Miss || curState == ClientCellState.Hit || curState == ClientCellState.Killed)
            {
                return;
            }

            opponentBoardControl.IsInteractive = false;
            await _client.SendAsync(MessageTypes.Shoot, new ShootRequestDto { X = x, Y = y });
        }

        private void HandleShotResult(ShotResultDto dto)
        {
            char colChar = (char)('А' + dto.X);
            string coordText = $"{colChar}{dto.Y + 1}";

            BoardControl targetBoard = dto.IsAttacker ? opponentBoardControl : myBoardControl;
            string shooterName = dto.IsAttacker ? "Ви" : "Суперник";

            if (dto.Outcome == ShotOutcome.Miss)
            {
                targetBoard.SetCellState(dto.X, dto.Y, ClientCellState.Miss);
                Log($"{shooterName} вистрілив по {coordText} — МИМО!", Color.FromArgb(180, 200, 230));
            }
            else if (dto.Outcome == ShotOutcome.Hit)
            {
                targetBoard.SetCellState(dto.X, dto.Y, ClientCellState.Hit);
                Log($"{shooterName} вистрілив по {coordText} — ПОРАНЕНО! (Додатковий хід)", Color.FromArgb(255, 120, 100));
            }
            else if (dto.Outcome == ShotOutcome.Kill)
            {
                targetBoard.SetCellState(dto.X, dto.Y, ClientCellState.Killed);

                if (dto.KilledShipCells != null)
                {
                    foreach (var c in dto.KilledShipCells)
                    {
                        targetBoard.SetCellState(c.X, c.Y, ClientCellState.Killed);
                    }
                }

                if (dto.SurroundingMissCells != null)
                {
                    foreach (var c in dto.SurroundingMissCells)
                    {
                        targetBoard.SetCellState(c.X, c.Y, ClientCellState.Miss);
                    }
                }

                Log($"{shooterName} вистрілив по {coordText} — УБИТО! Корабель потоплено! (Додатковий хід)", Color.FromArgb(255, 60, 60));
            }
        }

        private void HandleGameOver(GameOverDto dto)
        {
            _isGameRunning = false;
            opponentBoardControl.IsInteractive = false;

            if (dto.IsWinner)
            {
                lblTurnIndicator.Text = "🏆 ПЕРЕМОГА!";
                lblTurnIndicator.ForeColor = Color.FromArgb(255, 215, 0);
                lblGameStatus.Text = $"Вітаємо! Ви перемогли! Причина: {dto.Reason}";
                Log($"[КІНЕЦЬ ГРИ] Перемога! {dto.Reason}", Color.Gold);
                MessageBox.Show($"Вітаємо з перемогою!\n{dto.Reason}", "Перемога!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblTurnIndicator.Text = "💀 ПОРАЗКА";
                lblTurnIndicator.ForeColor = Color.FromArgb(255, 80, 80);
                lblGameStatus.Text = $"Гру завершено. Поразка. Причина: {dto.Reason}";
                Log($"[КІНЕЦЬ ГРИ] Поразка. {dto.Reason}", Color.IndianRed);
                MessageBox.Show($"На жаль, ви програли.\n{dto.Reason}", "Поразка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnSurrender_Click(object? sender, EventArgs e)
        {
            var res = MessageBox.Show("Ви впевнені, що хочете здатися та покинути гру?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                await _client.SendAsync(MessageTypes.LeaveGame);
                _client.Disconnect();

                pnlGame.Visible = false;
                pnlLobby.Visible = true;
                btnConnect.Enabled = true;
                lblLobbyStatus.Visible = false;
                progressBarLobby.Visible = false;
                btnCancelSearch.Visible = false;
            }
        }

        private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_client.IsConnected)
            {
                try
                {
                    await _client.SendAsync(MessageTypes.LeaveGame);
                }
                catch { }
                _client.Disconnect();
            }
        }
    }
}
