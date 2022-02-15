using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class UCI : MonoBehaviour
{
    Process myProcess;
    string lastInput = "";

    // Returns the move from a given FEN string format of the board. 
    string move(string pos)
    {
        string move = "";
        write($"position {pos}");
        write("go movetime 10");
        int moveIndex = lastInput.IndexOf("bestmove");
        if (moveIndex != -1)
        {
            move = lastInput.Substring(moveIndex + 9, 4);
            //UnityEngine.Debug.Log(move);
            // Will pass move to FEN string method here. 
        }
        return move;
    }

    void read(object sender, DataReceivedEventArgs x)
    {
        string text = x.Data;
        //UnityEngine.Debug.Log(x.Data);
        lastInput = text;
    }

    void write(string message)
    {
        myProcess.StandardInput.WriteLine(message);
        myProcess.StandardInput.Flush();
    }

    public UCI()
    {
        // The process was surprisingly tricky to set up, I ended up using asynchronous method as synchronous did not work for me.
        ProcessStartInfo si = new ProcessStartInfo()
        {
            FileName = Application.dataPath + "/Scripts/Chess/stockfish_14.1_win_x64_popcnt.exe",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        myProcess = new Process();
        myProcess.StartInfo = si;
        myProcess.OutputDataReceived += new DataReceivedEventHandler(read);

        myProcess.Start();
        myProcess.BeginErrorReadLine();
        myProcess.BeginOutputReadLine();

        // Sending commands to start. There are a variety of options we could send between "uci" and "isready" but for now just setting up conventionally.
        write("uci");
        write("isready");
        write("position startpos");
        /*
        // Loop is purely for debugging purposes.
        for (int i = 0; i < 10000; i++)
        {
            write("go movetime 10");
            int moveIndex = lastInput.IndexOf("bestmove");
            if (moveIndex != -1)
            {
                string move = lastInput.Substring(moveIndex + 9, 4);
                UnityEngine.Debug.Log(move);
                // when FEN method is implemented, we will call it here to get the position and set that before sending "go"
                // we will then send that to reanimate the board
            }
        }
        */
    }
}
