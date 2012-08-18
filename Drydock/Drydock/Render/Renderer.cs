﻿#region

using System;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Render{
    public static class Renderer{
        static private EnvironmentBatch _environmentBatch;
        static private Matrix _projectionMatrix;
        static private TextBatch _textBatch;
        static private ScreenText text;
        static private SpriteBatch _batch;
        static public float AspectRatio;
        static public float CameraDistance;
        static public float CameraPhi;
        static public float CameraTheta;
        static public GraphicsDevice Device;
        static public Vector3 CameraTarget;

        static public void Init(GraphicsDevice device, ContentManager content) {
            ScreenText.Init(content);
            Device = device;
            Singleton.Device = device;

            CameraTarget = new Vector3();
            CameraPhi = 0.32f;
            CameraTheta = 0.63f;
            CameraDistance = 300;

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
            _batch = new SpriteBatch(device);
            RenderPanel.Init();
            //BufferObject.Init(device, _projectionMatrix);
        }

        static public void Draw(){
            var position = new Vector3();
            position.X = (float)(CameraDistance * Math.Cos(CameraPhi) * Math.Sin(CameraTheta)) + CameraTarget.X;
            position.Z = (float)(CameraDistance * Math.Cos(CameraPhi) * Math.Cos(CameraTheta)) + CameraTarget.Z;
            position.Y = (float)(CameraDistance * Math.Sin(CameraPhi)) + CameraTarget.Y;

            var viewMatrix = Matrix.CreateLookAt(position, CameraTarget, Vector3.Up);
            

            RenderPanel.Draw(viewMatrix);
            ScreenText.Draw(_batch);
        }
    }
}