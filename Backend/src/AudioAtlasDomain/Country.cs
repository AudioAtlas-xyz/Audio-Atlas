namespace Domain.Country;
	
public class Country
{
	public required string Id { get; set ; }
    public required string Name { get; set ; }
    public required ICollection<Genre> Genres { get; set; } = new List<Genre>();
}