using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Drydock.Render{
    public class Renderer{
        private readonly Vector3 _camReference = new Vector3(0, 0, 1);

        private readonly EnvironmentBatch _environmentBatch;
        private readonly Matrix _projectionMatrix;
        private readonly TextBatch _textBatch;
        public float AspectRatio;
        public GraphicsDevice Device;
        public float ViewportPitch;
        public Vector3 ViewportPosition;
        public float ViewportYaw;

        public Renderer(GraphicsDevice device, ContentManager content){
            ScreenText.Init(content);
            Device = device;

            ViewportPitch = -0.47f;
            ViewportPosition = new Vector3(0, 10, 0);
            ViewportYaw = -6.9f;
            AspectRatio = Device.Viewport.Bounds.Width/(float) Device.Viewport.Bounds.Height;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView: 3.14f/4,
                aspectRatio: AspectRatio,
                nearPlaneDistance: 0.1f,
                farPlaneDistance: 500
                );


            _environmentBatch = new EnvironmentBatch(device, content, _projectionMatrix);
            _textBatch = new TextBatch(device, content);
            Dongle2D.Init(device, content);
        }

        public void Draw(){
            Matrix rotation = Matrix.CreateFromYawPitchRoll(ViewportYaw, -ViewportPitch, 0);
            Vector3 transformedReference = Vector3.Transform(_camReference, rotation);
            Vector3 cameraDirection = ViewportPosition + transformedReference;
            Matrix view = Matrix.CreateLookAt(ViewportPosition, cameraDirection, Vector3.Up);

            _environmentBatch.Draw(Device, view);
            Dongle2D.Draw();
            _textBatch.Draw();
        }
    }
}