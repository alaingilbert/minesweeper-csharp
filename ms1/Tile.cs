using System;
using SpriteKit;
using CoreGraphics;
using AppKit;

namespace ms1
{
    public class Tile : SKNode
    {
        public enum State
        {
            Empty,
            Discovered,
            Flagged,
        }

        private readonly int x;
        private readonly int y;
        private int size = 40;
        public State state = State.Empty;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
            RenderEmpty();
        }

        public void RenderEmpty()
        {
            RemoveAllChildren();
            DrawTile();
        }
        
        private void RenderFlag()
        {
            RemoveAllChildren();
            DrawTile();
            DrawFlag();
        }
        
        public void RenderBadFlag()
        {
            RemoveAllChildren();
            DrawTile();
            DrawFlag();
            DrawCross();
        }
        
        public void RenderMine()
        {
            RemoveAllChildren();
            DrawTile();
            DrawMine();
        }
        
        public void RenderFlaggedMine()
        {
            RemoveAllChildren();
            DrawTile();
            DrawMine();
            DrawFlag();
        }
        
        public void RenderExplodedMine()
        {
            RemoveAllChildren();
            var s = new SKShapeNode();
            var path = new CGPath();
            path.MoveToPoint(0, 0);
            path.AddLineToPoint(size, 0);
            path.AddLineToPoint(size, size);
            path.AddLineToPoint(0, size);
            path.AddLineToPoint(0, 0);
            s.Path = path.CopyByTransformingPath(CGAffineTransform.MakeTranslation(x*size, y*size));
            s.StrokeColor = NSColor.FromCalibratedRgb(0.502f, 0.502f, 0.502f);
            s.FillColor = NSColor.FromCalibratedRgb(0.8f, 0.8f, 0.8f);
            AddChild(s);
            
            var s1 = new SKShapeNode();
            var p1 = new CGPath();
            p1.AddArc(x*size+size/2, y*size+size/2, size / 4, 0, 2 * (float)Math.PI, true);
            s1.Path = p1;
            s1.StrokeColor = NSColor.FromCalibratedRgb(0.2f, 0.2f, 0.2f);
            s1.FillColor = NSColor.FromCalibratedRgb(0.8f, 0.0f, 0.0f);
            AddChild(s1);
        }

        public void RenderDiscovered(int nbMinesAround)
        {
            state = State.Discovered;
            RemoveAllChildren();
            var s = new SKShapeNode();
            var path = new CGPath();
            path.MoveToPoint(0, 0);
            path.AddLineToPoint(size, 0);
            path.AddLineToPoint(size, size);
            path.AddLineToPoint(0, size);
            path.AddLineToPoint(0, 0);
            s.Path = path.CopyByTransformingPath(CGAffineTransform.MakeTranslation(x*size, y*size));
            s.StrokeColor = NSColor.FromCalibratedRgb(0.502f, 0.502f, 0.502f);
            s.FillColor = NSColor.FromCalibratedRgb(1.0f, 1.0f, 1.0f);
            AddChild(s);

            if (nbMinesAround == 0)
                return;

            NSColor[] colors = {
                NSColor.FromCalibratedRgb(0.0f, 0.0f, 1.0f),     // Blue
                NSColor.FromCalibratedRgb(0.0f, 0.502f, 0.0f),   // Green
                NSColor.FromCalibratedRgb(1.0f, 0.0f, 0.0f),     // Red
                NSColor.FromCalibratedRgb(0.0f, 0.0f, 0.502f),   // Navy
                NSColor.FromCalibratedRgb(0.502f, 0.0f, 0.0f),   // Maroon
                NSColor.FromCalibratedRgb(0.0f, 1.0f, 1.0f),     // Aqua
                NSColor.FromCalibratedRgb(0.502f, 0.0f, 0.502f), // Purple
                NSColor.FromCalibratedRgb(0.0f, 0.0f, 0.0f),     // Black
            };

            var label = new SKLabelNode("Arial")
            {
                Text = nbMinesAround.ToString(),
                Position = new CGPoint(x*40+20, y*40+20),
                FontColor = colors[nbMinesAround-1],
                HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Center,
                VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center,
                FontSize = 38,
            };
	        
            AddChild(label);
        }

