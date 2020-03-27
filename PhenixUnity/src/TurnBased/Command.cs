namespace Phenix.Unity.TurnBased
{

    public interface ITurnBasedCommand
    {
        void OnStart();
        void OnUpdate();
        void OnEnd();
        bool Finished { get; set; }
    }
}