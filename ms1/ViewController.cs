using System;

using AppKit;
using Foundation;
using SpriteKit;
using CoreGraphics;

namespace ms1
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            SKView skView = new GameView(View.Frame);
            //skView.ShowsDrawCount = true;
            ///skView.ShowsNodeCount = true;
            //skView.ShowsFPS = true;
            View.AddSubview(skView);
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
