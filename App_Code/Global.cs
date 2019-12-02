using System;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Contains my site's global variables.
/// </summary>
public static class Global
{
    /// <summary>
    /// Global variable storing important stuff.
    /// </summary>
    static string _bodyClass;

    /// <summary>
    /// Get or set the static important data.
    /// </summary>
    public static string BodyClass
    {
	    get
	    {
	        return _bodyClass;
	    }
	    set
	    {
	        _bodyClass = value;
	    }
    }
}