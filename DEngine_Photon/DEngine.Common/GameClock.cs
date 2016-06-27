using System.Diagnostics;

namespace DEngine.Common
{
    public class GameClock
    {
        private float baseTime;
        private float updateTime;
        private float deltaTime;

        public float TotalTime
        {
            get
            {
                return getRealTime() - baseTime;
            }
        }

        public float DeltaTime
        {
            get
            {
                return deltaTime;
            }
        }

        public GameClock()
        {
            Reset();
        }

        public void Reset()
        {
            baseTime = getRealTime();
            updateTime = baseTime;
            deltaTime = 0;
        }

        public void Tick()
        {
            float curTime = getRealTime();
            deltaTime = curTime - updateTime;
            updateTime = curTime;
        }

        private float getRealTime()
        {
            double realTime = (double)Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            return (float)realTime;
        }
    }
}
