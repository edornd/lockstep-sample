using Game;
using System.Threading;
using UnityEngine;

public class SimulationWrapper : MonoBehaviour {

    public int frameLength = 50;
    public int maxFrames = 4;

    private Simulation game;
    private Thread gameThread;

	void Awake () {
        BeginSimulation();
	}

    void OnApplicationQuit() {
        if (game.IsRunning())
            StopSimulation();
    }

    private void BeginSimulation() {
        if (Simulation.Instance == null) {
            double delta = frameLength / 1000.0;
            Simulation.Create(delta, delta * 2, maxFrames);
        }
        game = Simulation.Instance;
        gameThread = new Thread(game.Run);
        gameThread.Start();
    }

    private void StopSimulation() {
        game.Stop();
    }
}
