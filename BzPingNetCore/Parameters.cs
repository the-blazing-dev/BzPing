namespace BzPing;

public class Parameters
{
    public required List<string> Hosts { get; set; }
    public bool LiveMode { get; set; }
    public bool? Spacer { get; set; }
    
    public static Parameters Parse(string[] args)
    {
        var result = new Parameters
        {
            Hosts = args.ToList()
        };

        if (result.Hosts.Remove("--live"))
        {
            result.LiveMode = true;
            result.Spacer = false;
        }

        if (result.Spacer == null)
        {
            result.Spacer = result.Hosts.Count > 3;
        }

        return result;
    }
}