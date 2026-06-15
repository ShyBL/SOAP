using System.Collections;
using UnityEngine;

namespace SOAP
{
    public class FloatChannelRaiserRegen : FloatChannelRaiser
    {
        private Coroutine regenRoutine;

        [SerializeField] private float regenInterval = 1f;

        private void Start()
        {
            StartRegeneration();
        }

        public void StartRegeneration()
        {
            if (regenRoutine != null)
                return;

            regenRoutine = StartCoroutine(RegenerationLoop());
            
            if (regenRoutine == null)
                regenRoutine = StartCoroutine(RegenerationLoop());
        }

        public void StopRegeneration()
        {
            if (regenRoutine != null)
            {
                StopCoroutine(regenRoutine);
                regenRoutine = null;
            }
        }

        private IEnumerator RegenerationLoop()
        {
            while (true)
            {
                if (floatVariable.currentValue >= floatVariable.maxValue)
                {
                    regenRoutine = null;
                    yield break;
                }

                yield return new WaitForSeconds(regenInterval);

                floatVariable.Add(changeAmount.variable.currentValue);

                channelToRaise.Raise(enterTopic, floatVariable.currentValue);
            }
        }
    }
}