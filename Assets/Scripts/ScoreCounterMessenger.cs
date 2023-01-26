using Unity.Entities;
using UnityEngine;
using Alteruna;
using Unity.Mathematics;
using UnityEngine.UI;

public struct ScoreMessage : IMessage
{
    public bool Team;

    public ScoreMessage(bool team) { Team = team; }

    public void Read(Reader reader, int LOD)
    {
        Team = reader.ReadBool();
    }

    public void Write(Writer writer, int LOD)
    {
        writer.Write(Team);
    }
}

public class ScoreCounterMessenger : Messenger<ScoreMessage>, IComponentData
{
    public Text RedScoreText;
    public Text BlueScoreText;

    private uint redScore = 0;
    private uint blueScore = 0;
    private void Awake()
    {
        Manager.CreateSingleton(this);
    }

    public override void OnReceive(int user, ScoreMessage data)
    {
        //RED TEAM
        if (data.Team == false)
        {
            redScore++;
            if (RedScoreText)
                RedScoreText.text = redScore.ToString();
        }
        //BLUE TEAM
        else
        {
            blueScore++;
            if (BlueScoreText)
                BlueScoreText.text = blueScore.ToString();
        }
    }
}