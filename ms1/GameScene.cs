using System;
using System.Collections.Generic;
using CoreGraphics;
using SpriteKit;
using AppKit;

namespace ms1
{
    public class GameScene : SKScene
    {
        enum State
        {
            Waiting,
            Started,
            GameOver,
            Win,
        }

        private State state = State.Waiting;
        private int tileSize = 40;
        private int nbMines = 50;
        private int nbHorizontalTiles = 19;
        private int nbVerticalTiles = 13;
        private bool[] data;
        private Tile[] tiles;
        private int safe;
        
        /**
         * Constructor for the scene.
         */
        public GameScene(CGSize size) : base(size)
        {
            var nbTiles = nbHorizontalTiles * nbVerticalTiles;
            tiles = new Tile[nbTiles];
            data = new bool[nbTiles];
            for (int i = 0; i < nbTiles; i++)
            {
                int y = i / nbHorizontalTiles;
                int x = i - y * nbHorizontalTiles;
                var t = new Tile(x, y);
                tiles[i] = t;
                AddChild(t);
            }
            Reset();
        }


        /**
         * Reset board and data.
         */
        private void Reset()
        {
            safe = 0;
            var nbTiles = nbHorizontalTiles * nbVerticalTiles;
            for (int i = 0; i < nbTiles; i++)
            {
                data[i] = false;
                tiles[i].state = Tile.State.Empty;
                tiles[i].RenderEmpty();
            }
        }

        /**
         * Returns if a specified idx is a mine.
         */
        private bool IsMine(int idx)
        {
            return data[idx];
        }

        /**
         * Returns if a specified idx is flagged.
         */
        private bool IsFlag(int idx)
        {
            return tiles[idx].state == Tile.State.Flagged;
        }

        /**
         * Returns matching coordinate of idx.
         */
        private Tuple<int, int> CoordFromIdx(int idx)
        {
            int y = idx / nbHorizontalTiles;
            int x = idx - y * nbHorizontalTiles;
            return new Tuple<int, int>(x, y);
        }

        /**
         * Returns the neighbor idexes of idx.
         */
        private List<int> NeighborIdx(int idx)
        {
            var (x, y) = CoordFromIdx(idx);
            var neighbors = NeighborCoord(x, y);
            var result = new List<int>();
            foreach (var (nx, ny) in neighbors)
                result.Add(IdxFromCoordinate(nx, ny));
            return result;
        }

        /**
         * Return either or not idx is located around a location.
         * Also returns false if a mine is at this location.
         */
        private bool Around(int idx, int x, int y)
        {
            int initialClickPosition = IdxFromCoordinate(x, y);
            var neighborsIdx = NeighborIdx(initialClickPosition);
            return neighborsIdx.IndexOf(idx) != -1 ||
                   idx == initialClickPosition ||
                   IsMine(idx);
        }

        /**
         * Place mines at random positions.
         */
        private void InitBoard(int x, int y)
        {
            Random rnd = new Random();
            for (int i = 0; i < nbMines; i++)
            {
                int val;
                for (;;)
                {
                    val = rnd.Next(0, nbHorizontalTiles * nbVerticalTiles - 1);
                    if (!Around(val, x, y)) break;
                }
                data[val] = true;
            }

            state = State.Started;
        }

        /**
         * Returns a linear index from a location.
         */
        private int IdxFromCoordinate(int x, int y)
        {
            return y * nbHorizontalTiles + x;
        }

        /**
         * Returns either or not a location is valid.
         */
        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < nbHorizontalTiles &&
                   y >= 0 && y < nbVerticalTiles;
        }

