namespace SuperSoccer.DomainLayer.Entities;

public class Team
{
    public Team(List<Player> players)
    {
        Players = players;
    }

    public List<Player> Players {get; set;}
    
    public Player GetGoalie()
    {
        ValidatePlayersPool();

        return Players.OrderByDescending(p => p.Height).First();
    }

    public List<Player> GetDefence(short defenceSize = 2)
    {
        ValidatePlayersPool(defenceSize);
        return Players.OrderByDescending(p => p.Weight).Take(defenceSize).ToList();
    }
    
    public List<Player> GetOffence(short offenceSize = 2)
    {
        ValidatePlayersPool(offenceSize);
        return Players.OrderBy(p => p.Height).Take(offenceSize).ToList();
    }
    
    private void ValidatePlayersPool(int minimumSize = 1)
    {
        if (Players == null)
        {
            throw new ArgumentNullException(nameof(Players), "Player list cannot be null.");
        }

        if (Players.Count >= minimumSize)
        {
            throw new InvalidOperationException("Player list is too small.");
        }
    }

    public static Team EmptyTeam()
    {
        return new Team(new List<Player>());
    }
}