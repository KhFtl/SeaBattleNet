namespace SeaBattleNet
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pnlLobby = new Panel();
            lblTitle = new Label();
            lblIp = new Label();
            txtIp = new TextBox();
            lblPort = new Label();
            txtPort = new TextBox();
            lblName = new Label();
            txtName = new TextBox();
            btnConnect = new Button();
            lblLobbyStatus = new Label();
            progressBarLobby = new ProgressBar();
            btnCancelSearch = new Button();
            pnlGame = new Panel();
            pnlTopStatus = new Panel();
            lblTurnIndicator = new Label();
            lblGameStatus = new Label();
            myBoardControl = new BoardControl();
            opponentBoardControl = new BoardControl();
            pnlGameControls = new Panel();
            btnShip4 = new Button();
            btnShip3 = new Button();
            btnShip2 = new Button();
            btnShip1 = new Button();
            btnRotate = new Button();
            btnClearBoard = new Button();
            btnAutoPlace = new Button();
            btnReady = new Button();
            btnSurrender = new Button();
            lblPlacementTip = new Label();
            txtLog = new RichTextBox();
            pnlLobby.SuspendLayout();
            pnlGame.SuspendLayout();
            pnlTopStatus.SuspendLayout();
            pnlGameControls.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLobby
            // 
            pnlLobby.BackColor = Color.FromArgb(18, 24, 38);
            pnlLobby.Controls.Add(lblTitle);
            pnlLobby.Controls.Add(lblIp);
            pnlLobby.Controls.Add(txtIp);
            pnlLobby.Controls.Add(lblPort);
            pnlLobby.Controls.Add(txtPort);
            pnlLobby.Controls.Add(lblName);
            pnlLobby.Controls.Add(txtName);
            pnlLobby.Controls.Add(btnConnect);
            pnlLobby.Controls.Add(lblLobbyStatus);
            pnlLobby.Controls.Add(progressBarLobby);
            pnlLobby.Controls.Add(btnCancelSearch);
            pnlLobby.Dock = DockStyle.Fill;
            pnlLobby.Location = new Point(0, 0);
            pnlLobby.Name = "pnlLobby";
            pnlLobby.Size = new Size(1040, 700);
            pnlLobby.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.None;
            lblTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(100, 180, 255);
            lblTitle.Location = new Point(270, 70);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(500, 60);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "МОРСЬКИЙ БІЙ ОНЛАЙН";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblIp
            // 
            lblIp.Anchor = AnchorStyles.None;
            lblIp.Font = new Font("Segoe UI", 11F);
            lblIp.ForeColor = Color.LightGray;
            lblIp.Location = new Point(350, 160);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(120, 25);
            lblIp.TabIndex = 1;
            lblIp.Text = "IP-адреса:";
            // 
            // txtIp
            // 
            txtIp.Anchor = AnchorStyles.None;
            txtIp.BackColor = Color.FromArgb(30, 40, 60);
            txtIp.BorderStyle = BorderStyle.FixedSingle;
            txtIp.Font = new Font("Segoe UI", 11F);
            txtIp.ForeColor = Color.White;
            txtIp.Location = new Point(350, 188);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(220, 27);
            txtIp.TabIndex = 2;
            txtIp.Text = "127.0.0.1";
            // 
            // lblPort
            // 
            lblPort.Anchor = AnchorStyles.None;
            lblPort.Font = new Font("Segoe UI", 11F);
            lblPort.ForeColor = Color.LightGray;
            lblPort.Location = new Point(585, 160);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(100, 25);
            lblPort.TabIndex = 3;
            lblPort.Text = "Порт:";
            // 
            // txtPort
            // 
            txtPort.Anchor = AnchorStyles.None;
            txtPort.BackColor = Color.FromArgb(30, 40, 60);
            txtPort.BorderStyle = BorderStyle.FixedSingle;
            txtPort.Font = new Font("Segoe UI", 11F);
            txtPort.ForeColor = Color.White;
            txtPort.Location = new Point(585, 188);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(105, 27);
            txtPort.TabIndex = 4;
            txtPort.Text = "8888";
            // 
            // lblName
            // 
            lblName.Anchor = AnchorStyles.None;
            lblName.Font = new Font("Segoe UI", 11F);
            lblName.ForeColor = Color.LightGray;
            lblName.Location = new Point(350, 235);
            lblName.Name = "lblName";
            lblName.Size = new Size(120, 25);
            lblName.TabIndex = 5;
            lblName.Text = "Нікнейм:";
            // 
            // txtName
            // 
            txtName.Anchor = AnchorStyles.None;
            txtName.BackColor = Color.FromArgb(30, 40, 60);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Font = new Font("Segoe UI", 11F);
            txtName.ForeColor = Color.White;
            txtName.Location = new Point(350, 263);
            txtName.Name = "txtName";
            txtName.Size = new Size(340, 27);
            txtName.TabIndex = 6;
            txtName.Text = "Гравець";
            // 
            // btnConnect
            // 
            btnConnect.Anchor = AnchorStyles.None;
            btnConnect.BackColor = Color.FromArgb(0, 120, 215);
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(350, 315);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(340, 45);
            btnConnect.TabIndex = 7;
            btnConnect.Text = "Знайти гру";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += BtnConnect_Click;
            // 
            // lblLobbyStatus
            // 
            lblLobbyStatus.Anchor = AnchorStyles.None;
            lblLobbyStatus.Font = new Font("Segoe UI", 11F);
            lblLobbyStatus.ForeColor = Color.FromArgb(255, 200, 100);
            lblLobbyStatus.Location = new Point(270, 380);
            lblLobbyStatus.Name = "lblLobbyStatus";
            lblLobbyStatus.Size = new Size(500, 30);
            lblLobbyStatus.TabIndex = 8;
            lblLobbyStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblLobbyStatus.Visible = false;
            // 
            // progressBarLobby
            // 
            progressBarLobby.Anchor = AnchorStyles.None;
            progressBarLobby.Location = new Point(350, 415);
            progressBarLobby.MarqueeAnimationSpeed = 30;
            progressBarLobby.Name = "progressBarLobby";
            progressBarLobby.Size = new Size(340, 15);
            progressBarLobby.Style = ProgressBarStyle.Marquee;
            progressBarLobby.TabIndex = 9;
            progressBarLobby.Visible = false;
            // 
            // btnCancelSearch
            // 
            btnCancelSearch.Anchor = AnchorStyles.None;
            btnCancelSearch.BackColor = Color.FromArgb(180, 50, 50);
            btnCancelSearch.FlatAppearance.BorderSize = 0;
            btnCancelSearch.FlatStyle = FlatStyle.Flat;
            btnCancelSearch.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancelSearch.ForeColor = Color.White;
            btnCancelSearch.Location = new Point(440, 445);
            btnCancelSearch.Name = "btnCancelSearch";
            btnCancelSearch.Size = new Size(160, 35);
            btnCancelSearch.TabIndex = 10;
            btnCancelSearch.Text = "Скасувати";
            btnCancelSearch.UseVisualStyleBackColor = false;
            btnCancelSearch.Visible = false;
            btnCancelSearch.Click += BtnCancelSearch_Click;
            // 
            // pnlGame
            // 
            pnlGame.BackColor = Color.FromArgb(14, 20, 32);
            pnlGame.Controls.Add(pnlTopStatus);
            pnlGame.Controls.Add(myBoardControl);
            pnlGame.Controls.Add(opponentBoardControl);
            pnlGame.Controls.Add(pnlGameControls);
            pnlGame.Dock = DockStyle.Fill;
            pnlGame.Location = new Point(0, 0);
            pnlGame.Name = "pnlGame";
            pnlGame.Size = new Size(1040, 700);
            pnlGame.TabIndex = 1;
            pnlGame.Visible = false;
            // 
            // pnlTopStatus
            // 
            pnlTopStatus.BackColor = Color.FromArgb(20, 28, 44);
            pnlTopStatus.Controls.Add(lblTurnIndicator);
            pnlTopStatus.Controls.Add(lblGameStatus);
            pnlTopStatus.Dock = DockStyle.Top;
            pnlTopStatus.Location = new Point(0, 0);
            pnlTopStatus.Name = "pnlTopStatus";
            pnlTopStatus.Size = new Size(1040, 50);
            pnlTopStatus.TabIndex = 0;
            // 
            // lblTurnIndicator
            // 
            lblTurnIndicator.Dock = DockStyle.Right;
            lblTurnIndicator.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTurnIndicator.ForeColor = Color.FromArgb(255, 215, 0);
            lblTurnIndicator.Location = new Point(620, 0);
            lblTurnIndicator.Name = "lblTurnIndicator";
            lblTurnIndicator.Padding = new Padding(0, 0, 15, 0);
            lblTurnIndicator.Size = new Size(420, 50);
            lblTurnIndicator.TabIndex = 1;
            lblTurnIndicator.Text = "Очікування готовності...";
            lblTurnIndicator.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblGameStatus
            // 
            lblGameStatus.AutoEllipsis = true;
            lblGameStatus.Dock = DockStyle.Fill;
            lblGameStatus.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblGameStatus.ForeColor = Color.FromArgb(120, 210, 255);
            lblGameStatus.Location = new Point(0, 0);
            lblGameStatus.Name = "lblGameStatus";
            lblGameStatus.Padding = new Padding(15, 0, 0, 0);
            lblGameStatus.Size = new Size(1040, 50);
            lblGameStatus.TabIndex = 0;
            lblGameStatus.Text = "Суперника знайдено! Розставте кораблі та натисніть «Готовий до бою».";
            lblGameStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // myBoardControl
            // 
            myBoardControl.Anchor = AnchorStyles.None;
            myBoardControl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            myBoardControl.IsInteractive = true;
            myBoardControl.IsOpponentBoard = false;
            myBoardControl.IsPlacementMode = false;
            myBoardControl.Location = new Point(40, 60);
            myBoardControl.Name = "myBoardControl";
            myBoardControl.PlacementIsHorizontal = true;
            myBoardControl.PlacementIsValid = false;
            myBoardControl.PlacementShipSize = 4;
            myBoardControl.Size = new Size(460, 440);
            myBoardControl.TabIndex = 1;
            myBoardControl.Title = "МОЄ ПОЛЕ";
            // 
            // opponentBoardControl
            // 
            opponentBoardControl.Anchor = AnchorStyles.None;
            opponentBoardControl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            opponentBoardControl.IsInteractive = false;
            opponentBoardControl.IsOpponentBoard = true;
            opponentBoardControl.IsPlacementMode = false;
            opponentBoardControl.Location = new Point(540, 60);
            opponentBoardControl.Name = "opponentBoardControl";
            opponentBoardControl.PlacementIsHorizontal = true;
            opponentBoardControl.PlacementIsValid = false;
            opponentBoardControl.PlacementShipSize = 4;
            opponentBoardControl.Size = new Size(460, 440);
            opponentBoardControl.TabIndex = 2;
            opponentBoardControl.Title = "ПОЛЕ СУПЕРНИКА";
            // 
            // pnlGameControls
            // 
            pnlGameControls.BackColor = Color.FromArgb(16, 22, 36);
            pnlGameControls.Controls.Add(btnShip4);
            pnlGameControls.Controls.Add(btnShip3);
            pnlGameControls.Controls.Add(btnShip2);
            pnlGameControls.Controls.Add(btnShip1);
            pnlGameControls.Controls.Add(btnRotate);
            pnlGameControls.Controls.Add(btnClearBoard);
            pnlGameControls.Controls.Add(btnAutoPlace);
            pnlGameControls.Controls.Add(btnReady);
            pnlGameControls.Controls.Add(btnSurrender);
            pnlGameControls.Controls.Add(lblPlacementTip);
            pnlGameControls.Controls.Add(txtLog);
            pnlGameControls.Dock = DockStyle.Bottom;
            pnlGameControls.Location = new Point(0, 510);
            pnlGameControls.Name = "pnlGameControls";
            pnlGameControls.Size = new Size(1040, 190);
            pnlGameControls.TabIndex = 3;
            // 
            // btnShip4
            // 
            btnShip4.BackColor = Color.FromArgb(40, 70, 110);
            btnShip4.FlatStyle = FlatStyle.Flat;
            btnShip4.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnShip4.ForeColor = Color.White;
            btnShip4.Location = new Point(40, 10);
            btnShip4.Name = "btnShip4";
            btnShip4.Size = new Size(105, 32);
            btnShip4.TabIndex = 0;
            btnShip4.Text = "4-пал (1)";
            btnShip4.UseVisualStyleBackColor = false;
            btnShip4.Click += BtnShip_Click;
            // 
            // btnShip3
            // 
            btnShip3.BackColor = Color.FromArgb(40, 70, 110);
            btnShip3.FlatStyle = FlatStyle.Flat;
            btnShip3.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnShip3.ForeColor = Color.White;
            btnShip3.Location = new Point(150, 10);
            btnShip3.Name = "btnShip3";
            btnShip3.Size = new Size(105, 32);
            btnShip3.TabIndex = 1;
            btnShip3.Text = "3-пал (2)";
            btnShip3.UseVisualStyleBackColor = false;
            btnShip3.Click += BtnShip_Click;
            // 
            // btnShip2
            // 
            btnShip2.BackColor = Color.FromArgb(40, 70, 110);
            btnShip2.FlatStyle = FlatStyle.Flat;
            btnShip2.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnShip2.ForeColor = Color.White;
            btnShip2.Location = new Point(260, 10);
            btnShip2.Name = "btnShip2";
            btnShip2.Size = new Size(105, 32);
            btnShip2.TabIndex = 2;
            btnShip2.Text = "2-пал (3)";
            btnShip2.UseVisualStyleBackColor = false;
            btnShip2.Click += BtnShip_Click;
            // 
            // btnShip1
            // 
            btnShip1.BackColor = Color.FromArgb(40, 70, 110);
            btnShip1.FlatStyle = FlatStyle.Flat;
            btnShip1.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnShip1.ForeColor = Color.White;
            btnShip1.Location = new Point(370, 10);
            btnShip1.Name = "btnShip1";
            btnShip1.Size = new Size(105, 32);
            btnShip1.TabIndex = 3;
            btnShip1.Text = "1-пал (4)";
            btnShip1.UseVisualStyleBackColor = false;
            btnShip1.Click += BtnShip_Click;
            // 
            // btnRotate
            // 
            btnRotate.BackColor = Color.FromArgb(45, 80, 120);
            btnRotate.FlatAppearance.BorderSize = 0;
            btnRotate.FlatStyle = FlatStyle.Flat;
            btnRotate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRotate.ForeColor = Color.White;
            btnRotate.Location = new Point(40, 48);
            btnRotate.Name = "btnRotate";
            btnRotate.Size = new Size(215, 32);
            btnRotate.TabIndex = 4;
            btnRotate.Text = "🔄 Орієнтація: Горизонт.";
            btnRotate.UseVisualStyleBackColor = false;
            btnRotate.Click += BtnRotate_Click;
            // 
            // btnClearBoard
            // 
            btnClearBoard.BackColor = Color.FromArgb(90, 45, 60);
            btnClearBoard.FlatAppearance.BorderSize = 0;
            btnClearBoard.FlatStyle = FlatStyle.Flat;
            btnClearBoard.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnClearBoard.ForeColor = Color.White;
            btnClearBoard.Location = new Point(260, 48);
            btnClearBoard.Name = "btnClearBoard";
            btnClearBoard.Size = new Size(105, 32);
            btnClearBoard.TabIndex = 5;
            btnClearBoard.Text = "🗑️ Очистити";
            btnClearBoard.UseVisualStyleBackColor = false;
            btnClearBoard.Click += BtnClearBoard_Click;
            // 
            // btnAutoPlace
            // 
            btnAutoPlace.BackColor = Color.FromArgb(45, 110, 180);
            btnAutoPlace.FlatAppearance.BorderSize = 0;
            btnAutoPlace.FlatStyle = FlatStyle.Flat;
            btnAutoPlace.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAutoPlace.ForeColor = Color.White;
            btnAutoPlace.Location = new Point(370, 48);
            btnAutoPlace.Name = "btnAutoPlace";
            btnAutoPlace.Size = new Size(105, 32);
            btnAutoPlace.TabIndex = 6;
            btnAutoPlace.Text = "🎲 Авто";
            btnAutoPlace.UseVisualStyleBackColor = false;
            btnAutoPlace.Click += BtnAutoPlace_Click;
            // 
            // btnReady
            // 
            btnReady.BackColor = Color.FromArgb(46, 125, 50);
            btnReady.FlatAppearance.BorderSize = 0;
            btnReady.FlatStyle = FlatStyle.Flat;
            btnReady.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnReady.ForeColor = Color.White;
            btnReady.Location = new Point(40, 108);
            btnReady.Name = "btnReady";
            btnReady.Size = new Size(260, 38);
            btnReady.TabIndex = 8;
            btnReady.Text = "⚔️ ГОТОВИЙ ДО БОЮ";
            btnReady.UseVisualStyleBackColor = false;
            btnReady.Click += BtnReady_Click;
            // 
            // btnSurrender
            // 
            btnSurrender.BackColor = Color.FromArgb(160, 45, 45);
            btnSurrender.FlatAppearance.BorderSize = 0;
            btnSurrender.FlatStyle = FlatStyle.Flat;
            btnSurrender.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSurrender.ForeColor = Color.White;
            btnSurrender.Location = new Point(310, 108);
            btnSurrender.Name = "btnSurrender";
            btnSurrender.Size = new Size(165, 38);
            btnSurrender.TabIndex = 9;
            btnSurrender.Text = "🏳️ Здатися";
            btnSurrender.UseVisualStyleBackColor = false;
            btnSurrender.Click += BtnSurrender_Click;
            // 
            // lblPlacementTip
            // 
            lblPlacementTip.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
            lblPlacementTip.ForeColor = Color.FromArgb(180, 200, 220);
            lblPlacementTip.Location = new Point(40, 84);
            lblPlacementTip.Name = "lblPlacementTip";
            lblPlacementTip.Size = new Size(435, 20);
            lblPlacementTip.TabIndex = 7;
            lblPlacementTip.Text = "💡 ЛКМ — поставити / зняти корабель. ПКМ — повернути.";
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            txtLog.BackColor = Color.FromArgb(22, 30, 46);
            txtLog.BorderStyle = BorderStyle.None;
            txtLog.Font = new Font("Segoe UI", 9.5F);
            txtLog.ForeColor = Color.FromArgb(200, 220, 240);
            txtLog.Location = new Point(540, 10);
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtLog.Size = new Size(460, 168);
            txtLog.TabIndex = 10;
            txtLog.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1040, 700);
            Controls.Add(pnlGame);
            Controls.Add(pnlLobby);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1040, 700);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Морський бій онлайн";
            FormClosing += MainForm_FormClosing;
            pnlLobby.ResumeLayout(false);
            pnlLobby.PerformLayout();
            pnlGame.ResumeLayout(false);
            pnlTopStatus.ResumeLayout(false);
            pnlGameControls.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlLobby;
        private Label lblTitle;
        private Label lblIp;
        private TextBox txtIp;
        private Label lblPort;
        private TextBox txtPort;
        private Label lblName;
        private TextBox txtName;
        private Button btnConnect;
        private Label lblLobbyStatus;
        private ProgressBar progressBarLobby;
        private Button btnCancelSearch;

        private Panel pnlGame;
        private Panel pnlTopStatus;
        private Label lblGameStatus;
        private Label lblTurnIndicator;
        private BoardControl myBoardControl;
        private BoardControl opponentBoardControl;
        private Panel pnlGameControls;
        private Button btnShip4;
        private Button btnShip3;
        private Button btnShip2;
        private Button btnShip1;
        private Button btnRotate;
        private Button btnClearBoard;
        private Button btnAutoPlace;
        private Button btnReady;
        private Button btnSurrender;
        private Label lblPlacementTip;
        private RichTextBox txtLog;
    }
}
