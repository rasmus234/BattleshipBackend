
using System.Diagnostics;

using Microsoft.AspNetCore.SignalR;

namespace BattleshipBackend.Hubs;

public class BattleshipHub : Hub
{

    static Dictionary<string, string> _users = new();
    static Dictionary<string, GameRoom> _games = new();

    /// <summary>
    /// Checks against the user dictionary to see if the username is already in use.
    /// </summary>
    /// <param name="username"> The username the user wants to use. </param>
    /// <returns></returns>
    public bool Register(string username)
    {
        Debug.WriteLine(Context.ConnectionId + ": Requested the username '" + username + "'");
        
        //Checks if the username fulfills all rules
        bool AllowedUserName()
        {
            if (username == null || username.Length < 1)
            {
                return false;
            }

            return true;
        }

        bool UsernameExists()
        {
            return _users.Any(user => user.Key.Equals(username));
        }

        //if username exists generate a new one based on it
        if (!UsernameExists() && AllowedUserName())
        {
            _users.Add(username, Context.ConnectionId);
            return true;
        }

        return false;
    }

    public bool Shoot(int[,] shotCoordinates)
    {
        throw new NotImplementedException();
    }

    public bool YouSunkMyBattleShip()
    {
        throw new NotImplementedException();
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
        _games[roomName].GameID = roomName;

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

    public GameRoom[] GetGameRooms()
    {


        return _games.Values.ToArray();
    }

    public override Task OnConnectedAsync()
    {
        Debug.WriteLine("User has connected with ID: " + Context.ConnectionId);
        
        return base.OnConnectedAsync();
    }
}

public class GameRoom
{
    public string GameID { get; set; }
    public string PlayerOne { get; set; }
    public string PlayerTwo { get; set; }
}