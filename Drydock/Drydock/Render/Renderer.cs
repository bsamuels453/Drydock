#region

using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    public static class Renderer{
        static EnvironmentBatch _environmentBatch;
        static Matrix _projectionMatrix;
        static SpriteBatch _batch;
        public static float AspectRatio;
        public static GraphicsDevice Device;
        public static Vector3 CameraPosition;
        public static Vector3 CameraTarget;

        public static void Init(GraphicsDevice device, ContentManager content){
            ScreenText.Init(content);
            Device = device;
            Singleton.Device = device;

            CameraTarget = new Vector3();
            CameraPosition = new Vector3();

            AspectRatio = Device.Viewport.Bounds.Width/(float) Device.Viewport.Bounds.Height;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView: 3.14f/4,
                aspectRatio: AspectRatio,
                nearPlaneDistance: 0.1f,
                farPlaneDistance: 50000
                );
            Singleton.ProjectionMatrix = _projectionMatrix;

            _environmentBatch = new EnvironmentBatch(device, content, _projectionMatrix);
            ScreenData.Init(Device.Viewport.Bounds.Width, Device.Viewport.Bounds.Height);
            _batch = new SpriteBatch(device);
            RenderPanel.Init();
            //BufferObject.Init(device, _projectionMatrix);
        }

        public static void Draw(){
            var viewMatrix = Matrix.CreateLookAt(CameraPosition, CameraTarget, Vector3.Up);


            RenderPanel.Draw(viewMatrix);
            ScreenText.Draw(_batch);
        }
    }
}