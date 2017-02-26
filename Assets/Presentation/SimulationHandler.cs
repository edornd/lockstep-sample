using Game;
using System.Threading;
using UnityEngine;

public class SimulationHandler : MonoBehaviour {

    private Simulation game;
    private Thread gameThread;

	// Use this for initialization
	void Start () {
        BeginSimulation();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.S)) {
            StopSimulation();
        }
	}

    void OnApplicationQuit() {
        if (game.IsRunning())
            StopSimulation();
    }

    private void BeginSimulation() {
        game = Simulation.Instance;
        gameThread = new Thread(game.Run);
        gameThread.Start();
    }

    private void StopSimulation() {
        game.Stop();
    }
}