        /**
         * List of locations around a specified location.
         */
        private List<Tuple<int, int>> NeighborCoord(int x, int y)
        {
            var result = new List<Tuple<int, int>>();
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (!IsValidPosition(x + dx, y + dy)) continue;
                    result.Add(new Tuple<int, int>(x + dx, y + dy));
                }
            }
            return result;
        }

        /**
         * Count mines around a location.
         */
        private int CountMinesAround(int x, int y)
        {
            int resNbMines = 0;
            foreach (var (nx, ny) in NeighborCoord(x, y))
                if (IsMine(IdxFromCoordinate(nx, ny)))
                    resNbMines++;
            
            return resNbMines;
        }

        /**
         * Count flags around a location.
         */
        private int CountFlagsAround(int x, int y)
        {
            int nbFlags = 0;
            foreach (var (nx, ny) in NeighborCoord(x, y))
            {
                if (IsFlag(IdxFromCoordinate(nx, ny)))
                    nbFlags++;
            }

            return nbFlags;
        }

        /**
         * Render all mines.
         */
        private void ShowMines(int deadIdx)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                var (x, y) = CoordFromIdx(i);
                if (IsMine(i) || IsFlag(i))
                {
                    if (i == deadIdx)
                    {
                        tiles[i].RenderExplodedMine();
                    }
                    else if (IsMine(i) && IsFlag(i))
                    {
                        tiles[i].RenderFlaggedMine();
                    }
                    else if (IsFlag(i))
                    {
                        tiles[i].RenderBadFlag();
                    }
                    else
                    {
                        tiles[i].RenderMine();
                    }
                }
            }
        }
        
        /**
         * Game over, display all the mines.
         */
        private void GameOver(int deadIdx)
        {
            ShowMines(deadIdx);
        }

        private void Win()
        {
            state = State.Win;
            ShowMines(-1);
            var background = new SKShapeNode();
            var p = new CGPath();
            p.MoveToPoint(0, 0);
            p.AddLineToPoint(40 * 19, 0);
            p.AddLineToPoint(40 * 19, 40 * 13);
            p.AddLineToPoint(0, 40 * 13);
            p.AddLineToPoint(0, 0);
            background.Path = p;
            background.FillColor = NSColor.FromCalibratedRgba(1.0f, 1.0f, 1.0f, 0.7f);
            AddChild(background);

            var label = new SKLabelNode("Arial");
            label.Text = "Win";
            label.FontColor = NSColor.FromCalibratedRgb(0.0f, 0.502f, 0.0f);
            label.FontSize = 40;
            label.Position = new CGPoint(40 * 19 / 2, 40 * 13 / 2);
            label.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;
            label.HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Center;
            AddChild(label);
        }

        /**
         * Click on the tile at x, y.
         */
        private void ShowTile(int x, int y)
        {
            int tileIdx = IdxFromCoordinate(x, y);
            var tile = tiles[tileIdx];

            if (tile.state == Tile.State.Discovered)
                return;
            if (tile.state == Tile.State.Flagged)
                return;
            
            if (IsMine(tileIdx))
            {
                tile.RenderMine();
                state = State.GameOver;
                GameOver(tileIdx);
                return;
            }

            int nbMinesAround = CountMinesAround(x, y);

            safe++;
            
            tile.RenderDiscovered(nbMinesAround);
            
            if (nbMinesAround == 0)
                foreach (var (nx, ny) in NeighborCoord(x, y))
                    ShowTile(nx, ny);
        }

        /**
         * Handle mouseup on the scene.
         */
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            int tileX = (int)Math.Floor(theEvent.LocationInWindow.X / tileSize);
            int tileY = (int)Math.Floor(theEvent.LocationInWindow.Y / tileSize);
            
            int tileIdx = IdxFromCoordinate(tileX, tileY);
            var tile = tiles[tileIdx];
            
            if (state == State.Waiting)
            {
                InitBoard(tileX, tileY);
            }
            
            if (state == State.Started)
            {
                if (tile.state == Tile.State.Discovered)
                {
                    if (CountFlagsAround(tileX, tileY) == CountMinesAround(tileX, tileY))
                    {
                        foreach (var (nx, ny) in NeighborCoord(tileX, tileY))
                        {
                            ShowTile(nx, ny);
                        }
                    }
                }
                else
                {
                    ShowTile(tileX, tileY);   
                }
                if (safe == tiles.Length - nbMines)
                {
                    Win();
                }
            }
            else if (state == State.GameOver || state == State.Win)
            {
                Reset();
                state = State.Waiting;
            }
        }

        /**
         * Handle right click on the scene.
         */
        public override void RightMouseUp(NSEvent theEvent)
        {
            base.RightMouseUp(theEvent);
            int tileX = (int)Math.Floor(theEvent.LocationInWindow.X / tileSize);
            int tileY = (int)Math.Floor(theEvent.LocationInWindow.Y / tileSize);
            int tileIdx = IdxFromCoordinate(tileX, tileY);
            if (state == State.Started)
            {
                Tile tile = tiles[tileIdx];
                tile.RightClick();
            }
        }
    }
}