namespace Drydock.Control{
    internal interface ILogicUpdates{
        void UpdateLogic(double timeDelta);
    }

    internal interface IInputUpdates{
        void UpdateInput(ref InputState state);
    }
}