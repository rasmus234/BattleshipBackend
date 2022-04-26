using System.Diagnostics;
using System.Numerics;
using Microsoft.AspNetCore.SignalR;

namespace BattleshipBackend.Hubs;

public class BattleshipHub : Hub
{
    static readonly Dictionary<string, string> _users = new();
    static readonly Dictionary<string, GameRoom> _games = new();

    /// <summary>
    /// Checks against the user dictionary to see if the username is already in use.
    /// </summary>
    /// <param name="username"> The username the user wants to use. </param>
    /// <returns></returns>
    public async Task Register(string username)
    {
        Debug.WriteLine(Context.ConnectionId + ": Requested the username '" + username + "'");
        
        //Checks if the username fulfills all rules
        bool allowedUserName = !string.IsNullOrEmpty(username);
        bool usernameExists = _users.Any(user => user.Key.Equals(username));

        //if username exists generate a new one based on it
        if (!usernameExists && allowedUserName)
        {
            Console.Out.WriteLine("Username '" + username + "' is available.");
            _users.Add(username, Context.ConnectionId);
            Console.Out.WriteLine("sending shot");
            await SendShot(1,2);
        }
    }

    public async Task SendShot(int x, int y)
    {
        await Clients.All.SendAsync("ReceiveShot", x, y);
    }

    public async Task YouSunkMyBattleShip()
    {
        await Clients.Others.SendAsync("YouSunkOpponentsBattleShip");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roomName">Identifier for a game session.</param>
    /// <returns>True if the room was successfully created.</returns>
    public bool CreateGameRoom(string roomName)
    {
        if (_games.Any(game => game.Key.Equals(roomName))) return false;
        
        _games.Add(roomName, new GameRoom());
        _games[roomName].PlayerOne = Context.ConnectionId;

        Debug.WriteLine(Context.ConnectionId + ": Created a room with the name '" + roomName + "'");

        return true;
    }

    public void JoinGameRoom(string roomName)
    {
        Debug.WriteLine("Rooms:");
        
        foreach (var keyValuePair in _games)
        {
            Debug.WriteLine(keyValuePair.Key);
        }
        
        Debug.WriteLine("");

        if (_games.TryGetValue(roomName, out var gameRoom))
        {
            gameRoom.PlayerTwo = Context.ConnectionId;
            
            Debug.WriteLine(Context.ConnectionId + ": Joined the room with the name '" + roomName + "'");
        }
    }

    public override Task OnConnectedAsync()
    {
        Debug.WriteLine("User has connected with ID: " + Context.ConnectionId);
        
        return base.OnConnectedAsync();
    }
}

class GameRoom
{
    public string PlayerOne { get; set; }
    public string PlayerTwo { get; set; }
}