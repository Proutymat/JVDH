using System;
using System.Collections.Generic;

[Serializable]
public class ProgressionData
{
    public int currentVideo = 0;
    
    // SUCCESS
    public List<bool> success =  new List<bool>(new []{false,false,false,false,false,false,false,false,false});
}
