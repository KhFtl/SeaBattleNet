using System.Text.Json.Serialization;

namespace SeaBattle.Protocol
{
    public class NetworkMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("payload")]
        public string Payload { get; set; } = string.Empty;

        public NetworkMessage() { }

        public NetworkMessage(string type, string payload = "")
        {
            Type = type;
            Payload = payload;
        }
    }

    public static class MessageTypes
    {
        // Client -> Server
        public const string JoinLobby = "JOIN_LOBBY";
        public const string SetReady = "SET_READY"; // Payload: List<ShipPlacementDto>
        public const string Shoot = "SHOOT"; // Payload: ShootRequestDto (X, Y)
        public const string LeaveGame = "LEAVE_GAME";

        // Server -> Client
        public const string LobbyStatus = "LOBBY_STATUS"; // Payload: LobbyStatusDto
        public const string MatchFound = "MATCH_FOUND"; // Payload: MatchFoundDto (RoomId, OpponentName)
        public const string GameStart = "GAME_START"; // Payload: GameStartDto (FirstTurn: bool/who)
        public const string TurnChanged = "TURN_CHANGED"; // Payload: TurnChangedDto (IsMyTurn)
        public const string ShotResult = "SHOT_RESULT"; // Payload: ShotResultDto
        public const string GameOver = "GAME_OVER"; // Payload: GameOverDto (IsWinner, Reason)
        public const string Error = "ERROR"; // Payload: Error message
    }

    public class ShipPlacementDto
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("isHorizontal")]
        public bool IsHorizontal { get; set; }
    }

    public class ShootRequestDto
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }
    }

    public enum ShotOutcome
    {
        Miss = 0,
        Hit = 1,
        Kill = 2
    }

    public class CellCoordDto
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }
    }

    public class ShotResultDto
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("isAttacker")]
        public bool IsAttacker { get; set; }

        [JsonPropertyName("outcome")]
        public ShotOutcome Outcome { get; set; }

        [JsonPropertyName("killedShipCells")]
        public List<CellCoordDto>? KilledShipCells { get; set; }

        [JsonPropertyName("surroundingMissCells")]
        public List<CellCoordDto>? SurroundingMissCells { get; set; }
    }

    public class LobbyStatusDto
    {
        [JsonPropertyName("playersInQueue")]
        public int PlayersInQueue { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class MatchFoundDto
    {
        [JsonPropertyName("roomId")]
        public string RoomId { get; set; } = string.Empty;

        [JsonPropertyName("opponentName")]
        public string OpponentName { get; set; } = string.Empty;
    }

    public class GameStartDto
    {
        [JsonPropertyName("isMyTurn")]
        public bool IsMyTurn { get; set; }

        [JsonPropertyName("opponentName")]
        public string OpponentName { get; set; } = string.Empty;
    }

    public class TurnChangedDto
    {
        [JsonPropertyName("isMyTurn")]
        public bool IsMyTurn { get; set; }
    }

    public class GameOverDto
    {
        [JsonPropertyName("isWinner")]
        public bool IsWinner { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;
    }
}
