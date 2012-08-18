#region

using System;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    public class Renderer{
        private readonly EnvironmentBatch _environmentBatch;
        private readonly Matrix _projectionMatrix;
        private readonly TextBatch _textBatch;
        private readonly ScreenText text;
        public float AspectRatio;
        public float CameraDistance;
        public float CameraPhi;
        public float CameraTheta;
        public GraphicsDevice Device;
        public Matrix ViewMatrix;
        public float ViewportPitch;
        public Vector3 ViewportPosition;
        public float ViewportYaw;

        public Renderer(GraphicsDevice device, ContentManager content){
            ScreenText.Init(content);
            Device = device;
            Singleton.Device = device;

            ViewportPitch = -0.5f;
            ViewportPosition = new Vector3(-176, 50, -319);
            ViewportYaw = -6.4f;

            CameraDistance = 100;

            AspectRatio = Device.Viewport.Bounds.Width/(float) Device.Viewport.Bounds.Height;
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView: 3.14f/4,
                aspectRatio: AspectRatio,
                nearPlaneDistance: 0.1f,
                farPlaneDistance: 50000
                );
            Singleton.ProjectionMatrix = _projectionMatrix;

            _environmentBatch = new EnvironmentBatch(device, content, _projectionMatrix);
            _textBatch = new TextBatch(device, content);
            ScreenData.Init(Device.Viewport.Bounds.Width, Device.Viewport.Bounds.Height);
            text = new ScreenText(0, 30, "not init");

            RenderPanel.Init();
            //BufferObject.Init(device, _projectionMatrix);
        }

        public void Draw(){
            var position = new Vector3();
            position.X = (float) (CameraDistance*Math.Cos(CameraPhi)*Math.Sin(CameraTheta));
            position.Z = (float) (CameraDistance*Math.Cos(CameraPhi)*Math.Cos(CameraTheta));
            position.Y = (float) (CameraDistance*Math.Sin(CameraPhi));

            ViewMatrix = Matrix.CreateLookAt(position, Vector3.Zero, Vector3.Up);
            text.EditText("X:" + position.X + " Y:" + position.Y + " Z:" + position.Z);
            //_environmentBatch.Draw(Device, ViewMatrix);


            //_textBatch.Draw();
            RenderPanel.Draw(ViewMatrix);
        }
    }
}