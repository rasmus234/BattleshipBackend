
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BattleshipBackend.Hubs;

public class BattleshipHub : Hub
{

    static Dictionary<string, string> _users = new();
    static Dictionary<string, GameRoom> _games = new();

    /// <summary>
    /// Checks against the user dictionary to see if the username is already in use.
    /// </summary>
    /// <param name="displayName"> The username the user wants to use. </param>
    /// <returns></returns>
    public bool Register(string displayName)
    {
        Debug.WriteLine(Context.ConnectionId + ": Requested the username '" + displayName + "'");
        
        //Checks if the username fulfills all rules
        bool AllowedUserName()
        {
            if (displayName == null || displayName.Length < 1)
            {
                return false;
            }

            return true;
        }

        bool UsernameExists()
        {
            return _users.Any(user => user.Key.Equals(displayName));
        }

        //if username doesnt exists generate a new one based on it
        if (!UsernameExists() && AllowedUserName())
        {
            _users.Add(Context.ConnectionId, displayName);
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
        _games[roomName].PlayerOne.ConnectionId = Context.ConnectionId;
        _games[roomName].PlayerOne.DisplayName = _users.First(user => user.Key.Equals(Context.ConnectionId)).Value;
        _games[roomName].GameID = roomName;

        Debug.WriteLine(Context.ConnectionId + ": Created a room with the name '" + roomName + "'");

        return true;
    }

    public bool JoinGameRoom(string roomName)
    {
        
        if (_games.TryGetValue(roomName, out var gameRoom) && NotFull())
        {
            gameRoom.PlayerTwo.ConnectionId = Context.ConnectionId;
            gameRoom.PlayerTwo.DisplayName = _users.First(user => user.Key.Equals(Context.ConnectionId)).Value;
            Debug.WriteLine(Context.ConnectionId + ": Joined the room with the name '" + roomName + "'");
            return true;
        }
        return false;


        bool NotFull()
        {
            return gameRoom.PlayerTwo.ConnectionId == null;
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
    public Player PlayerOne { get; set; } = new Player();
    public Player? PlayerTwo { get; set; } = new Player();
}

public class Player
{
    public string ConnectionId { get; set; }
    public string DisplayName { get; set; }
    public GameRoom? CurrentGame { get; set; }
}