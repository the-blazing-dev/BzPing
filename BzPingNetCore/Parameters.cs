using System.Collections.Generic;
using System.Linq;

namespace BzPing;

public class Parameters
{
    public List<string> Hosts { get; set; } = new();
    public bool LiveMode { get; set; }
    public bool? Spacer { get; set; }
    
    public static Parameters Parse(string[] args)
    {
        var result = new Parameters();
        result.Hosts.AddRange(args);

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