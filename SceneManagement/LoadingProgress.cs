using System;

namespace Assets.Scripts.Framework.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;
        private const float k_Ratio = 0.9f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / k_Ratio);
        }
    }
}
