using SpriteKit;
using CoreGraphics;

namespace ms1
{
    public sealed class GameView : SKView
    {
        public GameView(CGRect frame) : base(frame)
        {
            var scene = new GameScene(Bounds.Size);
	        PresentScene(scene);
        }
    }
}