        public void RightClick()
        {
            if (state == State.Empty)
            {
                RenderFlag();
                state = State.Flagged;
            } else if (state == State.Flagged)
            {
                RenderEmpty();
                state = State.Empty;
            }
        }
        
        private void DrawTile()
        {
            var s = new SKShapeNode();
            var path = new CGPath();
            path.MoveToPoint(0, 0);
            path.AddLineToPoint(size, 0);
            path.AddLineToPoint(size, size);
            path.AddLineToPoint(0, size);
            path.AddLineToPoint(0, 0);
            s.Path = path.CopyByTransformingPath(CGAffineTransform.MakeTranslation(x*size, y*size));
            s.StrokeColor = NSColor.FromCalibratedRgb(0.502f, 0.502f, 0.502f);
            s.FillColor = NSColor.FromCalibratedRgb(0.8f, 0.8f, 0.8f);
            AddChild(s);
        }

        private void DrawFlag()
        {
            var s1 = new SKShapeNode();
            var p1 = new CGPath();
            p1.MoveToPoint(size/3.0f, size/2.0f);
            p1.AddLineToPoint(size/3.0f*2, size/3.0f);
            p1.AddLineToPoint(size/3.0f*2, size/3.0f*2);
            s1.Path = p1.CopyByTransformingPath(CGAffineTransform.MakeTranslation(x*size, y*size));
            s1.StrokeColor = NSColor.Red;
            s1.FillColor = NSColor.Red;
            AddChild(s1);
            
            var s2 = new SKShapeNode();
            var p2 = new CGPath();
            p2.MoveToPoint(size/3.0f*2, size-size/3.0f);
            p2.AddLineToPoint(size/3.0f*2, size/4.0f);
            p2.AddLineToPoint(size/2.0f, size/4.0f);
            p2.AddLineToPoint(size-size/5.0f, size/4.0f);
            s2.Path = p2.CopyByTransformingPath(CGAffineTransform.MakeTranslation(x*size, y*size));
            s2.StrokeColor = NSColor.Black;
            s2.FillColor = NSColor.FromCalibratedRgba(0.0f, 0.0f, 0.0f, 0.0f);
            s2.LineWidth = size / 20;
            AddChild(s2);
        }

        private void DrawMine()
        {
            var s1 = new SKShapeNode();
            var p1 = new CGPath();
            p1.AddArc(size/2, size/2, size/4, 0, 2 * (float)Math.PI, true);
            s1.Path = p1.CopyByTransformingPath(CGAffineTransform.MakeTranslation(x*size, y*size));
            s1.StrokeColor = NSColor.FromCalibratedRgb(0.2f, 0.2f, 0.2f);
            s1.FillColor = NSColor.FromCalibratedRgb(0.4f, 0.4f, 0.4f);
            AddChild(s1);
        }

        private void DrawCross()
        {
            var s3 = new SKShapeNode();
            var p3 = new CGPath();
            p3.MoveToPoint(size/5.0f, size/5.0f);
            p3.AddLineToPoint(size-size/5.0f, size-size/5.0f);
            p3.MoveToPoint(size/5.0f, size-size/5.0f);
            p3.AddLineToPoint(size-size/5.0f, size/5.0f);
            s3.Path = p3.CopyByTransformingPath(CGAffineTransform.MakeTranslation(x*size, y*size));
            s3.StrokeColor = NSColor.Black;
            s3.FillColor = NSColor.FromCalibratedRgba(0.0f, 0.0f, 0.0f, 0.0f);
            s3.LineWidth = size / 20;
            AddChild(s3);
        }

    }
}