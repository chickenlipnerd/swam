using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for swim
/// </summary>
public class Swim
{
	public Swim()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int swim_id { get; set; }
    [Required]
    public string swim_title { get; set; }
    public string swim_type_name { get; set; }
}