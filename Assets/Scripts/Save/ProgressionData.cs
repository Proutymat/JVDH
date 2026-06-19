using System;
using System.Collections.Generic;

[Serializable]
public class ProgressionData
{
    public int currentVideo = 0;
    
    // SUCCESS
    public List<bool> successUnlocked =  new List<bool>(new []{false,false,false,false,false,false,false,false,false});
    public List<int> nbTimeViewed = new List<int>(new int[180]);
    public int nbDeath = 0;
    public int nbGameOver = 0;
}
