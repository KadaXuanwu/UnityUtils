public class JumpState : IResettableState {
    public bool ConsumedJump;

    public void Reset() {
        ConsumedJump = false;
    }
}