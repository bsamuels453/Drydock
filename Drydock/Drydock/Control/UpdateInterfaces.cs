namespace Drydock.Control{
    internal interface IUpdatable{
        void Update(double timeDelta);
    }

    internal interface IInputUpdatable{
        void InputUpdate(ref ControlState state);
    }
}