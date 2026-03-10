public interface IState
{
    void Update(float deltaTime);
    //void FixedUpdate(float fixedDeltaTime);
    void OnEnter();
    void OnExit();

}