namespace DefaultNamespace;
	
public class Country
{
    public string? Id { get; set ; }
    public string? Name { get; set ; }
	public ICollection<Genre> Genres { get; set; } = new List<Genre>();
	
}