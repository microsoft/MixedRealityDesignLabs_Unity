using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class DehydrationDeactivation : StateMachineBehaviour
    {
        /// <summary>
        /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        /// </summary>
        /// <param name="animator">Animator that triggered OnStateEnter.</param>
        /// <param name="stateInfo">Animator state info.</param>
        /// <param name="layerIndex">Layer index.</param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.transform.gameObject.SetActive(false);
        }
    }
}
